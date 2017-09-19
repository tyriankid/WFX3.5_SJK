using Hidistro.Core;
using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace Hidistro.UI.Web.GetStoreCard
{
	public class Default : System.Web.UI.Page
	{
		protected System.Web.UI.HtmlControls.HtmlForm form1;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			int currentDistributorId = Globals.GetCurrentDistributorId();
			base.Response.Redirect(string.Concat(new object[]
			{
				"/Vshop/StoreCard.aspx?userId=",
				currentDistributorId,
				"&ReferralId=",
				currentDistributorId
			}));
			base.Response.End();
		}
	}
}
