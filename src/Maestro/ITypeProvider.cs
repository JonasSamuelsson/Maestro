﻿using System;

namespace Maestro
{
	public interface ITypeProvider
	{
		Type GetInstanceTypeOrNull(Type serviceType, IContext context);
	}
}