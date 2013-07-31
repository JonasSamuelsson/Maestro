using System;

namespace Maestro
{
	internal class TypeProvider : IProvider
	{
		private readonly Type _type;

		public TypeProvider(Type type)
		{
			_type = type;
		}

		public object Get()
		{
			return Activator.CreateInstance(_type);
		}
	}
}