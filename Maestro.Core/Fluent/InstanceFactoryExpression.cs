using System;

namespace Maestro.Fluent
{
	internal class InstanceFactoryExpression<TPlugin> : IInstanceFactoryExpression<TPlugin>
	{
		private readonly Action<IInstanceBuilder> _registerPipeline;
		private readonly DefaultSettings _defaultSettings;

		public InstanceFactoryExpression(Action<IInstanceBuilder> registerPipeline, DefaultSettings defaultSettings)
		{
			_registerPipeline = registerPipeline;
			_defaultSettings = defaultSettings;
		}

		public void Use<TInstance>(TInstance instance) where TInstance : TPlugin
		{
			//ReSharper disable once CompareNonConstrainedGenericWithNull
			if (instance == null) throw new ArgumentNullException("instance");
			var provider = new ConstantInstanceFactory(instance);
			var pipeline = new InstanceBuilder(provider);
			_registerPipeline(pipeline);
		}

		public IInstanceBuilderExpression<TInstance> Use<TInstance>(Func<TInstance> lambda) where TInstance : TPlugin
		{
			if (lambda == null) throw new ArgumentNullException();
			return Use(_ => lambda());
		}

		public IInstanceBuilderExpression<TInstance> Use<TInstance>(Func<IContext, TInstance> lambda) where TInstance : TPlugin
		{
			if (lambda == null) throw new ArgumentNullException();
			var provider = new LambdaInstanceFactory(context => lambda(context));
			var pipeline = new InstanceBuilder(provider);
			pipeline.SetLifetime(_defaultSettings.GetLifetime());
			_registerPipeline(pipeline);
			return new InstanceBuilderExpression<TInstance>(pipeline);
		}

		public IInstanceBuilderExpression<TInstance> Use<TInstance>() where TInstance : TPlugin
		{
			return UseType<TInstance>(typeof(TInstance));
		}

		public IInstanceBuilderExpression<TPlugin> Use(Type type)
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

		public void UseConditional(Action<IConditionalInstanceBuilderExpression<TPlugin>> action)
		{
			if (action == null) throw new ArgumentNullException();
			var builder = new ConditionalInstanceBuilderExpression<TPlugin>(_defaultSettings);
			var pipeline = builder.GetPipeline(action);
			_registerPipeline(pipeline);
		}
	}
}