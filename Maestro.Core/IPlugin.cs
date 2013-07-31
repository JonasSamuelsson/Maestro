namespace Maestro
{
	internal interface IPlugin
	{
		void Add(string name, IPipeline pipeline);
		IPipeline Get(string name);
		bool TryGet(string name, out IPipeline pipeline);
	}
}