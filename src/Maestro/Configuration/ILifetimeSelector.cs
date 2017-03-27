using Maestro.Lifetimes;

namespace Maestro.Configuration
{
   public interface ILifetimeSelector<TParent>
   {
      TParent Transient();
      TParent Scoped();
      TParent Singleton();
      TParent Use<TLifetime>() where TLifetime : ILifetime, new();
      TParent Use(ILifetime lifetime);
   }
}