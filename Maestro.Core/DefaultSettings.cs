using System.Collections.Generic;
using Maestro.Conventions;
using Maestro.Fluent;
using Maestro.Lifecycles;

namespace Maestro
{
	internal class DefaultSettings : IDefaultSettingsExpression
	{
		private ILifecycle _lifecycle = TransientLifecycle.Instance;
		private readonly IList<IConventionFilter> _filters = new List<IConventionFilter>();

		ILifecycleSelector<IDefaultSettingsExpression> IDefaultSettingsExpression.Lifecycle
		{
			get { return new LifecycleSelector<IDefaultSettingsExpression>(this, x => _lifecycle = x); }
		}

		IDefaultFilterExpression IDefaultSettingsExpression.Filters
		{
			get { return new DefaultFilterExpression(_filters, this); }
		}

		public ILifecycle GetLifecycle()
		{
			return _lifecycle.Clone();
		}

		public IEnumerable<IConventionFilter> GetFilters()
		{
			return _filters;
		}
	}
}