using System;

namespace Maestro.Diagnostics
{
	public interface IDiagnostics
	{
		string WhatDoIHave(Func<Type, bool> predicate = null);
	}
}