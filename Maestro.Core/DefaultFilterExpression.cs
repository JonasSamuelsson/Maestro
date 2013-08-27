using System;
using System.Collections.Generic;
using Maestro.Conventions;
using Maestro.Fluent;

namespace Maestro
{
	internal class DefaultFilterExpression : IDefaultFilterExpression
	{
		private readonly IList<IConventionFilter> _filters;
		private readonly IDefaultSettingsExpression _parent;

		public DefaultFilterExpression(IList<IConventionFilter> filters, IDefaultSettingsExpression parent)
		{
			_filters = filters;
			_parent = parent;
		}

		public IDefaultSettingsExpression Add(Func<Type, bool> predicate)
		{
			return Add(new LambdaFilter(predicate));
		}

		public IDefaultSettingsExpression Add(IConventionFilter filter)
		{
			_filters.Add(filter);
			return _parent;
		}

		public IDefaultSettingsExpression Clear()
		{
			_filters.Clear();
			return _parent;
		}
	}
}