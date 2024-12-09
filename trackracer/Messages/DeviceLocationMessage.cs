using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using trackracer.Models;

namespace trackracer.Messages
{
    public class DeviceLocationMessage : ValueChangedMessage<DeviceLocation>
    {
        public DeviceLocationMessage(DeviceLocation deviceLocation) : base(deviceLocation)
        {
        }
    }
}
