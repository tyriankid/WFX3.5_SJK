using Hidistro.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hidistro.Entities.FenXiao
{
    public class MilkCardQuery : Pagination
    {

        public int siteId { get; set; }

        public int status { get; set; }


    }
}
