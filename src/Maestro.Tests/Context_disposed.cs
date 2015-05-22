using System;
using Shouldly;
using Xunit;

namespace Maestro.Tests
{
   public class Context_disposed
   {
      [Fact]
      public void Using_disposed_context_should_throw()
      {
         var container = new Container(x => x.For<Factory>().Use.Factory(ctx => new Factory(ctx.Get)));
         var factory = container.Get<Factory>();
         Should.Throw<ObjectDisposedException>(() => factory.Create(typeof(object)));
      }

      class Factory
      {
         private readonly Func<Type, object> _factory;

         public Factory(Func<Type, object> factory)
         {
            _factory = factory;
         }

         public object Create(Type type)
         {
            return _factory(type);
         }
      }
   }
}