using Serilog;

namespace Codexcite.DurableFunctions.Utils
{
	public class BaseLogged
	{
		protected ILogger _log;
		
		public BaseLogged()
		{
			_log = Log.Logger.ForContext(this.GetType());
			
		}
	}
}