using System;

namespace Maestro.Configuration
{
	internal class ConventionalServiceTypeExpression<T> : IConventionalServiceTypeExpression<T>
	{
		public ConventionalServiceTypeExpression(IContainerExpression containerExpression, Type baseType, Type type)
		{
			BaseType = new ConventionalServiceExpression<T>(containerExpression, baseType, type);
			Type = new ConventionalServiceExpression<T>(containerExpression, type, type);
		}

		public IConventionalServiceExpression<T> BaseType { get; }
		public IConventionalServiceExpression<T> Type { get; }
	}
}