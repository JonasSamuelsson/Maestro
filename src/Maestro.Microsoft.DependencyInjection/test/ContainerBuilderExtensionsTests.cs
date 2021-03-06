﻿using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Linq;
using Xunit;

namespace Maestro.Microsoft.DependencyInjection.Tests
{
   public class ContainerBuilderExtensionsTests
   {
      [Fact]
      public void ShouldAddRequiredServices()
      {
         var container = new Container(x => x.Populate(Enumerable.Empty<ServiceDescriptor>()));

         container.GetService<IServiceProvider>();
         container.GetService<IServiceScopeFactory>();
      }

      [Fact]
      public void ServiceProviderShouldBeScoped()
      {
         var container = new Container(x => x.Populate(Enumerable.Empty<ServiceDescriptor>()));

         var sp1 = container.GetService<IServiceProvider>();
         var sp2 = container.GetService<IServiceProvider>();

         sp1.ShouldBe(sp2);

         var scope = container.CreateScope();

         var sp3 = scope.GetService<IServiceProvider>();
         var sp4 = scope.GetService<IServiceProvider>();

         sp3.ShouldBe(sp4);

         sp1.ShouldNotBe(sp3);
      }

      [Fact]
      public void should_handle_factory_registrations()
      {
         var descriptors = new[]
         {
            new ServiceDescriptor(typeof(string), "success"),
            new ServiceDescriptor(typeof(object), x => x.GetService(typeof(string)), ServiceLifetime.Transient)
         };

         var container = new Container(x => x.Populate(descriptors));

         container.GetService<object>().ShouldBe("success");
      }

      [Fact]
      public void should_handle_instance_registrations()
      {
         var instance = new object();
         var descriptors = new[] { new ServiceDescriptor(typeof(object), instance) };

         var container = new Container(x => x.Populate(descriptors));

         container.GetService<object>().ShouldBe(instance);
      }

      [Fact]
      public void should_handle_type_registrations()
      {
         var descriptors = new[] { new ServiceDescriptor(typeof(object), typeof(TestType), ServiceLifetime.Transient) };

         var container = new Container(x => x.Populate(descriptors));

         container.GetService<object>().GetType().ShouldBe(typeof(TestType));
      }

      [Fact]
      public void should_handle_transient_services()
      {
         var descriptors = new[] { new ServiceDescriptor(typeof(object), typeof(object), ServiceLifetime.Transient) };

         var container = new Container(x => x.Populate(descriptors));

         container.GetService<object>().ShouldNotBe(container.GetService<object>());
      }

      [Fact]
      public void should_handle_scoped_services()
      {
         var descriptors = new[] { new ServiceDescriptor(typeof(object), typeof(object), ServiceLifetime.Scoped) };

         var root = new Container(x => x.Populate(descriptors));

         root.GetService<object>().ShouldBe(root.GetService<object>());

         var scope = root.CreateScope();

         scope.GetService<object>().ShouldBe(scope.GetService<object>());

         root.GetService<object>().ShouldNotBe(scope.GetService<object>());
      }

      [Fact]
      public void should_handle_singleton_services()
      {
         var descriptors = new[] { new ServiceDescriptor(typeof(object), typeof(object), ServiceLifetime.Singleton) };

         var root = new Container(x => x.Populate(descriptors));

         root.GetService<object>().ShouldBe(root.GetService<object>());

         var scope = root.CreateScope();

         scope.GetService<object>().ShouldBe(scope.GetService<object>());

         root.GetService<object>().ShouldBe(scope.GetService<object>());
      }

      [Fact]
      public void should_handle_multiple_registrations_for_the_same_service_type()
      {
         var descriptors = new[]
         {
            new ServiceDescriptor(typeof(object), "one"),
            new ServiceDescriptor(typeof(object), "two"),
            new ServiceDescriptor(typeof(object), "three")
         };

         var container = new Container(x => x.Populate(descriptors));

         container.GetService<object>().ShouldBe("three");
         container.GetServices<object>().ShouldBe(new[] { "one", "two", "three" });
      }

      private class TestType { }
   }
}
