using System;
using System.Collections.Generic;

namespace Maestro.Internals
{
	internal class ServiceDescriptorLookup : IDisposable
	{
		private Dictionary<Type, ServiceFamily> _serviceFamilies = new Dictionary<Type, ServiceFamily>();

		internal const string DefaultName = "";

		public static string GetRandomName()
		{
			return Guid.NewGuid().ToString();
		}

		public bool Add(ServiceDescriptor serviceDescriptor, bool throwIfDuplicate = true)
		{
			var type = serviceDescriptor.Type;
			var name = serviceDescriptor.Name;

			if (name == null)
			{
				AddToServices(serviceDescriptor);
				return true;
			}

			ServiceFamily serviceFamily;

			if (!_serviceFamilies.TryGetValue(type, out serviceFamily))
			{
				serviceFamily = new ServiceFamily();
				serviceFamily.Service.Add(serviceDescriptor.Name, serviceDescriptor);
				_serviceFamilies = new Dictionary<Type, ServiceFamily>(_serviceFamilies) { { type, serviceFamily } };
				return true;
			}

			try
			{
				serviceFamily.Service.Add(name, serviceDescriptor);
				return true;
			}
			catch (ArgumentException)
			{
				if (throwIfDuplicate) throw;
				return false;
			}
		}

		public void AddToServices(ServiceDescriptor serviceDescriptor)
		{
			var type = serviceDescriptor.Type;

			ServiceFamily serviceFamily;

			if (!_serviceFamilies.TryGetValue(type, out serviceFamily))
			{
				serviceFamily = new ServiceFamily();
				serviceFamily.Services.Add(serviceDescriptor);
				_serviceFamilies = new Dictionary<Type, ServiceFamily>(_serviceFamilies) { { type, serviceFamily } };
				return;
			}

			serviceFamily.Services.Add(serviceDescriptor);
		}

		public bool TryGetServiceDescriptor(Type type, string name, out ServiceDescriptor serviceDescriptor)
		{
			ServiceFamily serviceFamily;

			if (_serviceFamilies.TryGetValue(type, out serviceFamily))
			{
				if (serviceFamily.Service.TryGetValue(name, out serviceDescriptor))
				{
					return true;
				}
			}

			if (type.IsGenericType && !type.IsGenericTypeDefinition)
			{
				var genericTypeDefinition = type.GetGenericTypeDefinition();
				return TryGetServiceDescriptor(genericTypeDefinition, name, out serviceDescriptor);
			}

			serviceDescriptor = null;
			return false;
		}

		public bool TryGetServiceDescriptors(Type type, out IEnumerable<ServiceDescriptor> serviceDescriptors)
		{
			ServiceFamily serviceFamily;

			if (_serviceFamilies.TryGetValue(type, out serviceFamily))
			{
				if (serviceFamily.Services.Count != 0)
				{
					serviceDescriptors = serviceFamily.Services;
					return true;
				}
			}

			if (type.IsGenericType && !type.IsGenericTypeDefinition)
			{
				var genericTypeDefinition = type.GetGenericTypeDefinition();
				return TryGetServiceDescriptors(genericTypeDefinition, out serviceDescriptors);
			}

			serviceDescriptors = null;
			return false;
		}

		public void Dispose()
		{
			// todo
		}

		public static bool EqualsDefaultName(string name)
		{
			return string.IsNullOrEmpty(name);
		}
	}
}