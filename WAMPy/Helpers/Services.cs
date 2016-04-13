using System;
using System.ServiceProcess;

namespace WDS
{
    class Services
    {
        private ServiceController Service = null;

        public String DoesServiceExist(String serviceName, bool IsFullName = false)
        {
            ServiceController[] Services = ServiceController.GetServices();
            serviceName = serviceName.ToLower();

            // Try to find service name.
            foreach (ServiceController srvc in Services)
            {
                if (IsFullName && srvc.ServiceName == serviceName)
                {
                    this.Service = srvc;
                    return srvc.ServiceName;
                }
                else if (srvc.ServiceName.ToLower().Contains(serviceName))
                {
                    this.Service = srvc;
                    return srvc.ServiceName;
                }
            }

            return null;
        }

        public bool StopService()
        {
            return this.StopService(this.Service);
        }

        public bool StopService(ServiceController svc)
        {
            if (
                svc.Status == ServiceControllerStatus.Running ||
                svc.Status == ServiceControllerStatus.Paused ||
                svc.Status == ServiceControllerStatus.ContinuePending ||
                svc.Status == ServiceControllerStatus.StartPending
            )
            {
                svc.Stop();
                return true;
            }

            return false;
        }

        public bool StopService(String serviceName)
        {
            ServiceController svc = new ServiceController(serviceName);

            return this.StopService(svc);
        }
    }
}
