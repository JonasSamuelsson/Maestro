using System;
using System.Collections.Generic;
using Microsoft.Practices.ServiceLocation;

namespace Maestro.CommonServiceLocator
{
	public class MaestroServiceLocator : ServiceLocatorImplBase
	{
		private readonly IContainer _container;

		public MaestroServiceLocator(IContainer container)
		{
			_container = container;
		}

		protected override object DoGetInstance(Type serviceType, string key)
		{
			return _container.GetService(serviceType, key);
		}

		protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
		{
			return _container.GetServices(serviceType);
		}
	}
}