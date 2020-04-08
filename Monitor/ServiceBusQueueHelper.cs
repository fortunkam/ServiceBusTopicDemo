using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using SharedModels;
using System.ComponentModel;
using System.Threading;
using System.Linq;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Azure.ServiceBus.Management;

namespace Monitor
{
    public class ServiceBusQueueHelper
    {
        public ServiceBusQueueHelper()
        {
            var secretClient = new SecretClient(
                new System.Uri(ConfigurationManager.AppSettings["KeyVaultName"]),
                new DefaultAzureCredential()
                );

            connectionString = secretClient.GetSecret("MasterServiceBusConnectionString").Value.Value;
            queueName = secretClient.GetSecret("QueueName").Value.Value;

            managementClient = new ManagementClient(connectionString);

            worker = new BackgroundWorker();
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += Worker_DoWork;
        }

        private readonly string connectionString;
        private readonly string queueName;
        private ManagementClient managementClient;
        private readonly BackgroundWorker worker;

        public void Start()
        {
            if (!worker.IsBusy)
            {

                worker.RunWorkerAsync();
                IsRunning?.Invoke(true);
            }
        }

        public delegate void MessageCountHandler(long messageCount, long deadletterMessageCount);
        public event MessageCountHandler MessagesCount;

        public delegate void IsRunningHandler(bool IsRunning);
        public event IsRunningHandler IsRunning;

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                if (worker.CancellationPending)
                {
                    return;
                }

                var info = managementClient.GetQueueRuntimeInfoAsync(queueName).Result;

                MessagesCount?.Invoke(info.MessageCountDetails.ActiveMessageCount, 
                    info.MessageCountDetails.DeadLetterMessageCount);

                Thread.Sleep(1000);
            }

        }

        public void Stop()
        {
            if (worker.IsBusy)
            {
                worker.CancelAsync();
                IsRunning?.Invoke(false);
            }
        }

    }
}
