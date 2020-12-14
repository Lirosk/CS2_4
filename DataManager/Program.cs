using System.ServiceProcess;

namespace DataManager
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main()
		{
			try
			{
				ServiceBase[] ServicesToRun;
				ServicesToRun = new ServiceBase[]
				{
				new Manager()
				};
				ServiceBase.Run(ServicesToRun);
			}
			catch (System.Exception ex)
			{
				using (System.IO.StreamWriter writer = new System.IO.StreamWriter(System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "log.txt"), true))
				{
					writer.WriteLine("\nException!\n" + System.DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss ") + ex.Message + "\n\n");
					writer.Flush();
				}
			}
		}
	}
}
