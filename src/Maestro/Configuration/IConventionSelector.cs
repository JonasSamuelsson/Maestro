using System;

namespace Maestro.Configuration
{
   public interface IConventionSelector
   {
      IScanner ConcreteClassesOf<T>(Action<ConventionalTypeInstanceRegistrator<T>> serviceRegistration);
      IScanner ConcreteClassesOf(Type type, Action<ConventionalTypeInstanceRegistrator<object>> serviceRegistration);
      IScanner ConcreteClassesClosing(Type genericTypeDefinition, Action<ConventionalTypeInstanceRegistrator<object>> serviceRegistration);
      IScanner DefaultImplementations(Action<ConventionalTypeInstanceRegistrator<object>> serviceRegistration);
   }
}