using System;

namespace Maestro.Internals
{
	interface IPlugin
	{
		Func<object> Factory { get; set; }
	}
}