using System;
using System.Collections.Generic;
using System.ServiceProcess;

namespace WDS
{
    class Services
    {
        /// <summary>
        /// Names of the running Windows services whose name contains the given text.
        /// These services are never stopped by this program: they belong to other
        /// installations and stopping them needs administrator rights.
        /// </summary>
        public List<String> RunningServices(String partialName)
        {
            List<String> found = new List<String>();
            ServiceController[] services;
            try
            {
                services = ServiceController.GetServices();
            }
            catch (Exception ex)
            {
                Globals.AddToLogFile("Unable to list the Windows services: " + ex);
                return found;
            }

            foreach (ServiceController srvc in services)
            {
                try
                {
                    if (srvc.ServiceName.IndexOf(partialName, StringComparison.OrdinalIgnoreCase) >= 0 &&
                        srvc.Status != ServiceControllerStatus.Stopped &&
                        srvc.Status != ServiceControllerStatus.StopPending)
                    {
                        found.Add(srvc.ServiceName);
                    }
                }
                catch (Exception)
                {
                }
                finally
                {
                    srvc.Dispose();
                }
            }
            return found;
        }
    }
}
