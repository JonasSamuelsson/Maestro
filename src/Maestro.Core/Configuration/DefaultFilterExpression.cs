using System;
using System.Collections.Generic;
using Maestro.Conventions;

namespace Maestro.Configuration
{
	internal class DefaultFilterExpression : IDefaultFilterExpression
	{
		private readonly IList<IFilter> _filters;
		private readonly IDefaultSettingsExpression _parent;

		public DefaultFilterExpression(IList<IFilter> filters, IDefaultSettingsExpression parent)
		{
			_filters = filters;
			_parent = parent;
		}

		public IDefaultSettingsExpression Add(Func<Type, bool> predicate)
		{
			return Add(new LambdaFilter(predicate));
		}

		public IDefaultSettingsExpression Add(IFilter filter)
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