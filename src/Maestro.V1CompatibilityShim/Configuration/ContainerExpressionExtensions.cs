using System;

namespace Maestro.Configuration
{
	public static class ContainerExpressionExtensions
	{
		public static IDefaultPluginExpression For(this IContainerExpression expression, Type type)
		{
			return null;
		}

		public static IDefaultPluginExpression<T> For<T>(this IContainerExpression expression)
		{
			return null;
		}

		public static IPluginExpression For(this IContainerExpression expression, Type type, string name)
		{
			return null;
		}

		public static IPluginExpression<T> For<T>(this IContainerExpression expression, string name)
		{
			return null;
		}
	}
}