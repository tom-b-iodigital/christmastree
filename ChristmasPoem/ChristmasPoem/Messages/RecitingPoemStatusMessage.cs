using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChristmasPoem.Messages
{
    public enum PoemStatus
    {
        NoPoem,
        FetchingPoem,
        RecitingPoem,
        DisplayingPoem
    }

    public class RecitingPoemStatusMessage : ValueChangedMessage<PoemStatus>
    {
        public RecitingPoemStatusMessage(PoemStatus value) : base(value)
        {
        }
    }
}
