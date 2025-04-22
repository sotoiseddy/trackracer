using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace trackracer.Models
{
    public class ChatModel
    {
        public int Id { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public string ChatMessage { get; set; }
        public string ChatId { get; set; }
        public string SenderName { get; set; }
        public string ReceiverName { get; set; }

    }
}
