using Shouldly;
using Xunit;

namespace Maestro.Tests.Core.Conventions
{
   public class use_default_implementation_convention
   {
      [Fact]
      public void should_register_default_implementations()
      {
         var @namespace = typeof(IFoobar1).Namespace;

         var container = new Container(x => x.Scan(_ =>
         {
            _.AssemblyContainingTypeOf(this);
            _.Matching(y => y.Namespace == @namespace);
            _.For.DefaultImplementations(z => z.Use());
         }));

         Should.NotThrow(() => container.GetService<IFoobar1>());
         Should.NotThrow(() => container.GetService<IFoobar2>());
         Should.Throw<ActivationException>(() => container.GetService<IFoobar3>());
         Should.Throw<ActivationException>(() => container.GetService<IFoobar4>());
      }

      [Fact]
      public void should_support_instance_configuration()
      {
         var container = new Container(x => x.Scan(_ =>
         {
            _.Types(new[] { typeof(IFoobar1), typeof(Foobar1) });
            _.For.DefaultImplementations(z => z.Use().Lifetime.Singleton());
         }));

         var instance1 = container.GetService<IFoobar1>();
         var instance2 = container.GetService<IFoobar1>();

         instance1.ShouldBe(instance2);
      }
   }

   internal interface IFoobar1 { }
   internal interface IFoobar2 { }
   internal interface IFoobar3 { }
   internal interface IFoobar4 { }

   internal class Foobar1 : IFoobar1 { }
   internal class Foobar2 : IFoobar2 { }
   internal class FooBar3 : IFoobar3 { }

   internal class Foobar4 : IFoobar4 { }
}