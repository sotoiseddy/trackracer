using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace trackracer.Models
{
    public  class StatusDetails
    {
        public enum TrackingStatus
        {
            Pending = 1,
            Approved,
            Rejected,
            Canceled
        }
    }
}
