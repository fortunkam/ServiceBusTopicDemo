using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using SharedModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Text.Json;

namespace Receiver
{
    public class ServiceBusQueueHelper
    {
        public ServiceBusQueueHelper()
        {
            var secretClient = new SecretClient(
                new System.Uri(ConfigurationManager.AppSettings["KeyVaultName"]),
                new DefaultAzureCredential()
                );

            receiver = new MessageReceiver(secretClient.GetSecret("ReceiverServiceBusConnectionString").Value.Value,
                            secretClient.GetSecret("QueueName").Value.Value,ReceiveMode.ReceiveAndDelete);
        }

        private readonly IMessageReceiver receiver;

        public DemoMessage GetMessage()
        {
            receiver.OperationTimeout = new TimeSpan(0, 0, 1);
            try
            {
                var m = receiver.ReceiveAsync().Result;
                var demoMessage = JsonSerializer.Deserialize<DemoMessage>(Encoding.UTF8.GetString(m.Body));
                return demoMessage;
            }
            catch (Exception)
            {
                return null;
            }
            

        }
    }
}
