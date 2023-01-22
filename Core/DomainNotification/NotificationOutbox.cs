﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainNotification
{
    public class NotificationOutbox
    {
        public NotificationOutbox()
        {

        }
        public DateTime OccuredOn { get; set; }
        public DateTime? ProcessedDate { get; set; }
        public string @Type { get; set; }
        public string Payload { get; set; }
        public Guid IdempotentToken { get; set; }
        public Guid OrderIdempotentToken { get; set; }
        public long OrderId { get; set; }
    }
}
