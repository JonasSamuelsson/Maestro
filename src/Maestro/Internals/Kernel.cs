using Maestro.Configuration;
using Maestro.FactoryProviders;
using Maestro.TypeFactoryResolvers;
using Maestro.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Maestro.Internals
{
	internal class Kernel : IDisposable
	{
		private static readonly IReadOnlyList<IFactoryProviderResolver> _factoryProviderResolvers = new IFactoryProviderResolver[]
		{
			new FuncFactoryProviderResolver(),
			new LazyFactoryProviderResolver(),
			new ConcreteClosedClassFactoryProviderResolver(),
			new EmptyEnumerableFactoryProviderResolver()
		};

		private readonly ServiceDescriptorLookup _serviceDescriptorLookup;
		private readonly PipelineCache<long> _pipelineCache;
		private readonly Kernel _parent;

		internal event EventHandler ConfigurationChanged;

		internal Kernel()
		{
			_serviceDescriptorLookup = new ServiceDescriptorLookup();
			_pipelineCache = new PipelineCache<long>();
			Root = this;
		}

		internal Kernel(Kernel parent) : this()
		{
			_parent = parent;
			_parent.ConfigurationChanged += ParentConfigurationChanged;
			AutoResolveFilters = parent.AutoResolveFilters.ToList();
			Root = _parent.Root;
		}

		internal Config Config { get; } = new Config();
		public IList<Func<Type, bool>> AutoResolveFilters { get; } = new List<Func<Type, bool>>();
		internal IList<ITypeProvider> TypeProviders { get; } = new List<ITypeProvider>();
		internal ConcurrentDictionary<object, Lazy<object>> InstanceCache { get; } = new ConcurrentDictionary<object, Lazy<object>>();
		internal Kernel Root { get; }

		private void ParentConfigurationChanged(object sender, EventArgs e)
		{
			lock (_pipelineCache)
			{
				_pipelineCache.Clear();
				ConfigurationChanged?.Invoke(this, EventArgs.Empty);
			}
		}

		internal bool Add(ServiceDescriptor serviceDescriptor, bool throwIfDuplicate)
		{
			lock (_pipelineCache)
			{
				_pipelineCache.Clear();
				var added = _serviceDescriptorLookup.Add(serviceDescriptor, throwIfDuplicate);
				ConfigurationChanged?.Invoke(this, EventArgs.Empty);
				return added;
			}
		}

		internal bool CanGetService(Type type, string name, Context context)
		{
			return TryGetPipeline(type, name, context, out var pipeline);
		}

		internal bool TryGetService(Type type, string name, Context context, out object instance)
		{
			if (TryGetPipeline(type, name, context, out var pipeline))
			{
				instance = pipeline.Execute(context);
				return true;
			}

			instance = null;
			return false;
		}

		private bool TryGetPipeline(Type type, string name, Context context, out IPipeline pipeline)
		{
			var serviceKey = GetServiceKey(type, name);

			if (_pipelineCache.TryGet(serviceKey, out pipeline))
				return true;

			lock (_pipelineCache)
			{
				if (_pipelineCache.TryGet(serviceKey, out pipeline))
					return true;

				var typeIsIEnumerableOfT = Reflector.IsGenericEnumerable(type, out var elementType);

				if (TryGetPipelineFromServiceDesriptors(typeIsIEnumerableOfT, type, elementType, name, context, ref pipeline))
				{
					_pipelineCache.Add(serviceKey, pipeline);
					return true;
				}

				if (TryGetPipelineFromTypeProviders(type, name, context, ref pipeline))
				{
					_pipelineCache.Add(serviceKey, pipeline);
					return true;
				}

				if (TryGetPipelineFromFactoryProviders(type, name, context, typeIsIEnumerableOfT, ref pipeline))
				{
					_pipelineCache.Add(serviceKey, pipeline);
					return true;
				}

				return false;
			}
		}

		private bool TryGetPipelineFromServiceDesriptors(bool typeIsIEnumerableOfT, Type type, Type elementType, string name, Context context, ref IPipeline pipeline)
		{
			return typeIsIEnumerableOfT
				? TryGetPipelineFromServiceDesriptors(type, elementType, name, context, ref pipeline)
				: TryGetPipelineFromServiceDesriptor(type, name, context, ref pipeline);
		}

		private bool TryGetPipelineFromServiceDesriptor(Type type, string name, Context context, ref IPipeline pipeline)
		{
			if (TryGetServiceDescriptor(type, name, out var serviceDescriptor))
			{
				pipeline = CreatePipeline(PipelineType.Service, serviceDescriptor, context);
				return true;
			}

			return false;
		}

		private bool TryGetPipelineFromServiceDesriptors(Type type, Type elementType, string name, Context context, ref IPipeline pipeline)
		{
			var compoundPipeline = new ComposedPipeline(elementType);

			for (var kernel = this; kernel != null; kernel = kernel._parent)
			{
				var currentName = name;

				tryGetService:

				if (kernel._serviceDescriptorLookup.TryGetServiceDescriptor(type, currentName, out var serviceDescriptor))
				{
					compoundPipeline.Add(CreatePipeline(PipelineType.Services, serviceDescriptor, context));
					pipeline = compoundPipeline;
					return true;
				}

				var defaultName = ServiceNames.Default;
				if (currentName != defaultName)
				{
					currentName = defaultName;
					goto tryGetService;
				}

				if (kernel._serviceDescriptorLookup.TryGetServiceDescriptors(elementType, out var serviceDescriptors))
				{
					if (kernel.Config.GetServicesOrder == GetServicesOrder.Ordered)
					{
						serviceDescriptors = serviceDescriptors.OrderBy(x => x.SortOrder);
					}

					foreach (var descriptor in serviceDescriptors)
					{
						compoundPipeline.Add(CreatePipeline(PipelineType.Service, descriptor, context));
					}
				}
			}

			if (compoundPipeline.Any())
			{
				pipeline = compoundPipeline;
				return true;
			}

			return false;
		}

		private bool TryGetPipelineFromTypeProviders(Type type, string name, Context context, ref IPipeline pipeline)
		{
			// introduce ServiceTypeFactoryProvider
			for (var kernel = this; kernel != null; kernel = kernel._parent)
			{
				foreach (var typeProvider in kernel.TypeProviders)
				{
					var instanceType = typeProvider.GetInstanceTypeOrNull(type, context);
					if (instanceType == null) continue;
					var serviceDescriptor = new ServiceDescriptor
					{
						Type = type,
						Name = name,
						FactoryProvider = new TypeFactoryProvider(instanceType, name)
					};
					pipeline = CreatePipeline(PipelineType.Service, serviceDescriptor, context);
					return true;
				}
			}

			return false;
		}

		private bool TryGetPipelineFromFactoryProviders(Type type, string name, Context context, bool typeIsIEnumerableOfT, ref IPipeline pipeline)
		{
			if (AutoResolveFilters.Any() && !AutoResolveFilters.Any(x => x(type)))
				return false;

			foreach (var factoryProviderResolver in _factoryProviderResolvers)
			{
				if (!factoryProviderResolver.TryGet(type, name, context, out var factoryProvider))
					continue;

				var serviceDescriptor = new ServiceDescriptor
				{
					Type = type,
					Name = name,
					FactoryProvider = factoryProvider
				};
				var pipelineType = typeIsIEnumerableOfT ? PipelineType.Services : PipelineType.Service;
				pipeline = CreatePipeline(pipelineType, serviceDescriptor, context);
				return true;
			}

			return false;
		}

		private static Pipeline CreatePipeline(PipelineType pipelineType, ServiceDescriptor serviceDescriptor, Context context)
		{
			return new Pipeline(pipelineType, serviceDescriptor.FactoryProvider.GetFactory(context), serviceDescriptor.Interceptors, serviceDescriptor.Lifetime);
		}

		private static long GetServiceKey(Type type, string name)
		{
			long key = type.GetHashCode();
			key = (key << 32);
			key = key | (uint)name.GetHashCode();
			return key;
		}

		private bool TryGetServiceDescriptor(Type type, string name, out ServiceDescriptor serviceDescriptor)
		{
			for (var kernel = this; kernel != null; kernel = kernel._parent)
			{
				if (kernel._serviceDescriptorLookup.TryGetServiceDescriptor(type, name, out serviceDescriptor))
				{
					return true;
				}
			}

			var defaultName = ServiceNames.Default;
			if (name != defaultName)
			{
				return TryGetServiceDescriptor(type, defaultName, out serviceDescriptor);
			}

			serviceDescriptor = null;
			return false;
		}

		public void Dispose()
		{
			InstanceCache.Values
				.Where(x => x.IsValueCreated)
				.Select(x => x.Value)
				.OfType<IDisposable>()
				.ForEach(x => x.Dispose());

			if (_parent == null) return;
			_parent.ConfigurationChanged -= ParentConfigurationChanged;
		}

		internal void Populate(Diagnostics.Configuration configuration)
		{
			_serviceDescriptorLookup.Populate(configuration.Services);

			if (_parent != null)
			{
				configuration.Parent = new Diagnostics.Configuration();
				_parent.Populate(configuration.Parent);
			}
		}
	}
}