using System;
using System.Linq;
using System.Reflection;
using Maestro.FactoryProviders;
using Maestro.Internals;

namespace Maestro.TypeFactoryResolvers
{
	internal class ConcreteClosedClassTypeFactoryResolver : ITypeFactoryResolver
	{
		public bool TryGet(Type type, Context context, out Pipeline pipeline)
		{
			pipeline = null;
			if (!type.IsConcreteClosedClass()) return false;
			var pipelines = from ctor in type.GetConstructors(BindingFlags.Instance | BindingFlags.Public)
								 let parameters = ctor.GetParameters()
								 orderby parameters.Length descending
								 where parameters.All(p => context.Kernel.CanGetDependency(p.ParameterType, context))
								 let factoryProvider = new TypeFactoryProvider(type) { Constructor = ctor }
								 let plugin = new Plugin
								 {
									 FactoryProvider = factoryProvider
								 }
								 select new Pipeline(plugin);
			pipeline = pipelines.FirstOrDefault();
			return pipeline != null;
		}
	}
}