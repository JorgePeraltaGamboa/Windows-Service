using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WindowsService
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }
        public class service
        {
            public string name { get; set; }
            public string time { get; set; }
        }
        protected override void OnStart(string[] args)
        {
            string obj= System.IO.File.ReadAllText("C:\\Program Files (x86)\\Sistemas AGS\\ContpaqStarterV2\\Data\\ServiceList.json");
            JObject serviceList = JObject.Parse(obj);
            IList<JToken> results = serviceList["ServiceList"]["List"].Children().ToList();
            IList<service> searchResults = new List<service>();
            foreach (JToken result in results)
            {
                service aux_list = JsonConvert.DeserializeObject<service>(result.ToString());
                searchResults.Add(aux_list);
            }
            foreach (service servicio in searchResults)
            {
                //Instancia de ServiceController, permite controlar el estado y las acciones relacionadas a los estados
                ServiceController sc = new ServiceController();
                sc.ServiceName = servicio.name; //Nombre de nuestro servicio
                                               //Console.WriteLine("The xbgm service status is currently set to {0}", sc.Status.ToString()); //Checando Status
                if (sc.Status == ServiceControllerStatus.Stopped)
                {
                    // Iniciar el servicio si está detenido
                    //Console.WriteLine("Starting the", sc.ServiceName," service...");
                    try
                    {
                        // Se inicia el servicio y espera hasta que su estado sea "Running" o En Ejecucion
                        sc.Start();
                        sc.WaitForStatus(ServiceControllerStatus.Running);
                        // Comprobamos el estado
                        //Console.WriteLine("The Alerter service status is now set to {0}.", sc.Status.ToString());
                    }
                    catch (InvalidOperationException)
                    {
                        Console.WriteLine("Could not start the service.");
                    }
                }
            }
           
        }

        protected override void OnStop()
        {
            //Do something
        }
    }
}
