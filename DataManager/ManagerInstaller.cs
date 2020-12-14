using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace DataManager
{
    [RunInstaller(true)]
    public partial class ManagerInstaller : Installer
    {
        ServiceInstaller serviceInstaller;
        ServiceProcessInstaller processInstaller;

        public ManagerInstaller()
        {
            InitializeComponent();
            serviceInstaller = new ServiceInstaller();
            processInstaller = new ServiceProcessInstaller();

            processInstaller.Account = ServiceAccount.LocalSystem;
            serviceInstaller.StartType = ServiceStartMode.Manual;
            serviceInstaller.ServiceName = "DataManager";
            Installers.Add(processInstaller);
            Installers.Add(serviceInstaller);
        }
    }
}
