using Hidistro.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hidistro.Entities.FenXiao
{
    public class MilkCardInfo
    {

        public Guid ID { get; set; }

        public string CardNum { get; set; }

        public string CardPassword { get; set; }

        public int SiteId { get; set; }

        public int Status { get; set; }

        public DateTime CreateDate  { get;set;}

        public DateTime OrderDate { get; set; }

        public DateTime startSendDate { get; set; }

        public string OrderId { get; set; }

        public int UserId { get; set; }

        public int ProductId { get; set; }

        public int FreeSendDays { get; set; }

        public int FreeQuantityPerDay { get; set; }
    }
}
