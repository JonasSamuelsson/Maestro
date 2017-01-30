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

		public IList<ITypeProvider> TypeProviders { get; } = new List<ITypeProvider>();

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

		public bool CanGetService(Type type, string name, Context context)
		{
			IPipeline pipeline;
			return TryGetPipeline(type, name, context, out pipeline);
		}

		public bool TryGetService(Type type, string name, Context context, out object instance)
		{
			IPipeline pipeline;
			instance = TryGetPipeline(type, name, context, out pipeline) ? pipeline.Execute(context) : null;
			return instance != null;
		}

		private bool TryGetPipeline(Type type, string name, Context context, out IPipeline pipeline)
		{
			var pipelineKey = GetPipelineCacheKey(type, name);
			if (!_pipelineCache.TryGet(pipelineKey, out pipeline))
			{
				lock (_pipelineCache)
				{
					if (!_pipelineCache.TryGet(pipelineKey, out pipeline))
					{
						Type elementType;
						var isGenericEnumerable = Reflector.IsGenericEnumerable(type, out elementType);

						if (!isGenericEnumerable)
						{
							ServiceDescriptor serviceDescriptor;
							if (TryGetServiceDescriptor(type, name, out serviceDescriptor))
							{
								pipeline = CreatePipeline(PipelineType.Service, serviceDescriptor, context);
								_pipelineCache.Add(pipelineKey, pipeline);
								return true;
							}
						}
						else
						{
							var compoundPipeline = new ComposedPipeline(elementType);

							for (var kernel = this; kernel != null; kernel = kernel._parent)
							{
								var temp = name;

								tryGetService:

								ServiceDescriptor serviceDescriptor;
								if (kernel._serviceDescriptorLookup.TryGetServiceDescriptor(type, temp, out serviceDescriptor))
								{
									compoundPipeline.Add(CreatePipeline(PipelineType.Services, serviceDescriptor, context));
									goto addToPipelineCache;
								}

								var defaultName = ServiceNames.Default;
								if (temp != defaultName)
								{
									temp = defaultName;
									goto tryGetService;
								}

								IEnumerable<ServiceDescriptor> serviceDescriptors;
								if (kernel._serviceDescriptorLookup.TryGetServiceDescriptors(elementType, out serviceDescriptors))
								{
									foreach (var descriptor in serviceDescriptors)
									{
										compoundPipeline.Add(CreatePipeline(PipelineType.Service, descriptor, context));
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
								_pipelineCache.Add(pipelineKey, pipeline);
								return true;
							}
						}

						foreach (var factoryProviderResolver in _factoryProviderResolvers)
						{
							IFactoryProvider factoryProvider;
							if (!factoryProviderResolver.TryGet(type, name, context, out factoryProvider)) continue;
							var serviceDescriptor = new ServiceDescriptor
							{
								Type = type,
								Name = name,
								FactoryProvider = factoryProvider
							};
							var pipelineType = isGenericEnumerable ? PipelineType.Services : PipelineType.Service;
							pipeline = CreatePipeline(pipelineType, serviceDescriptor, context);
							_pipelineCache.Add(pipelineKey, pipeline);
							return true;
						}

						return false;
					}
				}
			}

			return true;
		}

		private static Pipeline CreatePipeline(PipelineType pipelineType, ServiceDescriptor serviceDescriptor, Context context)
		{
			return new Pipeline(pipelineType, serviceDescriptor.FactoryProvider.GetFactory(context), serviceDescriptor.Interceptors, serviceDescriptor.Lifetime);
		}

		private static long GetPipelineCacheKey(Type type, string name)
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
			if (_parent == null) return;
			_parent.ConfigurationChanged -= ParentConfigurationChanged;
		}
	}
}