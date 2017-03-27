using System;
using System.Collections.Generic;

namespace Maestro.Configuration
{
   public interface IContainerExpression
   {
      IDefaultsExpression Defaults { get; }
      IList<ITypeProvider> TypeProviders { get; }

      IServiceExpression For(Type type);
      INamedServiceExpression For(Type type, string name);
      IServiceExpression<T> For<T>();
      INamedServiceExpression<T> For<T>(string name);
      void Scan(Action<IScanner> action);
   }
}