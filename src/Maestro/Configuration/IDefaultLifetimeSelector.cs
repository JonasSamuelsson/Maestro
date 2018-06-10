namespace Maestro.Configuration
{
	public interface IDefaultLifetimeSelector
	{
		void Transient();
		void Scoped();
		void Singleton();
	}
}