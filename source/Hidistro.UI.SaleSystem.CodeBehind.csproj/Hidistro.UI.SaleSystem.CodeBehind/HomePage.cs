using Hidistro.Core;
using Hidistro.Core.Entities;
using Hidistro.Entities.Members;
using Hidistro.SaleSystem.Vshop;
using System;
using System.ComponentModel;
using System.Web.UI;

namespace Hidistro.UI.SaleSystem.CodeBehind
{
	[System.Web.UI.ParseChildren(true)]
	public class HomePage : NewTemplatedWebControl
	{
		[System.ComponentModel.Bindable(true)]
		public string TempFilePath
		{
			get;
			set;
		}

		protected override void OnInit(System.EventArgs e)
		{
			this.TempFilePath = "Skin-HomePage.html";
			if (this.SkinName == null)
			{
				this.SkinName = this.TempFilePath;
			}
			base.OnInit(e);
		}

        /// <summary>
        /// 重写首页_三剑客(如果访问的首页是服务站点并且服务站点有幻灯片信息时，重写)
        /// </summary>
        /// <returns></returns>
        protected override bool LoadHtmlThemedControl()
        {
            string text = System.IO.File.ReadAllText(this.Page.Request.MapPath(this.SkinPath), System.Text.Encoding.UTF8);
            bool result;
            if (!string.IsNullOrEmpty(text))
            {

                SiteSettings masterSettings = SettingsManager.GetMasterSettings(true);
                string vTheme = masterSettings.VTheme;
                int currentDistributorId = Globals.GetCurrentDistributorId();
                DistributorsInfo distributorInfo = DistributorsBrower.GetDistributorInfo(currentDistributorId);


                if (vTheme == "t15" && Core.Globals.GetCurrentDistributorId() > 0 && !string.IsNullOrEmpty( distributorInfo.BannerURL))
                {
                    text = "";//#mySwipe ul  现有的li清空
                    text += " <%@ Control Language = \"C#\" %> ";
                    text += " <%@ Register TagPrefix = \"Hi\" Namespace = \"HiTemplate\" Assembly = \"HiTemplate\" %> ";
                    text += " <div class=\"members_con\" style=\"margin:0 auto\"> ";
                    text += " <section class=\"members_flash j-swipe\" id=\"mySwipe\"> ";
                    text += " <ul class=\"clearfix\">";
                    for (int i = 0; i < distributorInfo.BannerURL.Split(',').Length; i++)
                    {
                        text += "<li><a href = \"\" title=\"\"><img src = \"" + distributorInfo.BannerURL.Split(',').GetValue(i) + "\" width=\"100%\" /></a></li>";
                    }
                    text += " </ul>";
                    text += " <section class=\"members_flash_time\">";
                    text += " </section>";
                    text += " </section>";
                    text += " </div>";
                    text += " <div class=\"members_con\">";
                    text += " <section class=\"members_search\">";
                    text += " <form action = \"/ProductList.aspx\" method=\"get\">";
                    text += " <input type = \"text\" name=\"keyWord\" id=\"keyWord\" value=\"\" placeholder=\"商品搜索：请输入商品关键字\">";
                    text += " <button type = \"submit\" ></button>";
                    text += " </form>";
                    text += " </section >";
                    text += " </div >";
                    text += " <Hi:GoodsListModule runat = \"server\"  Type=\"goodGroup\" Layout=\"4\" ShowName=\"True\" ShowIco=\"True\" ShowPrice=\"True\" ";
                    text += " DataUrl =\"/api/Hi_Ajax_GoodsListGroup.ashx\" ID=\"group_9d29f6a886654f98b1412a15e9184565\" TemplateFile=\"/Admin/shop/Modules/GoodGroup4.cshtml\"";
                    text += " GoodListSize=\"6\" FirstPriority=\"\"  SecondPriority=\"3\"  ShowMaketPrice=\"True\"  />";




                }
                System.Web.UI.Control control = this.Page.ParseControl(text);
                control.ID = "_";
                this.Controls.Add(control);
                result = true;
            }
            else
            {
                result = false;
            }
            return result;
        }

    }
}
