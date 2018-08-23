using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace Maestro.Microsoft.DependencyInjection.Tests
{
	internal class ServiceCollection : List<ServiceDescriptor>, IServiceCollection { }
}