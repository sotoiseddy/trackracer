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
        //public Guid? ReceiverID { get; set; }
        public string Status { get; set; }
        public string SenderName { get; set; } = "";
        //public string ReceiverName { get; set; } = "";
        public string Text { get; set; }
        public int RequestType { get; set; }
        public string Location { get; set; }
        public int Pay { get; set; }
    }
}
