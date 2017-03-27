using System;

namespace Maestro.Configuration
{
   public interface IConventionSelector
   {
      IScanner ConcreteSubClassesOf<T>(Action<ConventionalTypeInstanceRegistrator<T>> serviceRegistration);
      IScanner ConcreteSubClassesOf(Type type, Action<ConventionalTypeInstanceRegistrator<object>> serviceRegistration);
      IScanner ConcreteClassesClosing(Type genericTypeDefinition, Action<ConventionalTypeInstanceRegistrator<object>> serviceRegistration);
      IScanner DefaultImplementations(Action<ConventionalTypeInstanceRegistrator<object>> serviceRegistration);
   }
}