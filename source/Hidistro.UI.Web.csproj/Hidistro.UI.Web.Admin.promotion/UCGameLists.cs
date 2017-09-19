using ASPNET.WebControls;
using ControlPanel.Promotions;
using Hidistro.Core.Entities;
using Hidistro.Entities.Promotions;
using Hidistro.UI.ControlPanel.Utility;
using Hidistro.UI.Web.Admin.Ascx;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Runtime.CompilerServices;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Hidistro.UI.Web.Admin.promotion
{
    public class UCGameLists : BaseUserControl
    {
        protected string isFinished = "0";

        private int pageSize = 10;

        private int pageIndex = 1;

        protected PageSize hrefPageSize;

        protected ucDateTimePicker calendarStartDate;

        protected ucDateTimePicker calendarEndDate;

        protected Button btnSeach;

        protected Button btnDel;

        protected Grid grdGameLists;

        protected Pager pager1;

        public GameType? PGameType
        {
            get;
            set;
        }

        public UCGameLists()
        {
        }

        private void BindData()
        {
            GameSearch gameSearch = new GameSearch()
            {
                SortBy = "GameId",
                PageIndex = this.pageIndex,
                PageSize = this.pageSize,
                GameType = new int?(Convert.ToInt32( this.PGameType.Value)),
                BeginTime = this.calendarStartDate.SelectedDate,
                EndTime = this.calendarEndDate.SelectedDate
            };
            GameSearch gameSearch1 = gameSearch;
            string str = this.isFinished;
            if (!string.IsNullOrEmpty(str))
            {
                gameSearch1.Status = str;
            }
            DbQueryResult gameListByView = GameHelper.GetGameListByView(gameSearch1);
            DataTable data = (DataTable)gameListByView.Data;
            this.grdGameLists.DataSource = data;
            this.grdGameLists.DataBind();
            this.pager1.TotalRecords = gameListByView.TotalRecords;
        }

        protected void btnDel_Click(object sender, EventArgs e)
        {
            List<int> nums = new List<int>();
            foreach (GridViewRow row in this.grdGameLists.Rows)
            {
                if (row.RowIndex < 0 || !(row.Cells[0].Controls[0] as CheckBox).Checked)
                {
                    continue;
                }
                nums.Add(int.Parse(this.grdGameLists.DataKeys[row.RowIndex].Value.ToString()));
            }
            if (nums.Count <= 0)
            {
                this.ShowMsg("请至少选择一条要删除的数据！", false);
                return;
            }
            try
            {
                if (!GameHelper.Delete(nums.ToArray()))
                {
                    throw new Exception("操作失败！");
                }
                this.ShowMsg("操作成功！", true);
                this.BindData();
            }
            catch (Exception exception)
            {
                this.ShowMsg(exception.Message, false);
            }
        }

        protected void btnSeach_Click(object sender, EventArgs e)
        {
            this.BindData();
        }

        protected string GetDetialUrl(string gameId)
        {
            GameType? pGameType = this.PGameType;
            GameType valueOrDefault = pGameType.GetValueOrDefault();
            if (pGameType.HasValue)
            {
                switch (valueOrDefault)
                {
                    case GameType.幸运大转盘:
                        {
                            return string.Format("EditGame.aspx?action=deital&gameId={0}", gameId);
                        }
                    case GameType.疯狂砸金蛋:
                        {
                            return string.Format("EditGameEgg.aspx?action=deital&gameId={0}", gameId);
                        }
                    case GameType.好运翻翻看:
                        {
                            return string.Format("EditGameXingYun.aspx?action=deital&gameId={0}", gameId);
                        }
                    case GameType.大富翁:
                        {
                            return string.Format("EditGameDaFuWen.aspx?action=deital&gameId={0}", gameId);
                        }
                    case GameType.刮刮乐:
                        {
                            return string.Format("EditGameGuaGuaLe.aspx?action=deital&gameId={0}", gameId);
                        }
                }
            }
            return "";
        }

        protected string GetEditUrl(string gameId, string sstartTime)
        {
            DateTime now = DateTime.Now;
            DateTime.TryParse(sstartTime, out now);
            string empty = string.Empty;
            string str = "编辑";
            if (now <= DateTime.Now)
            {
                empty = "action=deital&";
                str = "详情";
            }
            GameType? pGameType = this.PGameType;
            GameType valueOrDefault = pGameType.GetValueOrDefault();
            if (pGameType.HasValue)
            {
                switch (valueOrDefault)
                {
                    case GameType.幸运大转盘:
                        {
                            string[] strArrays = new string[] { "<a href='EditGame.aspx?", empty, "gameId={0}' class='btn btn-primary resetSize'>", str, "</a>" };
                            return string.Format(string.Concat(strArrays), gameId);
                        }
                    case GameType.疯狂砸金蛋:
                        {
                            string[] strArrays1 = new string[] { "<a href='EditGameEgg.aspx?", empty, "gameId={0}' class='btn btn-primary resetSize'>", str, "</a>" };
                            return string.Format(string.Concat(strArrays1), gameId);
                        }
                    case GameType.好运翻翻看:
                        {
                            string[] strArrays2 = new string[] { "<a href='EditGameXingYun.aspx?", empty, "gameId={0}' class='btn btn-primary resetSize'>", str, "</a>" };
                            return string.Format(string.Concat(strArrays2), gameId);
                        }
                    case GameType.大富翁:
                        {
                            string[] strArrays3 = new string[] { "<a href='EditGameDaFuWen.aspx?", empty, "gameId={0}' class='btn btn-primary resetSize'>", str, "</a>" };
                            return string.Format(string.Concat(strArrays3), gameId);
                        }
                    case GameType.刮刮乐:
                        {
                            string[] strArrays4 = new string[] { "<a href='EditGameGuaGuaLe.aspx?", empty, "gameId={0}' class='btn btn-primary resetSize'>", str, "</a>" };
                            return string.Format(string.Concat(strArrays4), gameId);
                        }
                }
            }
            return "";
        }

        protected string GetLimit(object limitEveryDay, object maximumDailyLimit)
        {
            StringBuilder stringBuilder = new StringBuilder();
            int num = (int)limitEveryDay;
            int num1 = (int)maximumDailyLimit;
            if (num != 0 || num1 != 0)
            {
                if (num != 0)
                {
                    stringBuilder.AppendFormat("每天参与{0}次", num);
                }
                stringBuilder.Append("<br/>");
                if (num1 != 0)
                {
                    stringBuilder.AppendFormat("参与上限{0}次", num1);
                }
            }
            else
            {
                stringBuilder.Append("不限次");
            }
            return stringBuilder.ToString();
        }

        protected string GetPrizeListsUrl(string gameId)
        {
            GameType? pGameType = this.PGameType;
            GameType valueOrDefault = pGameType.GetValueOrDefault();
            if (pGameType.HasValue)
            {
                switch (valueOrDefault)
                {
                    case GameType.幸运大转盘:
                        {
                            return string.Format("PrizeLists.aspx?gameId={0}", gameId);
                        }
                    case GameType.疯狂砸金蛋:
                        {
                            return string.Format("PrizeListsEgg.aspx?gameId={0}", gameId);
                        }
                    case GameType.好运翻翻看:
                        {
                            return string.Format("PrizeListsHaoYun.aspx?gameId={0}", gameId);
                        }
                    case GameType.大富翁:
                        {
                            return string.Format("PrizeListsDaFuWen.aspx?gameId={0}", gameId);
                        }
                    case GameType.刮刮乐:
                        {
                            return string.Format("PrizeListsGuaGuaLe.aspx?gameId={0}", gameId);
                        }
                }
            }
            return "";
        }

        protected void grdGameLists_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowIndex >= 0)
            {
                GameStatus gameStatu = (GameStatus)int.Parse(((HiddenField)e.Row.FindControl("hfStatus")).Value);
                string value = ((HiddenField)e.Row.FindControl("hfBeginTime")).Value;
                string str = ((HiddenField)e.Row.FindControl("hfEndTime")).Value;
                Convert.ToDateTime(value);
                Convert.ToDateTime(str);
                if (gameStatu == GameStatus.正常)
                {
                    ((Button)e.Row.FindControl("lkDelete")).Visible = false;
                    return;
                }
                ((Button)e.Row.FindControl("FinishBtn")).Visible = false;
            }
        }

        protected void grdGameLists_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            string str = this.grdGameLists.DataKeys[e.RowIndex].Value.ToString();
            if (!string.IsNullOrEmpty(str))
            {
                try
                {
                    int[] numArray = new int[] { int.Parse(str) };
                    if (!GameHelper.Delete(numArray))
                    {
                        throw new Exception("操作失败！");
                    }
                    this.ShowMsg("操作成功！", true);
                    this.BindData();
                }
                catch (Exception exception)
                {
                    this.ShowMsg(exception.Message, false);
                }
            }
        }

        protected void grdGameLists_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            string str = this.grdGameLists.DataKeys[e.RowIndex].Value.ToString();
            if (!string.IsNullOrEmpty(str))
            {
                try
                {
                    if (!GameHelper.UpdateStatus(int.Parse(str), GameStatus.结束))
                    {
                        throw new Exception("操作失败！");
                    }
                    this.ShowMsg("操作成功！", true);
                    this.BindData();
                }
                catch (Exception exception)
                {
                    this.ShowMsg(exception.Message, false);
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.isFinished = base.Request.QueryString["isFinished"];
            if (string.IsNullOrEmpty(this.isFinished))
            {
                this.isFinished = "0";
            }
            try
            {
                this.pageIndex = int.Parse(base.Request["pageindex"]);
            }
            catch (Exception exception)
            {
                this.pageIndex = 1;
            }
            try
            {
                this.pageSize = int.Parse(base.Request.QueryString["pagesize"]);
            }
            catch (Exception exception1)
            {
                this.pageSize = 10;
            }
            this.pager1.DefaultPageSize = this.pageSize;
            if (!base.IsPostBack)
            {
                this.BindData();
            }
        }
    }
}