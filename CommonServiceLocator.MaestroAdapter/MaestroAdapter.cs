using System;
using System.Collections.Generic;
using Maestro;
using Microsoft.Practices.ServiceLocation;

namespace CommonServiceLocator.MaestroAdapter
{
	public class MaestroAdapter : ServiceLocatorImplBase
	{
		private readonly IContainer _container;

		public MaestroAdapter(IContainer container)
		{
			_container = container;
		}

		protected override object DoGetInstance(Type serviceType, string key)
		{
			return _container.Get(serviceType, key);
		}

		protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
		{
			return _container.GetAll(serviceType);
		}
	}
}