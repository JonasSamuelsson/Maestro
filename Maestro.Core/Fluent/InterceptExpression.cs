﻿using System;

namespace Maestro.Fluent
{
	internal class InterceptExpression<TParent> : IInterceptExpression<TParent>
	{
		private readonly TParent _parent;
		private readonly Action<IInterceptor> _action;

		public InterceptExpression(TParent parent, Action<IInterceptor> action)
		{
			_parent = parent;
			_action = action;
		}

		public TParent InterceptUsing(IInterceptor interceptor)
		{
			_action(interceptor);
			return _parent;
		}
	}
}