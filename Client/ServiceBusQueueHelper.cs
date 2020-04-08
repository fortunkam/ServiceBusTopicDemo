using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using SharedModels;
using Microsoft.Azure.ServiceBus;

namespace Client
{
    public class ServiceBusQueueHelper
    {
        public ServiceBusQueueHelper()
        {
            var secretClient = new SecretClient(
                new System.Uri(ConfigurationManager.AppSettings["KeyVaultName"]),
                new DefaultAzureCredential()
                );

            _queue = new QueueClient(secretClient.GetSecret("SenderServiceBusConnectionString").Value.Value,
                secretClient.GetSecret("QueueName").Value.Value);
        }

        private readonly IQueueClient _queue;

        public void SendMessage(DemoMessage message)
        {
            var serializedMesage = JsonSerializer.Serialize(message);
            var cloudQueueMessage = new Message(Encoding.UTF8.GetBytes(serializedMesage));
            _queue.SendAsync(cloudQueueMessage);
        }
    }
}
