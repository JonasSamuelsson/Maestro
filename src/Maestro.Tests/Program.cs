using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Maestro.Tests.TypeFactoryResolvers;

namespace Maestro.Tests
{
	static class Program
	{
		static void Main()
		{
			new FuncFactoryResolver_tests().should_get_unregistered_closed_func_of_resolvable_type();
		}
	}
}
