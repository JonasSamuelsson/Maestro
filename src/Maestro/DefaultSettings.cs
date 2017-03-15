using System;
using System.Collections.Generic;
using Maestro.Configuration;
using Maestro.Lifetimes;

namespace Maestro
{
	internal class DefaultSettings : IDefaultSettingsExpression
	{
		private Func<ILifetime> _lifetimeFactory = () => TransientLifetime.Instance;
		private readonly IList<IFilter> _filters = new List<IFilter>();

		LifetimeSelector<IDefaultSettingsExpression> IDefaultSettingsExpression.Lifetime
		{
			get { return new LifetimeSelector<IDefaultSettingsExpression>(this, lifetimeFactory => _lifetimeFactory = lifetimeFactory); }
		}

		IDefaultFilterExpression IDefaultSettingsExpression.Filters
		{
			get { return new DefaultFilterExpression(_filters, this); }
		}

		public ILifetime GetLifetime()
		{
			return _lifetimeFactory();
		}

		public IEnumerable<IFilter> GetFilters()
		{
			return _filters;
		}
	}
}