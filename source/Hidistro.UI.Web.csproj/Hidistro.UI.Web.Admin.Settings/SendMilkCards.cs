using Hidistro.ControlPanel.Store;
using Hidistro.Core;
using Hidistro.Core.Entities;
using Hidistro.Entities.Store;
using Hidistro.UI.Common.Controls;
using Hidistro.UI.ControlPanel.Utility;
using System;
using System.Text.RegularExpressions;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Hidistro.UI.Web.Admin.settings
{
	public class SendMilkCards : AdminPage
	{

        protected TextBox usernamename;
        private string cardies;//站点id

		protected SendMilkCards() : base("m09", "szp11")
		{
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
           
		}

		protected void btnSend_Click(object sender, System.EventArgs e)
		{
            this.cardies = usernamename.Text.Trim().Replace("\r\n",",");

            string[] openids = cardies.Split(',');
            string[] cardids = this.Page.Request.QueryString["cardids"].Split(',');

            if (openids.Length != cardids.Length)
            {
                this.ShowMsg("奶卡数量与用户数量不匹配，请保持一致再提交！",false);
                return;
            }
            int sendCounts = VShopHelper.SendMilkCards(openids, cardids);
            if (sendCounts>0)
            {
                this.ShowMsg("发送成功！勾选了"+ cardids.Length+"张奶卡，"+ openids.Length+"个用户，成功发送了"+sendCounts+"位用户", true);
            }
            else
            {
                this.ShowMsg("发送失败！", false);
            }

        }
	}
}
