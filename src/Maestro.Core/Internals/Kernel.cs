using System;
using System.Collections.Generic;
using Maestro.FactoryProviders;
using Maestro.TypeFactoryResolvers;
using Maestro.Utils;

namespace Maestro.Internals
{
	internal class Kernel : IDisposable
	{
		private readonly ServiceDescriptorLookup _serviceDescriptorLookup;
		private readonly PipelineCache<long> _pipelineCache;
		private readonly IEnumerable<IFactoryProviderResolver> _factoryProviderResolvers;
		private readonly Kernel _parent;

		public event EventHandler ConfigurationChanged;

		public Kernel() : this(new ServiceDescriptorLookup())
		{ }

		public Kernel(ServiceDescriptorLookup serviceDescriptorLookup)
		{
			_serviceDescriptorLookup = serviceDescriptorLookup;
			_pipelineCache = new PipelineCache<long>();
			_factoryProviderResolvers = new IFactoryProviderResolver[]
											{
												new FuncFactoryProviderResolver(),
												new LazyFactoryProviderResolver(),
												new ConcreteClosedClassFactoryProviderResolver(),
												new EmptyEnumerableFactoryProviderResolver()
											};
		}

		public Kernel(Kernel kernel) : this()
		{
			_parent = kernel;
			_parent.ConfigurationChanged += ParentConfigurationChanged;
		}

		private void ParentConfigurationChanged(object sender, EventArgs e)
		{
			lock (_pipelineCache)
			{
				_pipelineCache.Clear();
				ConfigurationChanged?.Invoke(this, EventArgs.Empty);
			}
		}

		public bool Add(ServiceDescriptor serviceDescriptor, bool throwIfDuplicate)
		{
			lock (_pipelineCache)
			{
				_pipelineCache.Clear();
				var added = _serviceDescriptorLookup.Add(serviceDescriptor, throwIfDuplicate);
				ConfigurationChanged?.Invoke(this, EventArgs.Empty);
				return added;
			}
		}

		public bool CanGetService(Type type, Context context)
		{
			IPipeline pipeline;
			return TryGetPipeline(type, context, out pipeline);
		}

		public bool TryGetService(Type type, Context context, out object instance)
		{
			IPipeline pipeline;
			instance = TryGetPipeline(type, context, out pipeline) ? pipeline.Execute(context) : null;
			return instance != null;
		}

		public IEnumerable<object> GetServices(Type type, Context context)
		{
			var enumerableType = typeof(IEnumerable<>).MakeGenericType(type);

			object instance;
			if (TryGetService(enumerableType, context, out instance))
			{
				return (IEnumerable<object>)instance;
			}

			throw new InvalidOperationException();
		}

		private bool TryGetPipeline(Type type, Context context, out IPipeline pipeline)
		{
			var pipelineKey = GetPipelineCacheKey(type, context);
			if (!_pipelineCache.TryGet(pipelineKey, out pipeline))
			{
				lock (_pipelineCache)
				{
					if (!_pipelineCache.TryGet(pipelineKey, out pipeline))
					{
						Type elementType;
						if (!Reflector.IsGenericEnumerable(type, out elementType))
						{
							ServiceDescriptor serviceDescriptor;
							if (TryGetServiceDescriptor(type, context.Name, out serviceDescriptor))
							{
								pipeline = new Pipeline(serviceDescriptor);
								_pipelineCache.Add(pipelineKey, pipeline);
								return true;
							}
						}
						else
						{
							var compoundPipeline = new CompoundPipeline(elementType);

							for (var kernel = this; kernel != null; kernel = kernel._parent)
							{
								var name = context.Name;

								tryGetService:

								ServiceDescriptor serviceDescriptor;
								if (kernel._serviceDescriptorLookup.TryGetServiceDescriptor(type, name, out serviceDescriptor))
								{
									compoundPipeline.Add(new EnumerablePipeline(elementType, serviceDescriptor));
									goto addToPipelineCache;
								}

								if (name != ServiceDescriptorLookup.DefaultName)
								{
									name = ServiceDescriptorLookup.DefaultName;
									goto tryGetService;
								}

								IEnumerable<ServiceDescriptor> serviceDescriptors;
								if (kernel._serviceDescriptorLookup.TryGetServiceDescriptors(elementType, out serviceDescriptors))
								{
									foreach (var descriptor in serviceDescriptors)
									{
										compoundPipeline.Add(new Pipeline(descriptor));
									}
								}
							}

							addToPipelineCache:

							if (compoundPipeline.Any())
							{
								_pipelineCache.Add(pipelineKey, compoundPipeline);
								pipeline = compoundPipeline;
								return true;
							}
						}

						foreach (var factoryProviderResolver in _factoryProviderResolvers)
						{
							IFactoryProvider factoryProvider;
							if (!factoryProviderResolver.TryGet(type, context, out factoryProvider)) continue;
							pipeline = new Pipeline
							{
								ServiceDescriptor = new ServiceDescriptor
								{
									Type = type,
									Name = context.Name,
									FactoryProvider = factoryProvider
								}
							};
							_pipelineCache.Add(pipelineKey, pipeline);
							return true;
						}

						return false;
					}
				}
			}

			return true;
		}

		private bool TryGetServicePipeline(Type type, Context context, ref IPipeline pipeline, long pipelineKey)
		{
			ServiceDescriptor serviceDescriptor;
			if (TryGetServiceDescriptor(type, context.Name, out serviceDescriptor))
			{
				pipeline = new Pipeline(serviceDescriptor);
				return true;
			}

			return false;
		}

		private static long GetPipelineCacheKey(Type type, Context context)
		{
			long key = type.GetHashCode();
			key = (key << 32);
			key = key | (uint)context.Name.GetHashCode();
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

			var defaultName = ServiceDescriptorLookup.DefaultName;
			if (name != defaultName)
			{
				return TryGetServiceDescriptor(type, defaultName, out serviceDescriptor);
			}

			serviceDescriptor = null;
			return false;
		}

		public void Dispose()
		{
			if (_parent == null) return;
			_parent.ConfigurationChanged -= ParentConfigurationChanged;
		}
	}
}