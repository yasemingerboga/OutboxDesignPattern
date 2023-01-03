﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainPayment
{
    public class PaymentOutbox
    {
        public PaymentOutbox()
        {

        }
        public DateTime OccuredOn { get; set; }
        public DateTime? ProcessedDate { get; set; }
        public string @Type { get; set; }
        public string Payload { get; set; }
        public Guid IdempotentToken { get; set; }
    }
}