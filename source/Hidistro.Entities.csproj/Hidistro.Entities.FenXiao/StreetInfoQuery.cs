using Hidistro.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hidistro.Entities.FenXiao
{
    public class StreetInfoQuery : Pagination
    {

        public string streetName { get; set; }

        public string regionCode { get; set; }

        public int distributorId { get; set; }

    }

    public class QuestInfoQuery : Pagination
    {
        public string UserId { get; set; }
        public string OrderId { get; set; }

        public string Status { get; set; }

        public string SendStartDate { get; set; }

        public string SendEndDate { get; set; }
        public string UserName { get; set; }

        public string Sort { get; set; }
    }


}
