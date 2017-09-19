using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebSite.Vshop
{
    public partial class DailyControl : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public DateTime startDate;
        public DateTime StartDate
        {
            set { startDate = value; }
        }

        public DateTime endDate;
        public DateTime EndDate
        {
            set { endDate = value; }
        }
    }
}