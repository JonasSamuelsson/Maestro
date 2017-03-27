using System;
using Maestro.Lifetimes;

namespace Maestro.Configuration
{
   public interface IDefaultLifetimeSelector
   {
      void Transient();
      void Scoped();
      void Singleton();
      void Use<TLifetime>() where TLifetime : ILifetime, new();
      void Use(Func<ILifetime> factory);
   }
}