using System;
using Maestro.Conventions;

namespace Maestro.Configuration
{
   internal class ConventionSelectorExpression : IConventionSelectorExpression
   {
      private readonly ConventionExpression _conventionExpression;
      private readonly ServiceRegistration _registration;

      public ConventionSelectorExpression(ConventionExpression conventionExpression, ServiceRegistration registration)
      {
         _conventionExpression = conventionExpression;
         _registration = registration;
      }

      public IConventionExpression ConcreteSubClassesOf<T>(Action<ITypeInstanceExpression<T>> instanceConfiguration = null)
      {
         var convention = new ConcreteSubClassesConvention<T>(typeof(T), _registration, instanceConfiguration);
         return _conventionExpression.With(convention);
      }

      public IConventionExpression ConcreteSubClassesOf(Type type, Action<ITypeInstanceExpression<object>> instanceConfiguration = null)
      {
         var convention = new ConcreteSubClassesConvention<object>(type, _registration, instanceConfiguration);
         return _conventionExpression.With(convention);
      }

      public IConventionExpression ConcreteClassesClosing(Type genericTypeDefinition, Action<ITypeInstanceExpression<object>> instanceConfiguration = null)
      {
         var convention = new ConcreteClassesClosingConvention(genericTypeDefinition, _registration, instanceConfiguration);
         return _conventionExpression.With(convention);
      }

      public IConventionExpression DefaultImplementations(Action<ITypeInstanceExpression<object>> instanceConfiguration = null)
      {
         var convention = new DefaultImplementationsConvention(_registration, instanceConfiguration);
         return _conventionExpression.With(convention);
      }
   }
}