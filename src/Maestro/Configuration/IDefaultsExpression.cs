namespace Maestro.Configuration
{
   public interface IDefaultsExpression
   {
      IDefaultLifetimeSelector Lifetime { get; }
   }
}