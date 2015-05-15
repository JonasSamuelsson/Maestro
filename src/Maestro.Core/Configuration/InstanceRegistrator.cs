using Maestro.Factories;
using System;

namespace Maestro.Configuration
{
	internal class InstanceRegistrator<TPlugin>
	{
		private readonly Action<IInstanceBuilder> _registerPipeline;
		private readonly DefaultSettings _defaultSettings;

		public InstanceRegistrator(Action<IInstanceBuilder> registerPipeline, DefaultSettings defaultSettings)
		{
			_registerPipeline = registerPipeline;
			_defaultSettings = defaultSettings;
		}

		public void Register<TInstance>(TInstance instance)
		{
			//ReSharper disable once CompareNonConstrainedGenericWithNull
			if (instance == null) throw new ArgumentNullException("instance");
			var provider = new ConstantInstanceFactory(instance);
			var pipeline = new InstanceBuilder(provider);
			_registerPipeline(pipeline);
		}

		public IInstanceBuilderExpression<TInstance> Register<TInstance>(Func<TInstance> lambda) where TInstance : TPlugin
		{
			if (lambda == null) throw new ArgumentNullException();
			return Register((IContext _) => lambda());
		}

		public IInstanceBuilderExpression<TInstance> Register<TInstance>(Func<IContext, TInstance> lambda) where TInstance : TPlugin
		{
			if (lambda == null) throw new ArgumentNullException();
			var provider = new LambdaInstanceFactory(context => lambda(context));
			var pipeline = new InstanceBuilder(provider);
			pipeline.SetLifetime(_defaultSettings.GetLifetime());
			_registerPipeline(pipeline);
			return new InstanceBuilderExpression<TInstance>(pipeline);
		}

		public IInstanceBuilderExpression<TInstance> Register<TInstance>() where TInstance : TPlugin
		{
			return UseType<TInstance>(typeof(TInstance));
		}

		public IInstanceBuilderExpression<TPlugin> Register(Type type)
		{
			if (type == null) throw new ArgumentNullException();
			return UseType<TPlugin>(type);
		}

		private IInstanceBuilderExpression<T> UseType<T>(Type type)
		{
			var provider = new TypeInstanceFactory(type);
			var pipeline = new InstanceBuilder(provider);
			pipeline.SetLifetime(_defaultSettings.GetLifetime());
			_registerPipeline(pipeline);
			return new InstanceBuilderExpression<T>(pipeline);
		}
	}
}