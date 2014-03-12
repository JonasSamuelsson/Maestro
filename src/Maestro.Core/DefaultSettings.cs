using Maestro.Configuration;
using Maestro.Conventions;
using Maestro.Lifetimes;
using System.Collections.Generic;

namespace Maestro
{
	internal class DefaultSettings : IDefaultSettingsExpression
	{
		private ILifetime _lifetime = TransientLifetime.Instance;
		private readonly IList<IConventionFilter> _filters = new List<IConventionFilter>();

		ILifetimeExpression<IDefaultSettingsExpression> IDefaultSettingsExpression.Lifetime
		{
			get { return new LifetimeExpression<IDefaultSettingsExpression>(this, x => _lifetime = x); }
		}

		IDefaultFilterExpression IDefaultSettingsExpression.Filters
		{
			get { return new DefaultFilterExpression(_filters, this); }
		}

		public ILifetime GetLifetime()
		{
			return _lifetime.Clone();
		}

		public IEnumerable<IConventionFilter> GetFilters()
		{
			return _filters;
		}
	}
}