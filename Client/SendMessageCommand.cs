using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Client
{
    public class SendMessageCommand : ICommand
    {

        private readonly ServiceBusTopicHelper _storageHelper = new ServiceBusTopicHelper();

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var model = parameter as ViewModel;
            if (model == null) throw new ArgumentNullException("Model of wrong type");
            _storageHelper.SendMessage(new SharedModels.DemoMessage
            {
                Name = model.Name,
                Message = model.Message,
                Time = DateTime.UtcNow
            });

        }
    }
}
