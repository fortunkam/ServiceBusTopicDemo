using SharedModels.ProgressThread;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Transactions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Receiver
{
    public class ViewModel : INotifyPropertyChanged
    {
        public ViewModel()
        {
            storageHelper = new ServiceBusTopicHelper();
            storageHelper.NewMessage += StorageHelper_NewMessage;
        }

        private void StorageHelper_NewMessage(SharedModels.DemoMessage message)
        {
            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                Name = message.Name;
                Message = message.Message;
                Time = message.Time.ToString("HH:mm:ss.ffff");
            });
        }

        private readonly ServiceBusTopicHelper storageHelper;

        private ICommand startCommand;
        private ICommand cleanupCommand;
        private ICommand setRuleCommand;
        private string name;
        private string message;
        private string time;
        private string rule = "MSName = 'Matt'";
        private bool canStart = true;

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand CleanUp
        {
            get
            {
                return cleanupCommand ?? (cleanupCommand = new RelayCommand(x =>
                {
                    storageHelper.Dispose();
                }));
            }
        }

        public ICommand Start
        {
            get
            {
                return startCommand ?? (startCommand = new RelayCommand(x =>
                {
                    storageHelper.Start();
                    CanStart = false;
                }));
            }
        }

        public ICommand SetRule
        {
            get
            {
                return setRuleCommand ?? (setRuleCommand = new RelayCommand(x =>
                {
                    storageHelper.SetRule(x.ToString());
                }));
            }
        }

        public bool CanStart
        {
            get => canStart; set
            {
                canStart = value;
                NotifyPropertyChanged();
            }
        }

        public string Name
        {
            get => name; set
            {
                name = value;
                NotifyPropertyChanged();
            }
        }
        public string Message
        {
            get => message; set
            {
                message = value;
                NotifyPropertyChanged();
            }
        }
        public string Time { get => time; set
            {
                time = value;
                NotifyPropertyChanged();
            }
        }

        public string Rule
        {
            get => rule; set
            {
                rule = value;
                NotifyPropertyChanged();
            }
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
