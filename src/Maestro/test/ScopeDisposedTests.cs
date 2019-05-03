using Shouldly;
using System;
using Xunit;

namespace Maestro.Tests
{
   public class ScopeDisposedTests
   {
      [Fact]
      public void ShouldThrowIfUsingDisposedScope()
      {
         var container = new Container();

         container.Dispose();

         Should.Throw<ObjectDisposedException>(() => container.Configure(delegate { }));
         Should.Throw<ObjectDisposedException>(() => container.CreateScope());
         Should.Throw<ObjectDisposedException>(() => container.GetService(typeof(object)));
         Should.Throw<ObjectDisposedException>(() => container.GetService(typeof(object), ""));
         Should.Throw<ObjectDisposedException>(() => container.GetService<object>());
         Should.Throw<ObjectDisposedException>(() => container.GetService<object>(""));
         Should.Throw<ObjectDisposedException>(() => container.GetServices(typeof(object)));
         Should.Throw<ObjectDisposedException>(() => container.GetServices<object>());
         Should.Throw<ObjectDisposedException>(() => container.TryGetService(typeof(object), out _));
         Should.Throw<ObjectDisposedException>(() => container.TryGetService(typeof(object), "", out _));
         Should.Throw<ObjectDisposedException>(() => container.TryGetService<object>(out _));
         Should.Throw<ObjectDisposedException>(() => container.TryGetService<object>("", out _));
         Should.Throw<ObjectDisposedException>(() => container.ToServiceProvider());
      }
   }
}