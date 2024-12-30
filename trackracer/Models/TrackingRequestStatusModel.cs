using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace trackracer.Models
{
    public class TrackingRequestStatusModel
    {
        public int ID { get; set; }
        public Guid? SenderID { get; set; }
        public Guid? ReceiverID { get; set; }
        public string Status { get; set; }


    }
}
