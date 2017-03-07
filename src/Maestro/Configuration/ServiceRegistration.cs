using System;

namespace Maestro.Configuration
{
   internal delegate ITypeInstanceExpression<object> ServiceRegistration(Type serviceType, Type instanceType);
}