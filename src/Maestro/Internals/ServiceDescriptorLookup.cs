using Maestro.Configuration;
using Maestro.Diagnostics;
using Maestro.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;

namespace Maestro.Internals
{
	internal class ServiceDescriptorLookup : IDisposable
	{
		private int _idCounter;
		private int _sortOrderCounter;

		private readonly ThreadSafeDictionary<Type, ServiceFamily> _serviceFamilies = new ThreadSafeDictionary<Type, ServiceFamily>();

		internal EventHandler<EventArgs> ServiceDescriptorAdded;

		public bool Add(ServiceDescriptor serviceDescriptor, ServiceRegistrationPolicy serviceRegistrationPolicy)
		{
			return Add(serviceDescriptor, serviceRegistrationPolicy, true);
		}

		public bool Add(ServiceDescriptor serviceDescriptor, ServiceRegistrationPolicy serviceRegistrationPolicy, bool triggerServiceDescriptorAddedEvent)
		{
			serviceDescriptor.Id = Interlocked.Increment(ref _idCounter);

			if (serviceDescriptor.SortOrder == 0)
			{
				serviceDescriptor.SortOrder = Interlocked.Increment(ref _sortOrderCounter);
			}

			var serviceFamily = _serviceFamilies.GetOrAdd(serviceDescriptor.Type, type => new ServiceFamily { Type = type });
			var serviceDescriptorName = serviceDescriptor.Name;

			switch (serviceRegistrationPolicy)
			{
				case ServiceRegistrationPolicy.AddOrThrow:
					if (!serviceFamily.Dictionary.TryAdd(ServiceNames.Default, serviceDescriptor))
						throw new DuplicateServiceRegistrationException($"Service of type '{serviceDescriptor.Type.ToFormattedString()}' has already been registered.");
					break;
				case ServiceRegistrationPolicy.AddOrUpdate:
					serviceFamily.Dictionary.AddOrUpdate(ServiceNames.Default, serviceDescriptor);
					break;
				case ServiceRegistrationPolicy.TryAdd:
					if (!serviceFamily.Dictionary.TryAdd(ServiceNames.Default, serviceDescriptor))
						return false;
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(serviceRegistrationPolicy), serviceRegistrationPolicy, null);
			}

			serviceFamily.Dictionary.AddOrUpdate(serviceDescriptorName, serviceDescriptor);
			serviceFamily.List.Add(serviceDescriptor);

			if (triggerServiceDescriptorAddedEvent)
			{
				ServiceDescriptorAdded.Invoke(this, EventArgs.Empty);
			}

			return true;
		}

		public bool TryGetServiceDescriptor(Type type, string name, out ServiceDescriptor serviceDescriptor)
		{
			ServiceFamily serviceFamily;

			if (Reflector.IsGeneric(type, out var genericTypeDefinition, out var genericArguments))
			{
				if (_serviceFamilies.TryGet(genericTypeDefinition, out serviceFamily))
				{
					BuildGenericServiceFamilyFrom(serviceFamily, genericArguments);
				}
			}

			if (_serviceFamilies.TryGet(type, out serviceFamily))
			{
				if (TryGetServiceDescriptor(serviceFamily, name, out serviceDescriptor))
				{
					return true;
				}
			}

			serviceDescriptor = null;
			return false;
		}

		private void BuildGenericServiceFamilyFrom(ServiceFamily serviceFamily, Type[] genericArguments)
		{
			var type = serviceFamily.Type.MakeGenericType(genericArguments);
			var sf = _serviceFamilies.GetOrAdd(type, () => new ServiceFamily { Type = type });
			var keys = sf.Dictionary.Keys.ToList();

			foreach (var serviceDescriptor in serviceFamily.List)
			{
				if (sf.List.Any(x => x.CorrelationId == serviceDescriptor.CorrelationId))
					continue;

				var sd = serviceDescriptor.MakeGeneric(genericArguments);

				if (!keys.Contains(sd.Name))
				{
					sf.Dictionary.TryAdd(ServiceNames.Default, sd);
					sf.Dictionary.TryAdd(sd.Name, sd);
				}

				sf.List.Add(sd);
			}
		}

		private static bool TryGetServiceDescriptor(ServiceFamily serviceFamily, string name, out ServiceDescriptor serviceDescriptor)
		{
			if (serviceFamily.Dictionary.TryGet(name, out serviceDescriptor))
			{
				return true;
			}

			const string defaultName = ServiceNames.Default;
			return name != defaultName && serviceFamily.Dictionary.TryGet(defaultName, out serviceDescriptor);
		}

		public bool TryGetServiceDescriptors(Type type, out IReadOnlyList<ServiceDescriptor> serviceDescriptors)
		{
			ServiceFamily serviceFamily;

			if (Reflector.IsGeneric(type, out var genericTypeDefinition, out var genericArguments))
			{
				if (_serviceFamilies.TryGet(genericTypeDefinition, out serviceFamily))
				{
					BuildGenericServiceFamilyFrom(serviceFamily, genericArguments);
				}
			}

			if (_serviceFamilies.TryGet(type, out serviceFamily))
			{
				if (serviceFamily.List.Count != 0)
				{
					serviceDescriptors = serviceFamily.List.OrderBy(x => x.SortOrder).ToList();
					return true;
				}
			}

			serviceDescriptors = null;
			return false;
		}

		public void Dispose()
		{
			_serviceFamilies.Values.ForEach(sf =>
			{
				sf.List.ForEach(DisposeServiceDescriptor);
				sf.Dictionary.Values.ForEach(DisposeServiceDescriptor);
			});
		}

		[SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global")]
		private static void DisposeServiceDescriptor(ServiceDescriptor sd)
		{
			(sd.FactoryProvider as IDisposable)?.Dispose();
			sd.Interceptors.OfType<IDisposable>().ForEach(x => x.Dispose());
			(sd.Lifetime as IDisposable)?.Dispose();
		}

		public void Populate(List<Service> services)
		{
			foreach (var serviceFamily in _serviceFamilies.OrderBy(x => x.Key.FullName).Select(x => x.Value))
			{
				var serviceDescriptors = serviceFamily.Dictionary
					.Select(x => x.Value)
					.OrderBy(x => x.Name)
					.Concat(serviceFamily.List);

				foreach (var serviceDescriptor in serviceDescriptors)
				{
					services.Add(new Service
					{
						// ReSharper disable once PossibleInvalidOperationException
						Id = serviceDescriptor.Id.Value,
						InstanceType = serviceDescriptor.FactoryProvider.GetInstanceType(),
						Lifetime = serviceDescriptor.Lifetime.ToString(),
						Name = serviceDescriptor.Name,
						Provider = serviceDescriptor.FactoryProvider.ToString(),
						ServiceType = serviceFamily.Type
					});
				}
			}
		}
	}
}