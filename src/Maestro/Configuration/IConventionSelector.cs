using System;

namespace Maestro.Configuration
{
   public interface IConventionSelector
   {
      IScanner ConcreteClassesOf<T>(Action<IConventionalServiceTypeSelector<T>> serviceRegistration);
      IScanner ConcreteClassesOf(Type type, Action<IConventionalServiceTypeSelector<object>> serviceRegistration);
      IScanner ConcreteClassesClosing(Type genericTypeDefinition, Action<IConventionalServiceTypeSelector<object>> serviceRegistration);
      IScanner DefaultImplementations(Action<IConventionalServiceTypeSelector<object>> serviceRegistration);
   }
}