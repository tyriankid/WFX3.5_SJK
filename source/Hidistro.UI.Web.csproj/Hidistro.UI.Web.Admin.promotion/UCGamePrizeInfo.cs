using ControlPanel.Promotions;
using Hidistro.ControlPanel.Promotions;
using Hidistro.Entities.Promotions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Hidistro.UI.Web.Admin.promotion
{
    public class UCGamePrizeInfo : UserControl
    {
        private IList<GamePrizeInfo> _prizeLists;

        private List<ListItem> _couponList = new List<ListItem>();

        protected int prizeTypeValue0;

        protected int prizeTypeValue1;

        protected int prizeTypeValue2;

        protected int prizeTypeValue3;

        protected HiddenField hfGameId;

        protected HiddenField hfIndex;

        protected TextBox txtPrizeRate;

        protected TextBox txtNotPrzeDescription;

        protected IList<ListItem> CouponIdList
        {
            get
            {
                return this._couponList;
            }
        }

        public string NotPrzeDescription
        {
            get
            {
                return this.txtNotPrzeDescription.Text.Trim();
            }
            set
            {
                this.txtNotPrzeDescription.Text = value;
            }
        }

        public IList<GamePrizeInfo> PrizeLists
        {
            get
            {
                this.GetDate();
                return this._prizeLists;
            }
            set
            {
                this._prizeLists = value;
            }
        }

        public float PrizeRate
        {
            get
            {
                return float.Parse(this.txtPrizeRate.Text);
            }
            set
            {
                this.txtPrizeRate.Text = value.ToString("f2");
            }
        }

        public UCGamePrizeInfo()
        {
        }

        private void BindDdlCouponId()
        {
            DataTable unFinishedCoupon = CouponHelper.GetUnFinishedCoupon(DateTime.Now, new CouponType?(CouponType.活动赠送));
            if (unFinishedCoupon != null)
            {
                foreach (DataRow row in unFinishedCoupon.Rows)
                {
                    List<ListItem> listItems = this._couponList;
                    ListItem listItem = new ListItem()
                    {
                        Text = row["CouponName"].ToString(),
                        Value = row["CouponId"].ToString()
                    };
                    listItems.Add(listItem);
                }
            }
        }

        private bool GetDate()
        {
            if (!this.Page.IsPostBack)
            {
                return true;
            }
            this._prizeLists = new List<GamePrizeInfo>();
            bool flag = true;
            switch (int.Parse(this.hfIndex.Value))
            {
                case 1:
                    {
                        this._prizeLists.Add(this.GetModel(PrizeGrade.一等奖));
                        break;
                    }
                case 2:
                    {
                        this._prizeLists.Add(this.GetModel(PrizeGrade.一等奖));
                        this._prizeLists.Add(this.GetModel(PrizeGrade.二等奖));
                        break;
                    }
                case 3:
                    {
                        this._prizeLists.Add(this.GetModel(PrizeGrade.一等奖));
                        this._prizeLists.Add(this.GetModel(PrizeGrade.二等奖));
                        this._prizeLists.Add(this.GetModel(PrizeGrade.三等奖));
                        break;
                    }
                case 4:
                    {
                        this._prizeLists.Add(this.GetModel(PrizeGrade.一等奖));
                        this._prizeLists.Add(this.GetModel(PrizeGrade.二等奖));
                        this._prizeLists.Add(this.GetModel(PrizeGrade.三等奖));
                        this._prizeLists.Add(this.GetModel(PrizeGrade.四等奖));
                        break;
                    }
            }
            return flag;
        }

        protected string GetGameMenu()
        {
            StringBuilder stringBuilder = new StringBuilder();
            int num = 0;
            if (!int.TryParse(base.Request.QueryString["gameId"], out num))
            {
                stringBuilder.AppendFormat("<li class=\"{0}\">{1}</li>", "active", "一等奖");
                this.hfIndex.Value = "1";
            }
            else
            {
                IList<GamePrizeInfo> gamePrizeListsByGameId = GameHelper.GetGamePrizeListsByGameId(num);
                if (gamePrizeListsByGameId == null || gamePrizeListsByGameId.Count<GamePrizeInfo>() <= 0)
                {
                    stringBuilder.AppendFormat("<li class=\"{0}\">{1}</li>", "active", "一等奖");
                    this.hfIndex.Value = "1";
                }
                else
                {
                    int num1 = 0;
                    foreach (GamePrizeInfo gamePrizeInfo in gamePrizeListsByGameId)
                    {
                        string str = "";
                        if (num1 == 0)
                        {
                            str = "active";
                        }
                        if (num1 <= 0)
                        {
                            stringBuilder.AppendFormat("<li class=\"{0}\" id=\"li{2}\" lival=\"{2}\">{1}</li>", str, gamePrizeInfo.PrizeName, gamePrizeInfo.PrizeId);
                        }
                        else
                        {
                            stringBuilder.AppendFormat("<li class=\"{0}\" id=\"li{2}\" lival=\"{2}\">{1}<i class='' onclick='DelPrize(this)'></i></li>", str, gamePrizeInfo.PrizeName, gamePrizeInfo.PrizeId);
                        }
                        num1++;
                    }
                }
            }
            return stringBuilder.ToString();
        }

        private GamePrizeInfo GetModel(PrizeGrade prizeGrade)
        {
            GamePrizeInfo gamePrizeInfo = new GamePrizeInfo()
            {
                PrizeGrade = prizeGrade
            };
            int num = (int)prizeGrade;
            PrizeType prizeType = PrizeType.赠送积分;
            try
            {
                prizeType = (PrizeType)Enum.Parse(typeof(PrizeType), base.Request[string.Format("prizeType_{0}", num)]);
            }
            catch (Exception exception)
            {
            }
            gamePrizeInfo.PrizeType = prizeType;
            switch (prizeType)
            {
                case PrizeType.赠送积分:
                    {
                        try
                        {
                            gamePrizeInfo.GivePoint = int.Parse(base.Request[string.Format("txtGivePoint{0}", num)]);
                            break;
                        }
                        catch (Exception exception1)
                        {
                            throw new Exception(string.Format("{0}的赠送积分格式不对!", prizeGrade.ToString()));
                        }
                        break;
                    }
                case PrizeType.赠送优惠券:
                    {
                        gamePrizeInfo.GiveCouponId = base.Request[string.Format("seletCouponId{0}", num)];
                        break;
                    }
                case PrizeType.赠送商品:
                    {
                        gamePrizeInfo.GiveShopBookId = base.Request[string.Format("txtShopbookId{0}", num)];
                        gamePrizeInfo.GriveShopBookPicUrl = base.Request[string.Format("txtProductPic{0}", num)];
                        break;
                    }
                case PrizeType.其他奖品:
                    {
                        try
                        {
                            gamePrizeInfo.IsLogistics = (base.Request[string.Format("ckbNeed_{0}", num)] == "on" ? 1 : 0);
                        }
                        catch (Exception exception2)
                        {
                            throw new Exception(string.Format("{0}的是否配送格式不对!", prizeGrade.ToString()));
                        }
                        gamePrizeInfo.PrizeImage = base.Request[string.Format("hiddPrizeImage{0}", num)];
                        break;
                    }
            }
            try
            {
                gamePrizeInfo.Prize = base.Request[string.Format("txtPrize{0}", num)];
            }
            catch (Exception exception3)
            {
                throw new Exception(string.Format("{0}的奖品名称格式不对!", prizeGrade.ToString()));
            }
            try
            {
                gamePrizeInfo.PrizeCount = int.Parse(base.Request[string.Format("txtPrizeCount{0}", num)]);
            }
            catch (Exception exception4)
            {
                throw new Exception(string.Format("{0}的奖品数量格式不对!", prizeGrade.ToString()));
            }
            try
            {
                gamePrizeInfo.PrizeId = int.Parse(base.Request[string.Format("prizeInfoId{0}", num)]);
            }
            catch (Exception exception5)
            {
                gamePrizeInfo.PrizeId = 0;
            }
            try
            {
                gamePrizeInfo.GameId = int.Parse(base.Request[string.Format("prizeGameId{0}", num)]);
            }
            catch (Exception exception6)
            {
                gamePrizeInfo.GameId = 0;
            }
            return gamePrizeInfo;
        }

        protected string GetPrizeInfoHtml(PrizeGrade prizeGrade, GamePrizeInfo model)
        {
            StringBuilder stringBuilder = new StringBuilder();
            int prizeId = 0;
            if (model != null)
            {
                prizeId = model.PrizeId;
            }
            stringBuilder.Append(string.Concat("<div class='tabContent' id='div", prizeId, "'>"));
            stringBuilder.Append("<div class='form-horizontal clearfix'>");
            stringBuilder.Append("<div class='form-group setmargin'>");
            stringBuilder.Append("<label class='col-xs-3 pad resetSize control-label'><em>*</em>&nbsp;&nbsp;奖品类别：</label>");
            stringBuilder.Append("<div class='form-inline col-xs-9'>");
            stringBuilder.Append("<div class='resetradio selectradio pt3' >");
            stringBuilder.Append("<label class=\"mr20\">");
            if (model == null)
            {
                stringBuilder.AppendFormat(" <input type=\"radio\" id=\"rd{0}_0\" name=\"prizeType_{0}\" checked=\"checked\" value=\"0\" />赠送积分</label>", (int)prizeGrade);
                stringBuilder.AppendFormat(" <label class=\"mr20\"> <input type=\"radio\" id=\"rd{0}_1\" name=\"prizeType_{0}\" value=\"1\" />赠送优惠券</label>", (int)prizeGrade);
                stringBuilder.AppendFormat(" <label class=\"mr20\"> <input type=\"radio\" id=\"rd{0}_2\" name=\"prizeType_{0}\" value=\"2\" />赠送商品</label>", (int)prizeGrade);
                stringBuilder.AppendFormat(" <label class=\"mr20\"> <input type=\"radio\" id=\"rd{0}_3\" name=\"prizeType_{0}\" value=\"3\" />其他奖品</label>", (int)prizeGrade);
            }
            else
            {
                stringBuilder.AppendFormat(" <input type=\"radio\" id=\"rd{0}_0\" name=\"prizeType_{0}\" {1} value=\"0\" />赠送积分</label>", (int)prizeGrade, (model.PrizeType == PrizeType.赠送积分 ? "checked=\"checked\"" : ""));
                stringBuilder.AppendFormat(" <label class=\"mr20\"> <input type=\"radio\" id=\"rd{0}_1\" name=\"prizeType_{0}\" {1} value=\"1\" />赠送优惠券</label>", (int)prizeGrade, (model.PrizeType == PrizeType.赠送优惠券 ? "checked=\"checked\"" : ""));
                stringBuilder.AppendFormat(" <label class=\"mr20\"> <input type=\"radio\" id=\"rd{0}_2\" name=\"prizeType_{0}\" {1} value=\"2\" />赠送商品</label>", (int)prizeGrade, (model.PrizeType == PrizeType.赠送商品 ? "checked=\"checked\"" : ""));
                stringBuilder.AppendFormat(" <label class=\"mr20\"> <input type=\"radio\" id=\"rd{0}_3\" name=\"prizeType_{0}\" {1} value=\"3\" />其他奖品</label>", (int)prizeGrade, (model.PrizeType == PrizeType.其他奖品 ? "checked=\"checked\"" : ""));
                stringBuilder.AppendFormat("<input type=\"hidden\" id=\"prizeTypeValue{0}\" value=\"{1}\" />", (int)prizeGrade, (int)model.PrizeType);
                stringBuilder.AppendFormat("<input type=\"hidden\" name=\"prizeInfoId{0}\" value=\"{1}\" />", (int)prizeGrade, model.PrizeId);
                stringBuilder.AppendFormat("<input type=\"hidden\" name=\"prizeGameId{0}\" value=\"{1}\" />", (int)prizeGrade, model.GameId);
            }
            stringBuilder.Append(" </div></div></div>");
            stringBuilder.Append("<div class=\"form-group setmargin\" style=\"display:normal\">");
            stringBuilder.Append(" <label class=\"col-xs-3 pad resetSize control-label\" for=\"Prize\"><em>*</em>&nbsp;&nbsp;奖品名称：</label> <div class=\"form-inline col-xs-9\">");
            if (model == null)
            {
                stringBuilder.AppendFormat("<input type=\"text\" name=\"txtPrize{0}\" id=\"txtPrize{0}\" class=\"form-control resetSize\" value=\"\"/>", (int)prizeGrade);
            }
            else
            {
                stringBuilder.AppendFormat("<input type=\"text\" name=\"txtPrize{0}\" id=\"txtPrize{0}\" class=\"form-control resetSize\" value=\"{1}\"/>", (int)prizeGrade, model.Prize);
            }
            stringBuilder.Append("</div></div>");
            if (model == null || model.PrizeType != PrizeType.赠送积分)
            {
                stringBuilder.Append(" <div class=\"form-group setmargin give giveint\">");
            }
            else
            {
                stringBuilder.Append(" <div class=\"form-group setmargin give giveint\"  style=\"display:normal\">");
            }
            stringBuilder.Append(" <label class=\"col-xs-3 pad resetSize control-label\" for=\"pausername\"><em>*</em>&nbsp;&nbsp");
            stringBuilder.Append("赠送积分：</label> <div class=\"form-inline col-xs-9\">");
            if (model == null)
            {
                stringBuilder.AppendFormat(" <input type=\"text\" name=\"txtGivePoint{0}\" id=\"txtGivePoint{0}\" class=\"form-control resetSize\" value=\"0\" />", (int)prizeGrade);
            }
            else
            {
                stringBuilder.AppendFormat(" <input type=\"text\" name=\"txtGivePoint{0}\" id=\"txtGivePoint{0}\" class=\"form-control resetSize\" value=\"{1}\" />", (int)prizeGrade, model.GivePoint);
            }
            if (model == null || model.PrizeType != PrizeType.赠送优惠券)
            {
                stringBuilder.Append(" </div> </div><div class=\"form-group setmargin give givecop\">");
            }
            else
            {
                stringBuilder.Append(" </div> </div><div class=\"form-group setmargin give givecop\" style=\"display:normal\">");
            }
            stringBuilder.Append(" <label class=\"col-xs-3 pad resetSize control-label\" for=\"pausername\"><em>*</em>&nbsp;&nbsp;赠送优惠券：</label> <div class=\"form-inline col-xs-9\">");
            stringBuilder.AppendFormat(" <select name=\"seletCouponId{0}\" id=\"seletCouponId{0}\" class=\"form-control resetSize\">", (int)prizeGrade);
            if (model == null)
            {
                foreach (ListItem couponIdList in this.CouponIdList)
                {
                    stringBuilder.AppendFormat(" <option value=\"{0}\">{1}</option>", couponIdList.Value, couponIdList.Text);
                }
            }
            else
            {
                foreach (ListItem listItem in this.CouponIdList)
                {
                    if (!string.Equals(model.GiveCouponId, listItem.Value))
                    {
                        stringBuilder.AppendFormat(" <option value=\"{0}\">{1}</option>", listItem.Value, listItem.Text);
                    }
                    else
                    {
                        stringBuilder.AppendFormat(" <option value=\"{0}\" selected=\"selected\">{1}</option>", listItem.Value, listItem.Text);
                    }
                }
            }
            stringBuilder.Append(" </select> </div>  </div> ");
            if (model == null || model.PrizeType != PrizeType.赠送商品)
            {
                stringBuilder.Append("<div class=\"form-group setmargin give giveshop\">");
            }
            else
            {
                stringBuilder.Append("<div class=\"form-group setmargin give giveshop\" style=\"display:normal\">");
            }
            stringBuilder.Append("<label class=\"col-xs-3 pad resetSize control-label\" for=\"pausername\"><em>*</em>&nbsp;&nbsp;赠送商品：</label>");
            stringBuilder.Append("<div class=\"form-inline col-xs-9\"><div class=\"pt3\">");
            if (model == null)
            {
                stringBuilder.AppendFormat("<img id=\"imgProduct{0}\" style=\"width:30px; height:30px;\" name=\"imgProduct{0}\" src=\"../images/u100.png\" onclick=\"SelectShopBookId({0});\" />", (int)prizeGrade);
                stringBuilder.AppendFormat("<input type=\"hidden\" name=\"txtShopbookId{0}\" id=\"txtShopbookId{0}\" />", (int)prizeGrade);
                stringBuilder.AppendFormat("<input type=\"hidden\" id=\"txtProductPic{0}\" name=\"txtProductPic{0}\"  />", (int)prizeGrade);
            }
            else
            {
                stringBuilder.AppendFormat("<img id=\"imgProduct{0}\" style=\"width:30px; height:30px;\" name=\"imgProduct{0}\"  src=\"{1}\"onclick=\"SelectShopBookId({0});\" />", (int)prizeGrade, (string.IsNullOrEmpty(model.GriveShopBookPicUrl) ? "../images/u100.png" : model.GriveShopBookPicUrl));
                stringBuilder.AppendFormat("<input type=\"hidden\" name=\"txtShopbookId{0}\" id=\"txtShopbookId{0}\"  value=\"{1}\" />", (int)prizeGrade, model.GiveShopBookId);
                stringBuilder.AppendFormat("<input type=\"hidden\" id=\"txtProductPic{0}\" name=\"txtProductPic{0}\"  value=\"{1}\" />", (int)prizeGrade, (string.IsNullOrEmpty(model.GriveShopBookPicUrl) ? "../images/u100.png" : model.GriveShopBookPicUrl));
            }
            stringBuilder.Append("</div> </div></div>");
            if (model == null || model.PrizeType != PrizeType.其他奖品)
            {
                stringBuilder.Append("<div class=\"form-group setmargin give other\">");
            }
            else
            {
                stringBuilder.Append("<div class=\"form-group setmargin give other\" style=\"display:normal\">");
            }
            stringBuilder.Append("<label class=\"col-xs-3 pad resetSize control-label\" for=\"pausername\"><em>*</em>&nbsp;&nbsp;是否配送：</label>");
            stringBuilder.Append("<div class=\"form-inline col-xs-9\"><div class=\"pt3 resetradio mb5 pt3 allradio\">");
            if (model == null)
            {
                stringBuilder.AppendFormat("<label class=\"mr20\"> <input type=\"checkbox\" id=\"ckbNeed_{0}\" name=\"ckbNeed_{0}\" >是，需要配送</label>", (int)prizeGrade);
            }
            else
            {
                stringBuilder.AppendFormat("<label class=\"mr20\"> <input type=\"checkbox\" id=\"ckbNeed_{0}\" name=\"ckbNeed_{0}\"  {1}>是，需要配送</label>", (int)prizeGrade, (model.IsLogistics == 1 ? "checked" : ""));
            }
            stringBuilder.Append("</div> </div></div>");
            stringBuilder.Append("<div class=\"form-group setmargin\">");
            stringBuilder.Append(" <label class=\"col-xs-3 pad resetSize control-label\" for=\"pausername\"><em>*</em>&nbsp;&nbsp;奖品数量：</label> <div class=\"form-inline col-xs-9\">");
            if (model == null)
            {
                stringBuilder.AppendFormat("<input type=\"text\" name=\"txtPrizeCount{0}\" id=\"txtPrizeCount{0}\" class=\"form-control resetSize\" value=\"1\"/>", (int)prizeGrade);
            }
            else
            {
                stringBuilder.AppendFormat("<input type=\"text\" name=\"txtPrizeCount{0}\" id=\"txtPrizeCount{0}\" class=\"form-control resetSize\" value=\"{1}\"/>", (int)prizeGrade, model.PrizeCount);
            }
            stringBuilder.Append("  <small>奖品数量为0时不设此奖项</small> </div> </div>");
            if (model == null || model.PrizeType != PrizeType.其他奖品)
            {
                stringBuilder.Append("<div class=\"form-group setmargin give other\">");
            }
            else
            {
                stringBuilder.Append("<div class=\"form-group setmargin give other\" style=\"display:normal\">");
            }
            stringBuilder.Append("<label class=\"col-xs-3 pad resetSize control-label\" for=\"PrizeImage\"><em></em>&nbsp;&nbsp;奖品图片：</label>");
            stringBuilder.Append("<div class=\"form-inline col-xs-9\"><div class=\"pt3\" style=\"vertical-align:bottom;\">");
            if (model == null)
            {
                stringBuilder.AppendFormat("<img id=\"PrizeImage{0}\" style=\"width:60px; height:60px;\" name=\"PrizeImage{0}\" src=\"../images/u100.png\" onclick=\"SelectPrizeImage({0});\" />  <div style=\"margin-left:70px\">仅支持jpg、 png、gif，尺寸60*60px,不超过1M</div> ", (int)prizeGrade);
                stringBuilder.AppendFormat("<input type=\"hidden\" id=\"hiddPrizeImage{0}\" name=\"hiddPrizeImage{0}\"  />", (int)prizeGrade);
            }
            else
            {
                stringBuilder.AppendFormat("<img id=\"PrizeImage{0}\" style=\"width:60px; height:60px;\" name=\"PrizeImage{0}\"  src=\"{1}\"onclick=\"SelectPrizeImage({0});\" />  <div style=\"margin-left:70px\">仅支持jpg、 png、gif，尺寸60*60px,不超过1M  </div>", (int)prizeGrade, (string.IsNullOrEmpty(model.PrizeImage) ? "../images/u100.png" : model.PrizeImage));
                stringBuilder.AppendFormat("<input type=\"hidden\" id=\"hiddPrizeImage{0}\" name=\"hiddPrizeImage{0}\"  value=\"{1}\" />", (int)prizeGrade, (string.IsNullOrEmpty(model.PrizeImage) ? "../images/u100.png" : model.PrizeImage));
            }
            stringBuilder.Append("</div> </div></div>");
            stringBuilder.Append("</div></div>");
            return stringBuilder.ToString();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.Page.IsPostBack)
            {
                int num = 0;
                if (int.TryParse(base.Request.QueryString["gameId"], out num))
                {
                    this.hfGameId.Value = num.ToString();
                    this.hfIndex.Value = (this.PrizeLists.Count<GamePrizeInfo>() == 0 ? "1" : this.PrizeLists.Count<GamePrizeInfo>().ToString());
                    GameInfo gameInfoById = GameHelper.GetGameInfoById(num);
                    if (gameInfoById != null)
                    {
                        TextBox textBox = this.txtPrizeRate;
                        float prizeRate = gameInfoById.PrizeRate;
                        textBox.Text = prizeRate.ToString("f2").Replace(".00", "");
                    }
                }
                this.BindDdlCouponId();
            }
        }

        protected string PrizeInfoHtml()
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (this.PrizeLists == null)
            {
                stringBuilder.Append(this.GetPrizeInfoHtml(PrizeGrade.一等奖, null));
            }
            else
            {
                switch (this.PrizeLists.Count<GamePrizeInfo>())
                {
                    case 0:
                        {
                            stringBuilder.Append(this.GetPrizeInfoHtml(PrizeGrade.一等奖, null));
                            this.hfIndex.Value = "1";
                            break;
                        }
                    case 1:
                        {
                            stringBuilder.Append(this.GetPrizeInfoHtml(PrizeGrade.一等奖, this.PrizeLists.FirstOrDefault<GamePrizeInfo>((GamePrizeInfo p) => p.PrizeGrade == PrizeGrade.一等奖)));
                            break;
                        }
                    case 2:
                        {
                            stringBuilder.Append(this.GetPrizeInfoHtml(PrizeGrade.一等奖, this.PrizeLists.FirstOrDefault<GamePrizeInfo>((GamePrizeInfo p) => p.PrizeGrade == PrizeGrade.一等奖)));
                            stringBuilder.Append(this.GetPrizeInfoHtml(PrizeGrade.二等奖, this.PrizeLists.FirstOrDefault<GamePrizeInfo>((GamePrizeInfo p) => p.PrizeGrade == PrizeGrade.二等奖)));
                            break;
                        }
                    case 3:
                        {
                            stringBuilder.Append(this.GetPrizeInfoHtml(PrizeGrade.一等奖, this.PrizeLists.FirstOrDefault<GamePrizeInfo>((GamePrizeInfo p) => p.PrizeGrade == PrizeGrade.一等奖)));
                            stringBuilder.Append(this.GetPrizeInfoHtml(PrizeGrade.二等奖, this.PrizeLists.FirstOrDefault<GamePrizeInfo>((GamePrizeInfo p) => p.PrizeGrade == PrizeGrade.二等奖)));
                            stringBuilder.Append(this.GetPrizeInfoHtml(PrizeGrade.三等奖, this.PrizeLists.FirstOrDefault<GamePrizeInfo>((GamePrizeInfo p) => p.PrizeGrade == PrizeGrade.三等奖)));
                            break;
                        }
                    case 4:
                        {
                            stringBuilder.Append(this.GetPrizeInfoHtml(PrizeGrade.一等奖, this.PrizeLists.FirstOrDefault<GamePrizeInfo>((GamePrizeInfo p) => p.PrizeGrade == PrizeGrade.一等奖)));
                            stringBuilder.Append(this.GetPrizeInfoHtml(PrizeGrade.二等奖, this.PrizeLists.FirstOrDefault<GamePrizeInfo>((GamePrizeInfo p) => p.PrizeGrade == PrizeGrade.二等奖)));
                            stringBuilder.Append(this.GetPrizeInfoHtml(PrizeGrade.三等奖, this.PrizeLists.FirstOrDefault<GamePrizeInfo>((GamePrizeInfo p) => p.PrizeGrade == PrizeGrade.三等奖)));
                            stringBuilder.Append(this.GetPrizeInfoHtml(PrizeGrade.四等奖, this.PrizeLists.FirstOrDefault<GamePrizeInfo>((GamePrizeInfo p) => p.PrizeGrade == PrizeGrade.四等奖)));
                            break;
                        }
                }
            }
            return stringBuilder.ToString();
        }
    }
}