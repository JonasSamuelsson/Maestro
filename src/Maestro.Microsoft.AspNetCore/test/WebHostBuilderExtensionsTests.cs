using Maestro.Microsoft.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Collections.Generic;
using Xunit;

namespace Maestro.Microsoft.AspNetCore.Tests
{
   public class WebHostBuilderExtensionsTests
   {
      [Fact]
      public void ShouldAddMaestro()
      {
         var builder = new WebHostBuilder();

         builder.UseMaestro();

         builder.Services.ShouldContain(x => x.ImplementationInstance.GetType() == typeof(MaestroServiceProviderFactory));
      }

      private class ServiceCollection : List<ServiceDescriptor>, IServiceCollection { }

      private class WebHostBuilder : IWebHostBuilder
      {
         public IServiceCollection Services { get; } = new ServiceCollection();

         public IWebHost Build()
         {
            throw new NotImplementedException();
         }

         public IWebHostBuilder ConfigureAppConfiguration(Action<WebHostBuilderContext, IConfigurationBuilder> configureDelegate)
         {
            throw new NotImplementedException();
         }

         public IWebHostBuilder ConfigureServices(Action<IServiceCollection> configureServices)
         {
            configureServices(Services);
            return this;
         }

         public IWebHostBuilder ConfigureServices(Action<WebHostBuilderContext, IServiceCollection> configureServices)
         {
            throw new NotImplementedException();
         }

         public string GetSetting(string key)
         {
            throw new NotImplementedException();
         }

         public IWebHostBuilder UseSetting(string key, string value)
         {
            throw new NotImplementedException();
         }
      }
   }
}