using System;
using Maestro.Conventions;

namespace Maestro.Configuration
{
	public interface IDefaultFilterExpression
	{
		IDefaultSettingsExpression Add(Func<Type, bool> predicate);
		IDefaultSettingsExpression Add(IConventionFilter filter);
		IDefaultSettingsExpression Clear();
	}
}