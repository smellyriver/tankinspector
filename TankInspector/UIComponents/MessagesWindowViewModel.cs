using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Smellyriver.TankInspector.UIComponents
{
	internal class MessagesWindowViewModel : NotificationObject
    {

        public ObservableCollection<MessageViewModel> Messages { get; }

        public MessagesWindowViewModel()
        {
            this.Messages = new ObservableCollection<MessageViewModel>();
        }

        public void AddMessages(IEnumerable<string> messages)
        {
            foreach (var message in messages)
                this.Messages.Add(new MessageViewModel(message, DateTime.Now));
        }

    }
}
