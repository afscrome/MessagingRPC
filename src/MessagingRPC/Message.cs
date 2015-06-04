using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessagingRPC
{
    public class Message
    {
        public string CorrelationId { get; set; }
        public string Value { get; set; }
    }
}
