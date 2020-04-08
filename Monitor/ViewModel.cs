using SharedModels;
using SharedModels.ProgressThread;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Monitor
{
    public class ViewModel : INotifyPropertyChanged
    {
        public ViewModel()
        {
            storageHelper = new ServiceBusTopicHelper();
            storageHelper.MessagesCount += StorageHelper_MessageCount;
            storageHelper.IsRunning += StorageHelper_IsRunning;
        }

        private void StorageHelper_IsRunning(bool IsRunning)
        {
            this.IsRunning = IsRunning;
        }

        private void StorageHelper_MessageCount(long messageCount, int subCount)
        {
            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                Messages.Clear();

                for (int i = 0; i < messageCount; i++)
                {
                    Messages.Add(new DemoMessage());
                }
                SubscriptionCount = subCount;

            });
        }


        public ObservableCollection<DemoMessage> Messages { get; }
            = new ObservableCollection<DemoMessage>();

        private int subscriptionCount;

        public int SubscriptionCount
        {
            get { return subscriptionCount; }
            set
            {
                subscriptionCount = value;
                NotifyPropertyChanged();
            }
        }



        private readonly ServiceBusTopicHelper storageHelper;

        public bool IsRunning
        {
            get => isRunning;
            set
            {
                isRunning = value;
                NotifyPropertyChanged("IsRunning");
            }
        }

        private ICommand startCommand;
        public ICommand Start
        {
            get
            {
                return startCommand ?? (startCommand = new RelayCommand(x =>
                {
                    storageHelper.Start();
                }, y => true));
            }
        }

        private ICommand stopCommand;
        private bool isRunning;


        public ICommand Stop
        {
            get
            {
                return stopCommand ?? (stopCommand = new RelayCommand(x =>
                {
                    storageHelper.Stop();
                }, y => true));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }





    }
}
