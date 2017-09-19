using ControlPanel.WeiBo;
using Hidistro.Core;
using Hidistro.Entities.Weibo;
using Hidistro.UI.Common.Controls;
using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Hidistro.UI.SaleSystem.CodeBehind
{
	[System.Web.UI.ParseChildren(true)]
	public class VFriendsCircleDetail : VshopTemplatedWebControl
	{
		private System.Web.UI.WebControls.Repeater TopCtx;

		private System.Web.UI.WebControls.Repeater ItemCtx;

		private System.Web.UI.HtmlControls.HtmlInputHidden hdDesc;

		protected int MaterialID = 0;

		protected override void OnInit(System.EventArgs e)
		{
			if (this.SkinName == null)
			{
				this.SkinName = "skin-VFriendsCircleDetail.html";
			}
			base.OnInit(e);
		}

		protected override void AttachChildControls()
		{
			this.hdDesc = (System.Web.UI.HtmlControls.HtmlInputHidden)this.FindControl("hdDesc");
			this.MaterialID = Globals.RequestQueryNum("ID");
			if (this.MaterialID <= 0)
			{
				this.Page.Response.Redirect("/");
			}
			ArticleInfo articleInfo = ArticleHelper.GetArticleInfo(this.MaterialID);
			if (articleInfo == null)
			{
				this.Page.Response.Redirect("/");
			}
			string title = articleInfo.Title;
			System.DateTime now = System.DateTime.Now;
			this.TopCtx = (System.Web.UI.WebControls.Repeater)this.FindControl("TopCtx");
			this.ItemCtx = (System.Web.UI.WebControls.Repeater)this.FindControl("ItemCtx");
			System.Collections.Generic.List<ArticleInfo> list = new System.Collections.Generic.List<ArticleInfo>();
			list.Add(articleInfo);
			this.TopCtx.DataSource = list;
			this.TopCtx.DataBind();
			if (articleInfo.ArticleType == ArticleType.List)
			{
				string value = Globals.ReplaceHtmlTag(articleInfo.Content, 50);
				if (!string.IsNullOrEmpty(value))
				{
					this.hdDesc.Value = value;
				}
				System.Collections.Generic.IList<ArticleItemsInfo> itemsInfo = articleInfo.ItemsInfo;
				this.ItemCtx.DataSource = itemsInfo;
				this.ItemCtx.DataBind();
			}
			else
			{
				string value = Globals.ReplaceHtmlTag(articleInfo.Memo, 50);
				if (!string.IsNullOrEmpty(value))
				{
					this.hdDesc.Value = value;
				}
			}
			PageTitle.AddSiteNameTitle(title);
		}
	}
}
