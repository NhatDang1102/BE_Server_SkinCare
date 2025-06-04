using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contract.DTOs
{
    public class ChatHistoryItem
    {
        public string ChatId { get; set; }
        public string Prompt { get; set; }
        public string Response { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
