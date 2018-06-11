using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace ContpaqStart
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }

        public void onDebug() {
            OnStart(null);
        }

        public class service
        {
            public string name { get; set; }
            public string seconds { get; set; }
        }

        bool DoesServiceExist(string serviceName)
        {
            ServiceController[] services = ServiceController.GetServices();
            var service = services.FirstOrDefault(s => s.ServiceName == serviceName);
            return service != null;
        }
        public static string StartupPath { get; }

        protected override void OnStart(string[] args)
        {
            string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string newpath = (path + "\\Data\\ServiceList.json");
            this.EventLog.WriteEntry(newpath);
            string obj = System.IO.File.ReadAllText(newpath);

            JArray aList = JArray.Parse(obj);

            foreach (JToken result in aList) {
                service servicio = JsonConvert.DeserializeObject<service>(result.ToString());

                if (DoesServiceExist(servicio.name))
                {

                    ServiceController sc = new ServiceController();
                    sc.ServiceName = servicio.name;

                    if (sc.Status == ServiceControllerStatus.Stopped)
                    {
                        
                        try
                        {
                            // Se inicia el servicio y espera hasta que su estado sea "Running" o En Ejecucion
                            sc.Start();
                            sc.WaitForStatus(ServiceControllerStatus.Running);
                            this.EventLog.WriteEntry("Service:" + sc.ServiceName + " is running");
                            // Comprobamos el estado
                        }
                        catch (InvalidOperationException)
                        {
                            Console.WriteLine("Could not start the service.");
                            this.EventLog.WriteEntry("Service:" + sc.ServiceName + " can't start");
                        }
                    }
                }
            }
        }

        protected override void OnStop()
        {
        }
    }
}
