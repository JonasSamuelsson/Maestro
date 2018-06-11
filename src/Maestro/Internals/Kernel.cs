using Maestro.Configuration;
using Maestro.TypeFactoryResolvers;
using Maestro.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Maestro.Internals
{
	internal class Kernel : IDisposable
	{
		private static readonly IReadOnlyList<IFactoryProviderResolver> FactoryProviderResolvers = new IFactoryProviderResolver[]
		{
			new FuncFactoryProviderResolver(),
			new LazyFactoryProviderResolver(),
			new ConcreteClosedClassFactoryProviderResolver(),
			new EmptyEnumerableFactoryProviderResolver()
		};

		private readonly ServiceDescriptorLookup _serviceDescriptorLookup;
		private readonly PipelineCache<long> _pipelineCache;

		internal event EventHandler ConfigurationChanged;

		internal Kernel()
		{
			_serviceDescriptorLookup = new ServiceDescriptorLookup();
			_pipelineCache = new PipelineCache<long>();
		}

		internal ContainerSettings Settings { get; } = new ContainerSettings();
		internal List<Func<Type, bool>> AutoResolveFilters { get; } = new List<Func<Type, bool>>();

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
			var enumerablePipeline = new EnumerablePipeline(elementType);

			if (_serviceDescriptorLookup.TryGetServiceDescriptor(type, name, out var serviceDescriptor))
			{
				enumerablePipeline.Add(CreatePipeline(PipelineType.Services, serviceDescriptor, context));
				pipeline = enumerablePipeline;
				return true;
			}

			const string defaultName = ServiceNames.Default;
			if (name != defaultName && _serviceDescriptorLookup.TryGetServiceDescriptor(type, defaultName, out serviceDescriptor))
			{
				enumerablePipeline.Add(CreatePipeline(PipelineType.Services, serviceDescriptor, context));
				pipeline = enumerablePipeline;
				return true;
			}

			if (_serviceDescriptorLookup.TryGetServiceDescriptors(elementType, out var serviceDescriptors))
			{
				if (Settings.GetServicesOrder == GetServicesOrder.Ordered)
				{
					serviceDescriptors = serviceDescriptors.OrderBy(x => x.SortOrder).ToList();
				}

				for (var i = 0; i < serviceDescriptors.Count; i++)
				{
					var descriptor = serviceDescriptors[i];
					enumerablePipeline.Add(CreatePipeline(PipelineType.Service, descriptor, context));
				}
			}

			if (enumerablePipeline.Any)
			{
				pipeline = enumerablePipeline;
				return true;
			}

			return false;
		}

		private bool TryGetPipelineFromFactoryProviders(Type type, string name, Context context, bool typeIsIEnumerableOfT, ref IPipeline pipeline)
		{
			if (AutoResolveFilters.Count != 0)
			{
				var match = false;

				for (var i = 0; i < AutoResolveFilters.Count; i++)
				{
					var filter = AutoResolveFilters[i];
					if (!filter.Invoke(type)) continue;
					match = true;
					break;
				}

				if (!match) return false;
			}

			for (var i = 0; i < FactoryProviderResolvers.Count; i++)
			{
				var factoryProviderResolver = FactoryProviderResolvers[i];

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
			if (_serviceDescriptorLookup.TryGetServiceDescriptor(type, name, out serviceDescriptor))
				return true;

			const string defaultName = ServiceNames.Default;
			if (name != defaultName &&
				 _serviceDescriptorLookup.TryGetServiceDescriptor(type, defaultName, out serviceDescriptor))
				return true;

			serviceDescriptor = null;
			return false;
		}

		public void Dispose()
		{
		}

		internal void Populate(Diagnostics.Configuration configuration)
		{
			_serviceDescriptorLookup.Populate(configuration.Services);
		}
	}
}