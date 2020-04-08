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
    public class ServiceBusTopicHelper
    {
        public ServiceBusTopicHelper()
        {
            var secretClient = new SecretClient(
                new System.Uri(ConfigurationManager.AppSettings["KeyVaultName"]),
                new DefaultAzureCredential()
                );

            topic = new TopicClient(secretClient.GetSecret("SenderServiceBusConnectionString").Value.Value,
                secretClient.GetSecret("QueueName").Value.Value);
        }

        private readonly ITopicClient topic;

        public void SendMessage(DemoMessage message)
        {
            var serializedMesage = JsonSerializer.Serialize(message);
            var cloudQueueMessage = new Message(Encoding.UTF8.GetBytes(serializedMesage));
            cloudQueueMessage.UserProperties.Add("MSName", message.Name);
            topic.SendAsync(cloudQueueMessage);
        }
    }
}
