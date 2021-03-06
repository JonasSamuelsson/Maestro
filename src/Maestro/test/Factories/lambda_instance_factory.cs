﻿using Shouldly;
using Xunit;

namespace Maestro.Tests.Factories
{
   public class lambda_instance_factory
   {
      [Fact]
      public void should_delegate_instantiation_to_provided_lambda()
      {
         var o = new object();

         var container = new Container(x => x.Add<object>().Factory(() => o));
         var instance = container.GetService<object>();

         instance.ShouldBe(o);
      }

      [Fact]
      public void should_be_able_to_retrieve_dependencies()
      {
         var o = new object();
         var container = new Container(x =>
         {
            x.Add<object>().Instance(o);
            x.Add<ClassWithDependency>().Factory(ctx => new ClassWithDependency { Dependency = ctx.GetService<object>() });
         });

         var instance = container.GetService<ClassWithDependency>();

         instance.Dependency.ShouldBe(o);
      }

      private class ClassWithDependency
      {
         public object Dependency { get; set; }
      }
   }
}