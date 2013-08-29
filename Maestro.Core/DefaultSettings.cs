using System.Collections.Generic;
using Maestro.Conventions;
using Maestro.Fluent;
using Maestro.Lifetimes;

namespace Maestro
{
	internal class DefaultSettings : IDefaultSettingsExpression
	{
		private ILifetime _lifetime = TransientLifetime.Instance;
		private readonly IList<IConventionFilter> _filters = new List<IConventionFilter>();

		ILifetimeSelector<IDefaultSettingsExpression> IDefaultSettingsExpression.Lifetime
		{
			get { return new LifetimeSelector<IDefaultSettingsExpression>(this, x => _lifetime = x); }
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