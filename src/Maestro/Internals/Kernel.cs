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

		private readonly PipelineCache _pipelineCache;

		internal Kernel()
		{
			_pipelineCache = new PipelineCache();
			AutoResolveFilters = new List<Func<Type, bool>>();
			Settings = new ContainerSettings();
			ServiceDescriptors = new ServiceDescriptorLookup();
			ServiceDescriptors.ServiceDescriptorAdded += ServiceDescriptorLookupServiceDescriptorAdded;
		}

		private void ServiceDescriptorLookupServiceDescriptorAdded(object sender, EventArgs e)
		{
			_pipelineCache.Clear();
		}

		internal List<Func<Type, bool>> AutoResolveFilters { get; }
		internal ContainerSettings Settings { get; }
		internal ServiceDescriptorLookup ServiceDescriptors { get; }

		internal bool CanGetService(Type type, string name, Context context)
		{
			return TryGetPipeline(type, name, context, out _);
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

		internal bool TryGetPipeline(Type type, string name, Context context, out Pipeline pipeline)
		{
			var key = new PipelineCache.Key(type, name);

			if (_pipelineCache.TryGet(key, out pipeline))
				return true;

			lock (_pipelineCache)
			{
				if (_pipelineCache.TryGet(key, out pipeline))
					return true;

				var typeIsIEnumerableOfT = Reflector.IsGenericEnumerable(type, out var elementType);

				if (TryGetPipelineFromServiceDesriptors(typeIsIEnumerableOfT, type, elementType, name, context, ref pipeline))
				{
					_pipelineCache.Add(key, pipeline);
					return true;
				}

				if (TryGetPipelineFromFactoryProviders(type, name, context, typeIsIEnumerableOfT, ref pipeline))
				{
					_pipelineCache.Add(key, pipeline);
					return true;
				}

				return false;
			}
		}

		private bool TryGetPipelineFromServiceDesriptors(bool typeIsIEnumerableOfT, Type type, Type elementType, string name, Context context, ref Pipeline pipeline)
		{
			return typeIsIEnumerableOfT
				? TryGetPipelineFromServiceDesriptors(type, elementType, name, context, ref pipeline)
				: TryGetPipelineFromServiceDesriptor(type, name, context, ref pipeline);
		}

		private bool TryGetPipelineFromServiceDesriptor(Type type, string name, Context context, ref Pipeline pipeline)
		{
			if (TryGetServiceDescriptor(type, name, out var serviceDescriptor))
			{
				pipeline = CreateSingleServicePipeline(serviceDescriptor, context);
				return true;
			}

			return false;
		}

		private bool TryGetPipelineFromServiceDesriptors(Type type, Type elementType, string name, Context context, ref Pipeline pipeline)
		{
			if (ServiceDescriptors.TryGetServiceDescriptor(type, name, out var serviceDescriptor))
			{
				pipeline = CreateSingleServicePipeline(serviceDescriptor, context);
				return true;
			}

			const string defaultName = ServiceNames.Default;
			if (name != defaultName && ServiceDescriptors.TryGetServiceDescriptor(type, defaultName, out serviceDescriptor))
			{
				pipeline = CreateSingleServicePipeline(serviceDescriptor, context);
				return true;
			}

			if (ServiceDescriptors.TryGetServiceDescriptors(elementType, out var serviceDescriptors))
			{
				var compositePipeline = new CompositePipeline(elementType);

				if (Settings.GetServicesOrder == GetServicesOrder.Ordered)
				{
					serviceDescriptors = serviceDescriptors.OrderBy(x => x.SortOrder).ToList();
				}

				for (var i = 0; i < serviceDescriptors.Count; i++)
				{
					var descriptor = serviceDescriptors[i];
					compositePipeline.Add(CreateSingleServicePipeline(descriptor, context));
				}

				pipeline = compositePipeline;
				return true;
			}

			return false;
		}

		private bool TryGetPipelineFromFactoryProviders(Type type, string name, Context context, bool typeIsIEnumerableOfT, ref Pipeline pipeline)
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
				pipeline = CreateSingleServicePipeline(serviceDescriptor, context);
				return true;
			}

			return false;
		}

		private static Pipeline CreateSingleServicePipeline(ServiceDescriptor serviceDescriptor, Context context)
		{
			var serviceId = serviceDescriptor.Id;
			var factory = serviceDescriptor.FactoryProvider.GetFactory(context);
			var interceptors = serviceDescriptor.Interceptors;
			var lifetime = serviceDescriptor.Lifetime;
			return new SingleServicePipeline(serviceId, factory, interceptors, lifetime);
		}

		private bool TryGetServiceDescriptor(Type type, string name, out ServiceDescriptor serviceDescriptor)
		{
			if (ServiceDescriptors.TryGetServiceDescriptor(type, name, out serviceDescriptor))
				return true;

			const string defaultName = ServiceNames.Default;
			if (name != defaultName && ServiceDescriptors.TryGetServiceDescriptor(type, defaultName, out serviceDescriptor))
				return true;

			serviceDescriptor = null;
			return false;
		}

		public void Dispose()
		{
			// ReSharper disable once DelegateSubtraction
			ServiceDescriptors.ServiceDescriptorAdded -= ServiceDescriptorLookupServiceDescriptorAdded;
		}

		internal void Populate(Diagnostics.Configuration configuration)
		{
			_pipelineCache.Populate(configuration.Pipelines);
			ServiceDescriptors.Populate(configuration.Services);
		}
	}
}