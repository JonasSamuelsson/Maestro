﻿using Maestro.FactoryProviders;
using Maestro.TypeFactoryResolvers;
using Maestro.Utils;
using System;
using System.Collections.Generic;

namespace Maestro.Internals
{
	internal class Kernel : IDisposable
	{
		private static readonly IFactoryProviderResolver[] FactoryProviderResolvers =
		{
			new ConcreteClosedClassFactoryProviderResolver(),
			new EmptyEnumerableFactoryProviderResolver()
		};

		private readonly PipelineCache _pipelineCache;

		internal Kernel()
		{
			_pipelineCache = new PipelineCache();
			AutoResolveFilters = new List<Func<Type, bool>>();
			InstanceTypeProviders = new List<IInstanceTypeProvider>();
			ServiceRegistry = new ServiceRegistry();
			ServiceRegistry.ServiceDescriptorAdded += ServiceDescriptorLookupServiceDescriptorAdded;
		}

		private void ServiceDescriptorLookupServiceDescriptorAdded(object sender, EventArgs e)
		{
			_pipelineCache.Clear();
		}

		internal List<Func<Type, bool>> AutoResolveFilters { get; }
		internal List<IInstanceTypeProvider> InstanceTypeProviders { get; }
		internal ServiceRegistry ServiceRegistry { get; }

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

				if (TryGetPipelineFromServiceDescriptors(typeIsIEnumerableOfT, type, elementType, name, context, ref pipeline))
				{
					_pipelineCache.Add(key, pipeline);
					return true;
				}

				if (TryGetPipelineFromTypeProviders(type, name, context, ref pipeline))
				{
					_pipelineCache.Add(key, pipeline);
					return true;
				}

				if (TryGetPipelineFromFactoryProviders(type, name, context, ref pipeline))
				{
					_pipelineCache.Add(key, pipeline);
					return true;
				}

				return false;
			}
		}

		private bool TryGetPipelineFromServiceDescriptors(bool typeIsIEnumerableOfT, Type type, Type elementType, string name, Context context, ref Pipeline pipeline)
		{
			return typeIsIEnumerableOfT
				? TryGetPipelineFromServiceDescriptors(type, elementType, name, context, ref pipeline)
				: TryGetPipelineFromServiceDescriptor(type, name, context, ref pipeline);
		}

		private bool TryGetPipelineFromServiceDescriptor(Type type, string name, Context context, ref Pipeline pipeline)
		{
			if (TryGetServiceDescriptor(type, name, out var serviceDescriptor))
			{
				pipeline = CreateSingleServicePipeline(serviceDescriptor, context);
				return true;
			}

			return false;
		}

		private bool TryGetPipelineFromServiceDescriptors(Type type, Type elementType, string name, Context context, ref Pipeline pipeline)
		{
			if (ServiceRegistry.TryGetServiceDescriptor(type, name, out var serviceDescriptor))
			{
				pipeline = CreateSingleServicePipeline(serviceDescriptor, context);
				return true;
			}

			if (ServiceRegistry.TryGetServiceDescriptors(elementType, out var serviceDescriptors))
			{
				var compositePipeline = new CompositePipeline(elementType);

				// ReSharper disable once ForCanBeConvertedToForeach
				for (var i = 0; i < serviceDescriptors.Length; i++)
				{
					var descriptor = serviceDescriptors[i];
					compositePipeline.Add(CreateSingleServicePipeline(descriptor, context));
				}

				pipeline = compositePipeline;
				return true;
			}

			return false;
		}

		private bool TryGetPipelineFromTypeProviders(Type type, string name, Context context, ref Pipeline pipeline)
		{
			// ReSharper disable once ForCanBeConvertedToForeach
			for (var i = 0; i < InstanceTypeProviders.Count; i++)
			{
				var typeProvider = InstanceTypeProviders[i];

				if (!typeProvider.TryGetInstanceType(type, context, out var instanceType))
					continue;

				var serviceDescriptor = new ServiceDescriptor
				{
					Type = type,
					Name = name,
					FactoryProvider = new TypeFactoryProvider(instanceType, name)
				};
				pipeline = CreateSingleServicePipeline(serviceDescriptor, context);
				return true;
			}

			return false;
		}

		private bool TryGetPipelineFromFactoryProviders(Type type, string name, Context context, ref Pipeline pipeline)
		{
			if (AutoResolveFilters.Count != 0)
			{
				var match = false;

				// ReSharper disable once ForCanBeConvertedToForeach
				// ReSharper disable once LoopCanBeConvertedToQuery
				for (var i = 0; i < AutoResolveFilters.Count; i++)
				{
					var filter = AutoResolveFilters[i];
					if (!filter.Invoke(type)) continue;
					match = true;
					break;
				}

				if (!match) return false;
			}

			// ReSharper disable once ForCanBeConvertedToForeach
			for (var i = 0; i < FactoryProviderResolvers.Length; i++)
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
			if (ServiceRegistry.TryGetServiceDescriptor(type, name, out serviceDescriptor))
				return true;

			serviceDescriptor = null;
			return false;
		}

		public void Dispose()
		{
			// ReSharper disable once DelegateSubtraction
			ServiceRegistry.ServiceDescriptorAdded -= ServiceDescriptorLookupServiceDescriptorAdded;
		}

		internal void Populate(Diagnostics.Configuration configuration)
		{
			_pipelineCache.Populate(configuration.Pipelines);
			ServiceRegistry.Populate(configuration.Services);
		}
	}
}