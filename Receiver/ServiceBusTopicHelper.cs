using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Azure.ServiceBus.Management;
using SharedModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Receiver
{
    public class ServiceBusTopicHelper : IDisposable
    {
        public ServiceBusTopicHelper()
        {
            var secretClient = new SecretClient(
                new System.Uri(ConfigurationManager.AppSettings["KeyVaultName"]),
                new DefaultAzureCredential()
                );

            connectionString = secretClient.GetSecret("MasterServiceBusConnectionString").Value.Value;
            queueName = secretClient.GetSecret("QueueName").Value.Value;

            managementClient = new ManagementClient(connectionString);
            managementClient.CreateSubscriptionAsync(new SubscriptionDescription(queueName, subscriptionName)
            {
                
            }).Wait();
            subscriptionClient = new SubscriptionClient(connectionString, queueName, subscriptionName, ReceiveMode.ReceiveAndDelete);

        }

        private readonly string subscriptionName = Guid.NewGuid().ToString();

        private readonly SubscriptionClient subscriptionClient;
        private readonly string connectionString;
        private readonly string queueName;
        private readonly ManagementClient managementClient;
        private readonly string ruleName = "myRule";

        public void SetRule(string rule)
        {

            try
            {
                subscriptionClient.RemoveRuleAsync("$Default").Wait();
            }
            catch
            {
            }
            subscriptionClient.AddRuleAsync(new RuleDescription(ruleName, new SqlFilter(rule)));
        }

        public void Start()
        {
            subscriptionClient.RegisterMessageHandler(HandleSubscriptionMessage, new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                AutoComplete = false,
                MaxConcurrentCalls = 1
            });
        }

        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            Console.WriteLine($"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");
            return Task.CompletedTask;
        }

        private async Task HandleSubscriptionMessage(Message message, CancellationToken cancellationToken)
        {
            var demoMessage = JsonSerializer.Deserialize<DemoMessage>(Encoding.UTF8.GetString(message.Body));
            NewMessage?.Invoke(demoMessage);
            await subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
        }

        public delegate void NewMessageHandler(DemoMessage message);
        public event NewMessageHandler NewMessage;


        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                managementClient.DeleteSubscriptionAsync(queueName, subscriptionName).Wait();

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        ~ServiceBusTopicHelper()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
             GC.SuppressFinalize(this);
        }
        #endregion
    }
}
