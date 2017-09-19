using ControlPanel.Promotions;
using Hidistro.ControlPanel.Sales;
using Hidistro.Core;
using Hidistro.Core.Entities;
using Hidistro.Entities;
using Hidistro.Entities.Sales;
using Hidistro.UI.ControlPanel.Utility;
using Hidistro.Vshop;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using System.Xml;

namespace Hidistro.UI.Web.Admin.promotion
{
	public class BatchPrintData : AdminPage
	{
		protected string ReUrl = Globals.RequestQueryStr("reurl");

		protected string btnShowStyle = "inline-block";

		protected int ShipperNameLength;

		protected static string orderlist = string.Empty;

		protected static string pidlist = string.Empty;

		protected int pringrows;

		protected string mailNo = "";

		protected string templateName = "";

		protected string width = "";

		protected string height = "";

		protected string UpdateOrderIds = string.Empty;

		protected string ShipperName = string.Empty;

		protected string SizeShipperName = string.Empty;

		protected string CellPhone = string.Empty;

		protected string SizeCellPhone = string.Empty;

		protected string TelPhone = string.Empty;

		protected string SizeTelPhone = string.Empty;

		protected string Address = string.Empty;

		protected string SizeAddress = string.Empty;

		protected string Zipcode = string.Empty;

		protected string SizeZipcode = string.Empty;

		protected string Province = string.Empty;

		protected string SizeProvnce = string.Empty;

		protected string City = string.Empty;

		protected string SizeCity = string.Empty;

		protected string District = string.Empty;

		protected string SizeDistrict = string.Empty;

		protected string ShipToDate = string.Empty;

		protected string SizeShipToDate = string.Empty;

		protected string OrderId = string.Empty;

		protected string SizeOrderId = string.Empty;

		protected string OrderTotal = string.Empty;

		protected string SizeOrderTotal = string.Empty;

		protected string Shipitemweith = string.Empty;

		protected string SizeShipitemweith = string.Empty;

		protected string Remark = string.Empty;

		protected string SizeRemark = string.Empty;

		protected string ShipitemInfos = string.Empty;

		protected string SizeitemInfos = string.Empty;

		protected string SiteName = string.Empty;

		protected string SizeSiteName = string.Empty;

		protected string ShipTo = string.Empty;

		protected string SizeShipTo = string.Empty;

		protected string ShipTelPhone = string.Empty;

		protected string SizeShipTelPhone = string.Empty;

		protected string ShipCellPhone = string.Empty;

		protected string SizeShipCellPhone = string.Empty;

		protected string ShipZipCode = string.Empty;

		protected string SizeShipZipCode = string.Empty;

		protected string ShipAddress = string.Empty;

		protected string ShipSizeAddress = string.Empty;

		protected string ShipProvince = string.Empty;

		protected string ShipSizeProvnce = string.Empty;

		protected string ShipCity = string.Empty;

		protected string ShipSizeCity = string.Empty;

		protected string ShipDistrict = string.Empty;

		protected string ShipSizeDistrict = string.Empty;

		protected string Departure = string.Empty;

		protected string SizeDeparture = string.Empty;

		protected string Destination = string.Empty;

		protected string SizeDestination = string.Empty;

		protected string SelfDefinedContent = string.Empty;

		protected string SizeSelfDefinedContent = string.Empty;

		protected int SelfDefinedContentLength;

		protected string SelContent = string.Empty;

		protected string SizeSelContent = string.Empty;

		protected int SelContentLength;

		private SiteSettings siteSettings = SettingsManager.GetMasterSettings(false);

		protected System.Web.UI.WebControls.Repeater rptItemList;

		protected System.Web.UI.WebControls.Panel pnlShipper;

		protected ShippersDropDownList ddlShoperTag;

		protected System.Web.UI.WebControls.Panel pnlEmptySender;

		protected System.Web.UI.WebControls.Panel pnlTemplates;

		protected System.Web.UI.WebControls.DropDownList ddlTemplates;

		protected System.Web.UI.WebControls.TextBox txtStartCode;

		protected System.Web.UI.WebControls.Literal litNumber;

		protected System.Web.UI.WebControls.Panel pnlEmptyTemplates;

		protected System.Web.UI.WebControls.Button btnPrint;

		protected BatchPrintData() : base("m03", "00000")
		{
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			string a = Globals.RequestFormStr("posttype");
			BatchPrintData.orderlist = Globals.RequestQueryStr("OrderIds").Trim(new char[]
			{
				','
			});
			BatchPrintData.pidlist = Globals.RequestQueryStr("PIds").Trim(new char[]
			{
				','
			});
			if (a == "printall")
			{
				string value = Globals.RequestFormStr("data");
				base.Response.ContentType = "text/html";
				JArray jArray = (JArray)JsonConvert.DeserializeObject(value);
				using (System.Collections.Generic.IEnumerator<JToken> enumerator = jArray.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						JObject jObject = (JObject)enumerator.Current;
						int iiscombine = Globals.ToNum(jObject["iscombine"].ToString());
						int num = Globals.ToNum(jObject["shoper"].ToString());
						string text = jObject["compname"].ToString().Trim();
						string expressCompanyAbb = string.Empty;
						ExpressCompanyInfo expressCompanyInfo = ExpressHelper.FindNode(text);
						if (expressCompanyInfo != null)
						{
							expressCompanyAbb = expressCompanyInfo.Kuaidi100Code;
						}
						else
						{
							expressCompanyAbb = text;
						}
						string text2 = jObject["l"].ToString();
						JArray jArray2 = (JArray)jObject["data"];
						string arg_135_0 = string.Empty;
						string arg_13B_0 = string.Empty;
						string arg_141_0 = string.Empty;
						if (num > 0 && !string.IsNullOrEmpty(text2) && !string.IsNullOrEmpty(text))
						{
							string text3 = string.Empty;
							string text4 = string.Empty;
							if (jArray2.Count > 0)
							{
								using (System.Collections.Generic.IEnumerator<JToken> enumerator2 = jArray2.GetEnumerator())
								{
									while (enumerator2.MoveNext())
									{
										JObject jObject2 = (JObject)enumerator2.Current;
										text3 = text3 + "," + jObject2["orderid"].ToString();
										text4 = text4 + "," + jObject2["pid"].ToString();
										GameHelper.SetPrintOrderExpress(jObject2["orderid"].ToString(), jObject2["pid"].ToString(), text, expressCompanyAbb, jObject2["shipordernumber"].ToString());
									}
								}
								this.printdata(num, text2, text3.Trim(new char[]
								{
									','
								}), text4.Trim(new char[]
								{
									','
								}), iiscombine);
							}
						}
					}
					return;
				}
			}
			if (a == "getmyaddr")
			{
				base.Response.ContentType = "application/json";
				string s = "{\"type\":\"0\",\"data\":\"[]\"}";
				System.Collections.Generic.IList<ShippersInfo> shippers = SalesHelper.GetShippers(false);
				System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
				if (shippers.Count > 0)
				{
					bool flag = false;
					foreach (ShippersInfo current in shippers)
					{
						if (current.IsDefault && !flag)
						{
							flag = true;
							stringBuilder.Append(string.Concat(new object[]
							{
								"<option value='",
								current.ShipperId,
								"' selected='selected'>",
								Globals.String2Json(current.ShipperName),
								"</option>"
							}));
						}
						else
						{
							stringBuilder.Append(string.Concat(new object[]
							{
								"<option value='",
								current.ShipperId,
								"'>",
								Globals.String2Json(current.ShipperName),
								"</option>"
							}));
						}
					}
					s = "{\"type\":\"1\",\"data\":\"" + stringBuilder.ToString() + "\"}";
				}
				base.Response.Write(s);
				base.Response.End();
				return;
			}
			if (string.IsNullOrEmpty(this.ReUrl))
			{
				this.ReUrl = "PrizeList.aspx?ShowTabNum=2&pageSize=10";
			}
			if (!string.IsNullOrEmpty(BatchPrintData.orderlist) || !string.IsNullOrEmpty(BatchPrintData.pidlist))
			{
				int num2 = (!string.IsNullOrEmpty(BatchPrintData.orderlist)) ? BatchPrintData.orderlist.Split(new char[]
				{
					','
				}).Length : 0;
				int num3 = (!string.IsNullOrEmpty(BatchPrintData.pidlist)) ? BatchPrintData.pidlist.Split(new char[]
				{
					','
				}).Length : 0;
				this.litNumber.Text = (num2 + num3).ToString();
				this.btnPrint.Click += new System.EventHandler(this.btnbtnPrint_Click);
				this.ddlShoperTag.SelectedIndexChanged += new System.EventHandler(this.ddlShoperTag_SelectedIndexChanged);
				if (!this.Page.IsPostBack)
				{
					this.ddlShoperTag.DataBind();
					System.Collections.Generic.IList<ShippersInfo> shippers2 = SalesHelper.GetShippers(false);
					foreach (ShippersInfo current2 in shippers2)
					{
						if (Globals.ToNum(current2.ShipperTag) % 2 == 1)
						{
							this.ddlShoperTag.SelectedValue = current2.ShipperId;
							break;
						}
					}
					this.LoadShipper();
					this.LoadTemplates();
					this.LoadData(BatchPrintData.orderlist, BatchPrintData.pidlist);
				}
			}
		}

		private void PrintPage(string pagewidth, string pageheght)
		{
			System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
			stringBuilder.Append("<script src='../js/LodopFuncs.js'></script><object id='LODOP_OB' classid='clsid:2105C259-1E0C-4534-8141-A753534CB4CA' width='0' height='0'><embed id='LODOP_EM' type='application/x-print-lodop' width='0' height='0'></embed></object><script language='javascript'>");
			stringBuilder.Append("function clicks(){");
			if (!string.IsNullOrEmpty(this.SizeShipperName.Trim()))
			{
				stringBuilder.Append(string.Concat(new string[]
				{
					" var ShipperName=[",
					this.ShipperName.Substring(0, this.ShipperName.Length - 1),
					"];var SizeShipperName=[",
					this.SizeShipperName.Substring(0, this.SizeShipperName.Length - 1),
					"];"
				}));
			}
			if (!string.IsNullOrEmpty(this.SizeSelfDefinedContent.Trim()))
			{
				stringBuilder.Append(string.Concat(new string[]
				{
					" var SelfDefinedContent=[",
					this.SelfDefinedContent.Substring(0, this.SelfDefinedContent.Length - 1),
					"];var SizeSelfDefinedContent=[",
					this.SizeSelfDefinedContent.Substring(0, this.SizeSelfDefinedContent.Length - 1),
					"];"
				}));
			}
			if (!string.IsNullOrEmpty(this.SizeSelContent.Trim()))
			{
				stringBuilder.Append(string.Concat(new string[]
				{
					" var SelContent=[",
					this.SelContent.Substring(0, this.SelContent.Length - 1),
					"];var SizeSelContent=[",
					this.SizeSelContent.Substring(0, this.SizeSelContent.Length - 1),
					"];"
				}));
			}
			if (!string.IsNullOrEmpty(this.SizeCellPhone.Trim()))
			{
				stringBuilder.Append(string.Concat(new string[]
				{
					" var CellPhone=[",
					this.CellPhone.Substring(0, this.CellPhone.Length - 1),
					"];var SizeCellPhone=[",
					this.SizeCellPhone.Substring(0, this.SizeCellPhone.Length - 1),
					"];"
				}));
			}
			if (!string.IsNullOrEmpty(this.SizeTelPhone.Trim()))
			{
				stringBuilder.Append(string.Concat(new string[]
				{
					" var TelPhone=[",
					this.TelPhone.Substring(0, this.TelPhone.Length - 1),
					"];var SizeTelPhone=[",
					this.SizeTelPhone.Substring(0, this.SizeTelPhone.Length - 1),
					"];"
				}));
			}
			if (!string.IsNullOrEmpty(this.SizeAddress.Trim()))
			{
				stringBuilder.Append(string.Concat(new string[]
				{
					" var Address=[",
					this.Address.Substring(0, this.Address.Length - 1),
					"];var SizeAddress=[",
					this.SizeAddress.Substring(0, this.SizeAddress.Length - 1),
					"];"
				}));
			}
			if (!string.IsNullOrEmpty(this.SizeZipcode.Trim()))
			{
				stringBuilder.Append(string.Concat(new string[]
				{
					" var Zipcode=[",
					this.Zipcode.Substring(0, this.Zipcode.Length - 1),
					"];var SizeZipcode=[",
					this.SizeZipcode.Substring(0, this.SizeZipcode.Length - 1),
					"];"
				}));
			}
			if (!string.IsNullOrEmpty(this.SizeProvnce.Trim()))
			{
				stringBuilder.Append(string.Concat(new string[]
				{
					" var Province=[",
					this.Province.Substring(0, this.Province.Length - 1),
					"];var SizeProvnce=[",
					this.SizeProvnce.Substring(0, this.SizeProvnce.Length - 1),
					"];"
				}));
			}
			if (!string.IsNullOrEmpty(this.SizeCity.Trim()))
			{
				stringBuilder.Append(string.Concat(new string[]
				{
					" var City=[",
					this.City.Substring(0, this.City.Length - 1),
					"];var SizeCity=[",
					this.SizeCity.Substring(0, this.SizeCity.Length - 1),
					"];"
				}));
			}
			if (!string.IsNullOrEmpty(this.SizeDistrict.Trim()))
			{
				stringBuilder.Append(string.Concat(new string[]
				{
					" var District=[",
					this.District.Substring(0, this.District.Length - 1),
					"];var SizeDistrict=[",
					this.SizeDistrict.Substring(0, this.SizeDistrict.Length - 1),
					"];"
				}));
			}
			if (!string.IsNullOrEmpty(this.SizeShipToDate.Trim()))
			{
				stringBuilder.Append(string.Concat(new string[]
				{
					" var ShipToDate=[",
					this.ShipToDate.Substring(0, this.ShipToDate.Length - 1),
					"];var SizeShipToDate=[",
					this.SizeShipToDate.Substring(0, this.SizeShipToDate.Length - 1),
					"];"
				}));
			}
			if (!string.IsNullOrEmpty(this.SizeOrderId.Trim()))
			{
				stringBuilder.Append(string.Concat(new string[]
				{
					" var OrderId=[",
					this.OrderId.Substring(0, this.OrderId.Length - 1),
					"];var SizeOrderId=[",
					this.SizeOrderId.Substring(0, this.SizeOrderId.Length - 1),
					"];"
				}));
			}
			if (!string.IsNullOrEmpty(this.SizeOrderTotal.Trim()))
			{
				stringBuilder.Append(string.Concat(new string[]
				{
					" var OrderTotal=[",
					this.OrderTotal.Substring(0, this.OrderTotal.Length - 1),
					"];var SizeOrderTotal=[",
					this.SizeOrderTotal.Substring(0, this.SizeOrderTotal.Length - 1),
					"];"
				}));
			}
			if (!string.IsNullOrEmpty(this.SizeShipitemweith.Trim()))
			{
				stringBuilder.Append(string.Concat(new string[]
				{
					" var Shipitemweith=[",
					this.Shipitemweith.Substring(0, this.Shipitemweith.Length - 1),
					"];var SizeShipitemweith=[",
					this.SizeShipitemweith.Substring(0, this.SizeShipitemweith.Length - 1),
					"];"
				}));
			}
			if (!string.IsNullOrEmpty(this.SizeRemark.Trim()))
			{
				stringBuilder.Append(string.Concat(new string[]
				{
					" var Remark=[",
					this.Remark.Substring(0, this.Remark.Length - 1),
					"];var SizeRemark=[",
					this.SizeRemark.Substring(0, this.SizeRemark.Length - 1),
					"];"
				}));
			}
			if (!string.IsNullOrEmpty(this.SizeitemInfos.Trim()))
			{
				stringBuilder.Append(string.Concat(new string[]
				{
					" var ShipitemInfos=[",
					this.ShipitemInfos.Substring(0, this.ShipitemInfos.Length - 1),
					"];var SizeitemInfos=[",
					this.SizeitemInfos.Substring(0, this.SizeitemInfos.Length - 1),
					"];"
				}));
			}
			if (!string.IsNullOrEmpty(this.SizeSiteName.Trim()))
			{
				stringBuilder.Append(string.Concat(new string[]
				{
					" var SiteName=[",
					this.SiteName.Substring(0, this.SiteName.Length - 1),
					"];var SizeSiteName=[",
					this.SizeSiteName.Substring(0, this.SizeSiteName.Length - 1),
					"];"
				}));
			}
			if (!string.IsNullOrEmpty(this.SizeShipTo.Trim()))
			{
				stringBuilder.Append(string.Concat(new string[]
				{
					" var ShipTo=[",
					this.ShipTo.Substring(0, this.ShipTo.Length - 1),
					"];var SizeShipTo=[",
					this.SizeShipTo.Substring(0, this.SizeShipTo.Length - 1),
					"];"
				}));
			}
			if (!string.IsNullOrEmpty(this.SizeShipTelPhone.Trim()))
			{
				stringBuilder.Append(string.Concat(new string[]
				{
					" var ShipTelPhone=[",
					this.ShipTelPhone.Substring(0, this.ShipTelPhone.Length - 1),
					"];var SizeShipTelPhone=[",
					this.SizeShipTelPhone.Substring(0, this.SizeShipTelPhone.Length - 1),
					"];"
				}));
			}
			if (!string.IsNullOrEmpty(this.SizeShipCellPhone.Trim()))
			{
				stringBuilder.Append(string.Concat(new string[]
				{
					" var ShipCellPhone=[",
					this.ShipCellPhone.Substring(0, this.ShipCellPhone.Length - 1),
					"];var SizeShipCellPhone=[",
					this.SizeShipCellPhone.Substring(0, this.SizeShipCellPhone.Length - 1),
					"];"
				}));
			}
			if (!string.IsNullOrEmpty(this.SizeShipZipCode.Trim()))
			{
				stringBuilder.Append(string.Concat(new string[]
				{
					" var ShipZipCode=[",
					this.ShipZipCode.Substring(0, this.ShipZipCode.Length - 1),
					"];var SizeShipZipCode=[",
					this.SizeShipZipCode.Substring(0, this.SizeShipZipCode.Length - 1),
					"];"
				}));
			}
			if (!string.IsNullOrEmpty(this.ShipSizeAddress.Trim()))
			{
				stringBuilder.Append(string.Concat(new string[]
				{
					" var ShipAddress=[",
					this.ShipAddress.Substring(0, this.ShipAddress.Length - 1),
					"];var ShipSizeAddress=[",
					this.ShipSizeAddress.Substring(0, this.ShipSizeAddress.Length - 1),
					"];"
				}));
			}
			if (!string.IsNullOrEmpty(this.ShipSizeProvnce.Trim()))
			{
				stringBuilder.Append(string.Concat(new string[]
				{
					" var ShipProvince=[",
					this.ShipProvince.Substring(0, this.ShipProvince.Length - 1),
					"];var ShipSizeProvnce=[",
					this.ShipSizeProvnce.Substring(0, this.ShipSizeProvnce.Length - 1),
					"];"
				}));
			}
			if (!string.IsNullOrEmpty(this.ShipSizeCity.Trim()))
			{
				stringBuilder.Append(string.Concat(new string[]
				{
					" var ShipCity=[",
					this.ShipCity.Substring(0, this.ShipCity.Length - 1),
					"];var ShipSizeCity=[",
					this.ShipSizeCity.Substring(0, this.ShipSizeCity.Length - 1),
					"];"
				}));
			}
			if (!string.IsNullOrEmpty(this.ShipSizeDistrict.Trim()))
			{
				stringBuilder.Append(string.Concat(new string[]
				{
					" var ShipDistrict=[",
					this.ShipDistrict.Substring(0, this.ShipDistrict.Length - 1),
					"];var ShipSizeDistrict=[",
					this.ShipSizeDistrict.Substring(0, this.ShipSizeDistrict.Length - 1),
					"];"
				}));
			}
			if (!string.IsNullOrEmpty(this.SizeDeparture.Trim()))
			{
				stringBuilder.Append(string.Concat(new string[]
				{
					" var Departure=[",
					this.Departure.Substring(0, this.Departure.Length - 1),
					"];var SizeDeparture=[",
					this.SizeDeparture.Substring(0, this.SizeDeparture.Length - 1),
					"];"
				}));
			}
			if (!string.IsNullOrEmpty(this.SizeDestination.Trim()))
			{
				stringBuilder.Append(string.Concat(new string[]
				{
					" var Destination=[",
					this.Destination.Substring(0, this.Destination.Length - 1),
					"];var SizeDestination=[",
					this.SizeDestination.Substring(0, this.SizeDestination.Length - 1),
					"];"
				}));
			}
			stringBuilder.Append(" var LODOP = getLodop(document.getElementById('LODOP_OB'), document.getElementById('LODOP_EM'));");
			stringBuilder.Append(" try{ ");
			stringBuilder.Append("  for(var i=0;i<" + this.pringrows + ";++i){ ");
			stringBuilder.Append("parent.showdiv();");
			stringBuilder.Append(string.Concat(new object[]
			{
				" LODOP.SET_PRINT_PAGESIZE (1,",
				decimal.Parse(pagewidth) * 10m,
				",",
				decimal.Parse(pageheght) * 10m,
				",\"\");"
			}));
			stringBuilder.Append(" LODOP.SET_PRINT_STYLE(\"FontSize\",12);");
			stringBuilder.Append(" LODOP.SET_PRINT_STYLE(\"Bold\",1);");
			if (!string.IsNullOrEmpty(this.SizeShipperName.Trim()))
			{
				int num = this.ShipperNameLength / this.pringrows;
				this.ShipperName.Trim().Trim(new char[]
				{
					','
				});
				stringBuilder.Append(string.Concat(new object[]
				{
					"LODOP.ADD_PRINT_TEXT(SizeShipperName[i*",
					num,
					"].split(',')[0],SizeShipperName[i*",
					num,
					"].split(',')[1],SizeShipperName[i*",
					num,
					"].split(',')[2],SizeShipperName[i*",
					num,
					"].split(',')[3],ShipperName[0]);"
				}));
				for (int i = 1; i < num; i++)
				{
					stringBuilder.Append(string.Concat(new object[]
					{
						"LODOP.ADD_PRINT_TEXT(SizeShipperName[i*",
						num,
						"+",
						i,
						"].split(',')[0],SizeShipperName[i*",
						num,
						"+",
						i,
						"].split(',')[1],SizeShipperName[i*",
						num,
						"+",
						i,
						"].split(',')[2],SizeShipperName[i*",
						num,
						"+",
						i,
						"].split(',')[3],ShipperName[0]);"
					}));
				}
			}
			if (!string.IsNullOrEmpty(this.SelfDefinedContent.Trim()))
			{
				int num2 = this.SelfDefinedContentLength / this.pringrows;
				this.SelfDefinedContent.Trim().Trim(new char[]
				{
					','
				});
				stringBuilder.Append(string.Concat(new object[]
				{
					"LODOP.ADD_PRINT_TEXT(SizeSelfDefinedContent[i*",
					num2,
					"].split(',')[0],SizeSelfDefinedContent[i*",
					num2,
					"].split(',')[1],SizeSelfDefinedContent[i*",
					num2,
					"].split(',')[2],SizeSelfDefinedContent[i*",
					num2,
					"].split(',')[3],SelfDefinedContent[0]);"
				}));
				for (int j = 1; j < num2; j++)
				{
					stringBuilder.Append(string.Concat(new object[]
					{
						"LODOP.ADD_PRINT_TEXT(SizeSelfDefinedContent[i*",
						num2,
						"+",
						j,
						"].split(',')[0],SizeSelfDefinedContent[i*",
						num2,
						"+",
						j,
						"].split(',')[1],SizeSelfDefinedContent[i*",
						num2,
						"+",
						j,
						"].split(',')[2],SizeSelfDefinedContent[i*",
						num2,
						"+",
						j,
						"].split(',')[3],SelfDefinedContent[i*",
						num2,
						"+",
						j,
						"]);"
					}));
				}
			}
			if (!string.IsNullOrEmpty(this.SelContent.Trim()))
			{
				int num3 = this.SelContentLength / this.pringrows;
				this.SelContent.Trim().Trim(new char[]
				{
					','
				});
				stringBuilder.Append(string.Concat(new object[]
				{
					"LODOP.ADD_PRINT_TEXT(SizeSelContent[i*",
					num3,
					"].split(',')[0],SizeSelContent[i*",
					num3,
					"].split(',')[1],SizeSelContent[i*",
					num3,
					"].split(',')[2],SizeSelContent[i*",
					num3,
					"].split(',')[3],SelContent[0]);"
				}));
				for (int k = 1; k < num3; k++)
				{
					stringBuilder.Append(string.Concat(new object[]
					{
						"LODOP.ADD_PRINT_TEXT(SizeSelContent[i*",
						num3,
						"+",
						k,
						"].split(',')[0],SizeSelContent[i*",
						num3,
						"+",
						k,
						"].split(',')[1],SizeSelContent[i*",
						num3,
						"+",
						k,
						"].split(',')[2],SizeSelContent[i*",
						num3,
						"+",
						k,
						"].split(',')[3],SelContent[i*",
						num3,
						"+",
						k,
						"]);"
					}));
				}
			}
			if (!string.IsNullOrEmpty(this.SizeCellPhone.Trim()))
			{
				stringBuilder.Append(" LODOP.ADD_PRINT_TEXT(SizeCellPhone[i].split(',')[0],SizeCellPhone[i].split(',')[1],SizeCellPhone[i].split(',')[2],SizeCellPhone[i].split(',')[3],CellPhone[0]);");
			}
			if (!string.IsNullOrEmpty(this.SizeTelPhone.Trim()))
			{
				stringBuilder.Append(" LODOP.ADD_PRINT_TEXT(SizeTelPhone[i].split(',')[0],SizeTelPhone[i].split(',')[1],SizeTelPhone[i].split(',')[2],SizeTelPhone[i].split(',')[3],TelPhone[0]);");
			}
			if (!string.IsNullOrEmpty(this.SizeAddress.Trim()))
			{
				stringBuilder.Append(" LODOP.ADD_PRINT_TEXT(SizeAddress[i].split(',')[0],SizeAddress[i].split(',')[1],SizeAddress[i].split(',')[2],SizeAddress[i].split(',')[3],Address[0]);");
			}
			if (!string.IsNullOrEmpty(this.SizeZipcode.Trim()))
			{
				stringBuilder.Append(" LODOP.ADD_PRINT_TEXT(SizeZipcode[i].split(',')[0],Zipcode[0]);");
			}
			if (!string.IsNullOrEmpty(this.SizeProvnce.Trim()))
			{
				stringBuilder.Append(" LODOP.ADD_PRINT_TEXT(SizeProvnce[i].split(',')[0],SizeProvnce[i].split(',')[1],SizeProvnce[i].split(',')[2],SizeProvnce[i].split(',')[3],Province[0]);");
			}
			if (!string.IsNullOrEmpty(this.SizeCity.Trim()))
			{
				stringBuilder.Append(" LODOP.ADD_PRINT_TEXT(SizeCity[i].split(',')[0],SizeCity[i].split(',')[1],SizeCity[i].split(',')[2],SizeCity[i].split(',')[3],City[0]);");
			}
			if (!string.IsNullOrEmpty(this.SizeDistrict.Trim()))
			{
				stringBuilder.Append(" LODOP.ADD_PRINT_TEXT(SizeDistrict[i].split(',')[0],SizeDistrict[i].split(',')[1],SizeDistrict[i].split(',')[2],SizeDistrict[i].split(',')[3],District[0]);");
			}
			if (!string.IsNullOrEmpty(this.SizeShipToDate.Trim()))
			{
				stringBuilder.Append(" LODOP.ADD_PRINT_TEXT(SizeShipToDate[i].split(',')[0],SizeShipToDate[i].split(',')[1],SizeShipToDate[i].split(',')[2],SizeShipToDate[i].split(',')[3],ShipToDate[i]);");
			}
			if (!string.IsNullOrEmpty(this.SizeOrderId.Trim()))
			{
				stringBuilder.Append(" LODOP.ADD_PRINT_TEXT(SizeOrderId[i].split(',')[0],SizeOrderId[i].split(',')[1],SizeOrderId[i].split(',')[2],SizeOrderId[i].split(',')[3],OrderId[i]);");
			}
			if (!string.IsNullOrEmpty(this.SizeOrderTotal.Trim()))
			{
				stringBuilder.Append(" LODOP.ADD_PRINT_TEXT(SizeOrderTotal[i].split(',')[0],SizeOrderTotal[i].split(',')[1],SizeOrderTotal[i].split(',')[2],SizeOrderTotal[i].split(',')[3],OrderTotal[i]);");
			}
			if (!string.IsNullOrEmpty(this.SizeShipitemweith.Trim()))
			{
				stringBuilder.Append(" LODOP.ADD_PRINT_TEXT(SizeShipitemweith[i].split(',')[0],SizeShipitemweith[i].split(',')[1],SizeShipitemweith[i].split(',')[2],SizeShipitemweith[i].split(',')[3],Shipitemweith[i]);");
			}
			if (!string.IsNullOrEmpty(this.SizeRemark.Trim()))
			{
				stringBuilder.Append(" LODOP.ADD_PRINT_TEXT(SizeRemark[i].split(',')[0],SizeRemark[i].split(',')[1],SizeRemark[i].split(',')[2],SizeRemark[i].split(',')[3],Remark[i]);");
			}
			if (!string.IsNullOrEmpty(this.SizeitemInfos.Trim()))
			{
				stringBuilder.Append(" LODOP.ADD_PRINT_TEXT(SizeitemInfos[i].split(',')[0],SizeitemInfos[i].split(',')[1],SizeitemInfos[i].split(',')[2],SizeitemInfos[i].split(',')[3],ShipitemInfos[i]);");
			}
			if (!string.IsNullOrEmpty(this.SizeSiteName.Trim()))
			{
				stringBuilder.Append(" LODOP.ADD_PRINT_TEXT(SizeSiteName[i].split(',')[0],SizeSiteName[i].split(',')[1],SizeSiteName[i].split(',')[2],SizeSiteName[i].split(',')[3],SiteName[i]);");
			}
			if (!string.IsNullOrEmpty(this.SizeShipTo.Trim()))
			{
				stringBuilder.Append(" LODOP.ADD_PRINT_TEXT(SizeShipTo[i].split(',')[0],SizeShipTo[i].split(',')[1],SizeShipTo[i].split(',')[2],SizeShipTo[i].split(',')[3],ShipTo[i]);");
			}
			if (!string.IsNullOrEmpty(this.SizeShipTelPhone.Trim()))
			{
				stringBuilder.Append(" LODOP.ADD_PRINT_TEXT(SizeShipTelPhone[i].split(',')[0],SizeShipTelPhone[i].split(',')[1],SizeShipTelPhone[i].split(',')[2],SizeShipTelPhone[i].split(',')[3],ShipTelPhone[i]);");
			}
			if (!string.IsNullOrEmpty(this.SizeShipCellPhone.Trim()))
			{
				stringBuilder.Append(" LODOP.ADD_PRINT_TEXT(SizeShipCellPhone[i].split(',')[0],SizeShipCellPhone[i].split(',')[1],SizeShipCellPhone[i].split(',')[2],SizeShipCellPhone[i].split(',')[3],ShipCellPhone[i]);");
			}
			if (!string.IsNullOrEmpty(this.SizeShipZipCode.Trim()))
			{
				stringBuilder.Append(" LODOP.ADD_PRINT_TEXT(SizeShipZipCode[i].split(',')[0],SizeShipZipCode[i].split(',')[1],SizeShipZipCode[i].split(',')[2],SizeShipZipCode[i].split(',')[3],ShipZipCode[i]);");
			}
			if (!string.IsNullOrEmpty(this.ShipSizeAddress.Trim()))
			{
				stringBuilder.Append(" LODOP.ADD_PRINT_TEXT(ShipSizeAddress[i].split(',')[0],ShipSizeAddress[i].split(',')[1],ShipSizeAddress[i].split(',')[2],ShipSizeAddress[i].split(',')[3],ShipAddress[i]);");
			}
			if (!string.IsNullOrEmpty(this.ShipSizeProvnce.Trim()))
			{
				stringBuilder.Append(" LODOP.ADD_PRINT_TEXT(ShipSizeProvnce[i].split(',')[0],ShipSizeProvnce[i].split(',')[1],ShipSizeProvnce[i].split(',')[2],ShipSizeProvnce[i].split(',')[3],ShipProvince[i]);");
			}
			if (!string.IsNullOrEmpty(this.ShipSizeCity.Trim()))
			{
				stringBuilder.Append(" LODOP.ADD_PRINT_TEXT(ShipSizeCity[i].split(',')[0],ShipSizeCity[i].split(',')[1],ShipSizeCity[i].split(',')[2],ShipSizeCity[i].split(',')[3],ShipCity[i]);");
			}
			if (!string.IsNullOrEmpty(this.ShipSizeDistrict.Trim()))
			{
				stringBuilder.Append(" LODOP.ADD_PRINT_TEXT(ShipSizeDistrict[i].split(',')[0],ShipSizeDistrict[i].split(',')[1],ShipSizeDistrict[i].split(',')[2],ShipSizeDistrict[i].split(',')[3],ShipDistrict[i]);");
			}
			if (!string.IsNullOrEmpty(this.SizeDeparture.Trim()))
			{
				stringBuilder.Append(" LODOP.ADD_PRINT_TEXT(SizeDeparture[i].split(',')[0],SizeDeparture[i].split(',')[1],SizeDeparture[i].split(',')[2],SizeDeparture[i].split(',')[3],Departure[0]);");
			}
			if (!string.IsNullOrEmpty(this.SizeDestination.Trim()))
			{
				stringBuilder.Append(" LODOP.ADD_PRINT_TEXT(SizeDestination[i].split(',')[0],SizeDestination[i].split(',')[1],SizeDestination[i].split(',')[2],SizeDestination[i].split(',')[3],Destination[i]);");
			}
			stringBuilder.Append(" LODOP.PRINT();");
			stringBuilder.Append("   }");
			stringBuilder.Append(" setTimeout(\"parent.hidediv()\",3000);");
			stringBuilder.Append("  }catch(e){ alert(\"请先安装打印控件！\"+e.message);parent.hidediv();return false;}");
			stringBuilder.Append("}");
			stringBuilder.Append(" setTimeout(\"clicks()\",1000); ");
			stringBuilder.Append("</script>");
			base.Response.Write(stringBuilder.ToString());
			base.Response.End();
		}

		private void btnbtnPrint_Click(object sender, System.EventArgs e)
		{
		}

		private System.Data.DataSet GetPrintData(string orderIds, string pIds)
		{
			orderIds = "'" + orderIds.Replace(",", "','") + "'";
			pIds = "'" + pIds.Replace(",", "','") + "'";
			return GameHelper.GetOrdersAndLines(orderIds, pIds);
		}

		private void printdata(int shipperId, string lgFile, string neworderlist, string newpidlist, int iiscombine)
		{
			ShippersInfo shipper = SalesHelper.GetShipper(shipperId);
			if (shipper == null)
			{
				this.ShowMsgToTarget("请选择一个发货人", false, "parent");
				return;
			}
			string text = System.Web.HttpContext.Current.Request.MapPath(string.Format("../../Storage/master/flex/{0}", lgFile));
			if (System.IO.File.Exists(text))
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.Load(text);
				XmlNode xmlNode = xmlDocument.DocumentElement.SelectSingleNode("//printer");
				this.templateName = xmlNode.SelectSingleNode("kind").InnerText;
				string arg_86_0 = xmlNode.SelectSingleNode("pic").InnerText;
				string innerText = xmlNode.SelectSingleNode("size").InnerText;
				this.width = innerText.Split(new char[]
				{
					':'
				})[0];
				this.height = innerText.Split(new char[]
				{
					':'
				})[1];
				System.Data.DataSet printData = this.GetPrintData(neworderlist, newpidlist);
				System.Data.DataTable dataTable = printData.Tables[0];
				this.pringrows = dataTable.Rows.Count;
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					System.Data.DataRow dataRow = dataTable.Rows[i];
					int num = i;
					if (iiscombine == 1)
					{
						while (num < dataTable.Rows.Count && num + 1 < dataTable.Rows.Count && dataTable.Rows[num]["CourierNumber"].ToString() == dataTable.Rows[num + 1]["CourierNumber"].ToString() && dataTable.Rows[num]["Receiver"].ToString() == dataTable.Rows[num + 1]["Receiver"].ToString())
						{
							this.pringrows--;
							num++;
						}
					}
					string[] array = dataRow["ReggionPath"].ToString().Split(new char[]
					{
						','
					});
					string[] array2 = RegionHelper.GetFullRegion(Globals.ToNum(array[array.Length - 1]), "-").Split(new char[]
					{
						'-'
					});
					foreach (XmlNode xmlNode2 in xmlNode.SelectNodes("item"))
					{
						string text2 = string.Empty;
						string innerText2 = xmlNode2.SelectSingleNode("name").InnerText;
						string innerText3 = xmlNode2.SelectSingleNode("position").InnerText;
						string text3 = innerText3.Split(new char[]
						{
							':'
						})[0];
						string text4 = innerText3.Split(new char[]
						{
							':'
						})[1];
						string text5 = innerText3.Split(new char[]
						{
							':'
						})[2];
						string text6 = innerText3.Split(new char[]
						{
							':'
						})[3];
						string str = string.Concat(new string[]
						{
							text6,
							",",
							text5,
							",",
							text3,
							",",
							text4
						});
						string[] array3 = new string[]
						{
							"",
							"",
							""
						};
						if (shipper != null)
						{
							array3 = RegionHelper.GetFullRegion(shipper.RegionId, "-").Split(new char[]
							{
								'-'
							});
						}
						string text7 = string.Empty;
						string text8 = innerText2.Split(new char[]
						{
							'_'
						})[0];
						string key;
						switch (key = text8)
						{
						case "收货人-姓名":
							this.ShipTo = this.ShipTo + "'" + this.ReplaceString(dataRow["Receiver"].ToString()) + "',";
							if (!string.IsNullOrEmpty(dataRow["Receiver"].ToString().Trim()))
							{
								this.SizeShipTo = this.SizeShipTo + "'" + str + "',";
								continue;
							}
							this.SizeShipTo += "'0,0,0,0',";
							continue;
						case "收货人-电话":
							this.ShipTelPhone = this.ShipTelPhone + "'" + dataRow["Tel"].ToString() + "',";
							if (!string.IsNullOrEmpty(dataRow["Tel"].ToString().Trim()))
							{
								this.SizeShipTelPhone = this.SizeShipTelPhone + "'" + str + "',";
								continue;
							}
							this.SizeShipTelPhone += "'0,0,0,0',";
							continue;
						case "收货人-手机":
							this.ShipCellPhone = this.ShipCellPhone + "'" + dataRow["Tel"].ToString() + "',";
							if (!string.IsNullOrEmpty(dataRow["Tel"].ToString().Trim()))
							{
								this.SizeShipCellPhone = this.SizeShipCellPhone + "'" + str + "',";
								continue;
							}
							this.SizeShipCellPhone += "'0,0,0,0',";
							continue;
						case "收货人-地址":
							this.ShipAddress = this.ShipAddress + "'" + this.ReplaceString(dataRow["Address"].ToString()) + "',";
							if (!string.IsNullOrEmpty(dataRow["Address"].ToString().Trim()))
							{
								this.ShipSizeAddress = this.ShipSizeAddress + "'" + str + "',";
								continue;
							}
							this.ShipSizeAddress += "'0,0,0,0',";
							continue;
						case "收货人-地区1级":
							if (array2.Length > 0)
							{
								text2 = array2[0];
							}
							this.ShipProvince = this.ShipProvince + "'" + text2 + "',";
							if (!string.IsNullOrEmpty(text2.Trim()))
							{
								this.ShipSizeProvnce = this.ShipSizeProvnce + "'" + str + "',";
								continue;
							}
							this.ShipSizeProvnce += "'0,0,0,0',";
							continue;
						case "收货人-地区2级":
							text2 = string.Empty;
							if (array2.Length > 1)
							{
								text2 = array2[1];
							}
							this.ShipCity = this.ShipCity + "'" + text2 + "',";
							if (!string.IsNullOrEmpty(text2.Trim()))
							{
								this.ShipSizeCity = this.ShipSizeCity + "'" + str + "',";
								continue;
							}
							this.ShipSizeCity += "'0,0,0,0',";
							continue;
						case "目的地-地区":
							text2 = string.Empty;
							if (array2.Length > 1)
							{
								text2 = array2[1];
							}
							this.Destination = this.Destination + "'" + text2 + "',";
							if (!string.IsNullOrEmpty(text2.Trim()))
							{
								this.SizeDestination = this.SizeDestination + "'" + str + "',";
								continue;
							}
							this.SizeDestination += "'0,0,0,0',";
							continue;
						case "收货人-地区3级":
							text2 = string.Empty;
							if (array2.Length > 2)
							{
								text2 = array2[2];
							}
							this.ShipDistrict = this.ShipDistrict + "'" + text2 + "',";
							if (!string.IsNullOrEmpty(text2.Trim()))
							{
								this.ShipSizeDistrict = this.ShipSizeDistrict + "'" + str + "',";
								continue;
							}
							this.ShipSizeDistrict += "'0,0,0,0',";
							continue;
						case "送货-上门时间":
							this.ShipToDate = this.ShipToDate + "'" + dataRow["DeliveryTime"].ToString() + "',";
							if (!string.IsNullOrEmpty(dataRow["DeliveryTime"].ToString().Trim()))
							{
								this.SizeShipToDate = this.SizeShipToDate + "'" + str + "',";
								continue;
							}
							this.SizeShipToDate += "'0,0,0,0',";
							continue;
						case "订单-订单号":
							if (num > i)
							{
								System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
								for (int j = i; j < num + 1; j++)
								{
									stringBuilder.Append(";" + ((Globals.ToNum(dataTable.Rows[j]["Ptype"]) == 1) ? dataTable.Rows[j]["PrizeNums"].ToString().Remove(dataTable.Rows[j]["PrizeNums"].ToString().Length - 1) : (System.Convert.ToDateTime(dataTable.Rows[j]["WinTime"]).ToString("0:yyyy-MM-dd-") + dataTable.Rows[j]["LogId"].ToString())));
								}
								this.OrderId = this.OrderId + "'中奖编号：" + stringBuilder.ToString().Trim(new char[]
								{
									';'
								}) + "',";
							}
							else
							{
								this.OrderId = this.OrderId + "'中奖编号：" + ((Globals.ToNum(dataRow["Ptype"]) == 1) ? dataRow["PrizeNums"].ToString().Remove(dataRow["PrizeNums"].ToString().Length - 1) : (System.Convert.ToDateTime(dataRow["WinTime"]).ToString("yyyy-MM-dd-") + dataRow["LogId"].ToString())) + "',";
							}
							if (!string.IsNullOrEmpty(dataRow["LogId"].ToString().Trim()))
							{
								this.SizeOrderId = this.SizeOrderId + "'" + str + "',";
								continue;
							}
							this.SizeOrderId += "'0,0,0,0',";
							continue;
						case "订单-详情":
						{
							string text9 = string.Empty;
							text9 = string.Concat(new object[]
							{
								text9,
								"奖品名称：",
								dataRow["ProductName"],
								" "
							});
							text9 = text9.Replace(";", "");
							if (num > i)
							{
								for (int k = i + 1; k < num + 1; k++)
								{
									text9 = string.Concat(new object[]
									{
										text9,
										"奖品名称：",
										dataRow["ProductName"],
										" "
									});
									text9 = text9.Replace(";", "");
								}
							}
							text9 = text9.Trim();
							if (!string.IsNullOrEmpty(text9))
							{
								this.SizeitemInfos = this.SizeitemInfos + "'" + str + "',";
							}
							else
							{
								this.SizeitemInfos += "'0,0,0,0',";
							}
							this.ShipitemInfos = this.ShipitemInfos + "'" + this.ReplaceString(text9) + "',";
							continue;
						}
						case "发货人-姓名":
							this.ShipperNameLength++;
							this.ShipperName = this.ShipperName + "'" + this.ReplaceString(shipper.ShipperName) + "',";
							if (!string.IsNullOrEmpty(shipper.ShipperName.Trim()))
							{
								this.SizeShipperName = this.SizeShipperName + "'" + str + "',";
								continue;
							}
							this.SizeShipperName += "'0,0,0,0',";
							continue;
						case "发货人-电话":
							this.TelPhone = this.TelPhone + "'" + shipper.TelPhone + "',";
							if (!string.IsNullOrEmpty(shipper.TelPhone.Trim()))
							{
								this.SizeTelPhone = this.SizeTelPhone + "'" + str + "',";
								continue;
							}
							this.SizeTelPhone += "'0,0,0,0',";
							continue;
						case "发货人-手机":
							this.CellPhone = this.CellPhone + "'" + shipper.CellPhone + "',";
							if (!string.IsNullOrEmpty(shipper.CellPhone.Trim()))
							{
								this.SizeCellPhone = this.SizeCellPhone + "'" + str + "',";
								continue;
							}
							this.SizeCellPhone += "'0,0,0,0',";
							continue;
						case "发货人-邮编":
							this.Zipcode = this.Zipcode + "'" + shipper.Zipcode + "',";
							if (!string.IsNullOrEmpty(shipper.Zipcode.Trim()))
							{
								this.SizeZipcode = this.SizeZipcode + "'" + str + "',";
								continue;
							}
							this.SizeZipcode += "'0,0,0,0',";
							continue;
						case "发货人-地址":
							this.Address = this.Address + "'" + this.ReplaceString(shipper.Address) + "',";
							if (!string.IsNullOrEmpty(shipper.Address.Trim()))
							{
								this.SizeAddress = this.SizeAddress + "'" + str + "',";
								continue;
							}
							this.SizeAddress += "'0,0,0,0',";
							continue;
						case "发货人-地区1级":
							if (array3.Length > 0)
							{
								text7 = array3[0];
							}
							this.Province = this.Province + "'" + text7 + "',";
							if (!string.IsNullOrEmpty(text7.Trim()))
							{
								this.SizeProvnce = this.SizeProvnce + "'" + str + "',";
								continue;
							}
							this.SizeProvnce += "'0,0,0,0',";
							continue;
						case "发货人-地区2级":
							text7 += string.Empty;
							if (array3.Length > 1)
							{
								text7 = array3[1];
							}
							this.City = this.City + "'" + text7 + "',";
							if (!string.IsNullOrEmpty(text7.Trim()))
							{
								this.SizeCity = this.SizeCity + "'" + str + "',";
								continue;
							}
							this.SizeCity += "'0,0,0,0',";
							continue;
						case "始发地-地区":
							text7 += string.Empty;
							if (array3.Length > 1)
							{
								text7 = array3[1];
							}
							this.Departure = this.Departure + "'" + text7 + "',";
							if (!string.IsNullOrEmpty(text7.Trim()))
							{
								this.SizeDeparture = this.SizeDeparture + "'" + str + "',";
								continue;
							}
							this.SizeDeparture += "'0,0,0,0',";
							continue;
						case "发货人-地区3级":
							text7 += string.Empty;
							if (array3.Length > 2)
							{
								text7 = array3[2];
							}
							this.District = this.District + "'" + text7 + "',";
							if (!string.IsNullOrEmpty(text7.Trim()))
							{
								this.SizeDistrict = this.SizeDistrict + "'" + str + "',";
								continue;
							}
							this.SizeDistrict += "'0,0,0,0',";
							continue;
						case "网店名称":
							this.SiteName = this.SiteName + "'" + this.ReplaceString(this.siteSettings.SiteName) + "',";
							if (!string.IsNullOrEmpty(this.siteSettings.SiteName.Trim()))
							{
								this.SizeSiteName = this.SizeSiteName + "'" + str + "',";
								continue;
							}
							this.SizeSiteName += "'0,0,0,0',";
							continue;
						}
						if (text8 == "√")
						{
							this.SelContentLength++;
							this.SelContent += "'√',";
							this.SizeSelContent = this.SizeSelContent + "'" + str + "',";
						}
						else
						{
							string[] array4 = innerText2.Split(new char[]
							{
								'_'
							});
							if (array4.Length == 3 && array4[1] == "自定义内容")
							{
								this.SelfDefinedContentLength++;
								this.SelfDefinedContent = this.SelfDefinedContent + "'" + this.ReplaceString(array4[0]) + "',";
								this.SizeSelfDefinedContent = this.SizeSelfDefinedContent + "'" + str + "',";
							}
						}
					}
					i = num;
				}
				this.PrintPage(this.width, this.height);
				return;
			}
			this.ShowMsgToTarget("模版文件【" + text + "】丢失", false, "parent");
		}

		private string ReplaceString(string str)
		{
			return str.Replace("'", "＇").Replace("\n", " ").Replace("\r", "");
		}

		private string GetFormatTitle(object title)
		{
			string result = "-";
			if (title != System.DBNull.Value && title != null && !string.IsNullOrEmpty(title.ToString()))
			{
				result = title.ToString();
			}
			return result;
		}

		private void LoadData(string orderlist, string pidlist)
		{
			System.Data.DataSet prizeListByLogIDList = GameHelper.GetPrizeListByLogIDList(orderlist, pidlist);
			this.rptItemList.DataSource = prizeListByLogIDList;
			this.rptItemList.DataBind();
		}

		private void btnUpdateAddrdss_Click(object sender, System.EventArgs e)
		{
		}

		private void ddlShoperTag_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			this.LoadShipper();
		}

		private void LoadShipper()
		{
			ShippersInfo shipper = SalesHelper.GetShipper(this.ddlShoperTag.SelectedValue);
			if (shipper != null)
			{
				this.pnlEmptySender.Visible = false;
				this.pnlShipper.Visible = true;
				return;
			}
			this.pnlShipper.Visible = false;
			this.pnlEmptySender.Visible = true;
		}

		private void LoadTemplates()
		{
			System.Data.DataTable isUserExpressTemplates = SalesHelper.GetIsUserExpressTemplates();
			if (isUserExpressTemplates != null && isUserExpressTemplates.Rows.Count > 0)
			{
				this.ddlTemplates.Items.Add(new System.Web.UI.WebControls.ListItem("-请选择-", ""));
				foreach (System.Data.DataRow dataRow in isUserExpressTemplates.Rows)
				{
					this.ddlTemplates.Items.Add(new System.Web.UI.WebControls.ListItem(dataRow["ExpressName"].ToString(), dataRow["XmlFile"].ToString()));
				}
				this.pnlEmptyTemplates.Visible = false;
				this.pnlTemplates.Visible = true;
				return;
			}
			this.pnlEmptyTemplates.Visible = true;
			this.btnShowStyle = "none";
			this.pnlTemplates.Visible = false;
		}
	}
}
