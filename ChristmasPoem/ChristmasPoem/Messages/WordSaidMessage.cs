using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChristmasPoem.Messages
{
    public class WordSaidMessage : ValueChangedMessage<string>
    {
        public WordSaidMessage(string value) : base(value)
        {
        }
    }
}
