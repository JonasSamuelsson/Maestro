using System;
using Maestro.Configuration;
using Maestro.Lifetimes;

namespace Maestro
{
	internal class DefaultLifetimeExpression<T> : ILifetimeExpression<IDefaultSettingsExpression>
	{
		private readonly DefaultSettings _defaultSettings;
		private readonly Action<Func<ILifetime>> _registerLifetimeFactory;

		public DefaultLifetimeExpression(DefaultSettings defaultSettings, Action<Func<ILifetime>> registerLifetimeFactory)
		{
			_defaultSettings = defaultSettings;
			_registerLifetimeFactory = registerLifetimeFactory;
		}

		public IDefaultSettingsExpression Transient()
		{
			_registerLifetimeFactory(() => TransientLifetime.Instance);
			return _defaultSettings;
		}

		public IDefaultSettingsExpression Context()
		{
			return Use<ContextSingletonLifetime>();
		}

		public IDefaultSettingsExpression Singleton()
		{
			return Use<SingletonLifetime>();
		}

		public IDefaultSettingsExpression Use<TLifetime>() where TLifetime : ILifetime, new()
		{
			_registerLifetimeFactory(() => new TLifetime());
			return _defaultSettings;
		}

		public IDefaultSettingsExpression Use(ILifetime lifetime)
		{
			_registerLifetimeFactory(() => lifetime);
			return _defaultSettings;
		}
	}
}