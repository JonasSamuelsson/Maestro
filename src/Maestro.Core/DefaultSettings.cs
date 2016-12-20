using System;
using System.Collections.Generic;
using Maestro.Configuration;
using Maestro.Conventions;
using Maestro.Lifetimes;

namespace Maestro
{
	internal class DefaultSettings : IDefaultSettingsExpression
	{
		private Func<ILifetime> _lifetimeFactory = () => TransientLifetime.Instance;
		private readonly IList<IFilter> _filters = new List<IFilter>();

		ILifetimeExpression<IDefaultSettingsExpression> IDefaultSettingsExpression.Lifetime
		{
			get { return new DefaultLifetimeExpression<IDefaultSettingsExpression>(this, x => _lifetimeFactory = x); }
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