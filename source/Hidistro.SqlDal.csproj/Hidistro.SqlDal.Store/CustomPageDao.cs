using Hidistro.Core;
using Hidistro.Core.Entities;
using Hidistro.Entities;
using Hidistro.Entities.Store;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Data;
using System.Data.Common;
using System.Text;

namespace Hidistro.SqlDal.Store
{
	public class CustomPageDao
	{
		private Database database;

		public CustomPageDao()
		{
			this.database = DatabaseFactory.CreateDatabase();
		}

		public DbQueryResult GetPages(CustomPageQuery query)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat(" Status={0} ", query.Status);
			if (!string.IsNullOrEmpty(query.Name))
			{
				stringBuilder.AppendFormat(" And Name LIKE '%{0}%'", DataHelper.CleanSearchString(query.Name));
			}
			if (query.Status.HasValue)
			{
				stringBuilder.AppendFormat(" And Status = {0}", query.Status.Value);
			}
			return DataHelper.PagingByRownumber(query.PageIndex, query.PageSize, query.SortBy, query.SortOrder, query.IsCount, "Hishop_CustomPage", "Id", stringBuilder.ToString(), "*");
		}

		public int Create(CustomPage page)
		{
			System.Data.Common.DbCommand sqlStringCommand = this.database.GetSqlStringCommand("INSERT INTO Hishop_CustomPage (Name, Status, FormalJson, DraftJson,TempIndexName,PageUrl,IsShowMenu,Details,CreateTime,PV, DraftName, DraftDetails, DraftPageUrl, DraftIsShowMenu) VALUES (@Name, @Status, @FormalJson, @DraftJson,@TempIndexName,@PageUrl,@IsShowMenu,@Details,@CreateTime,@PV, @DraftName, @DraftDetails, @DraftPageUrl, @DraftIsShowMenu) SELECT @@IDENTITY");
			this.database.AddInParameter(sqlStringCommand, "Name", System.Data.DbType.String, page.Name);
			this.database.AddInParameter(sqlStringCommand, "Status", System.Data.DbType.Int32, page.Status);
			this.database.AddInParameter(sqlStringCommand, "FormalJson", System.Data.DbType.String, page.FormalJson);
			this.database.AddInParameter(sqlStringCommand, "DraftJson", System.Data.DbType.String, page.DraftJson);
			this.database.AddInParameter(sqlStringCommand, "TempIndexName", System.Data.DbType.String, page.TempIndexName);
			this.database.AddInParameter(sqlStringCommand, "PageUrl", System.Data.DbType.String, page.PageUrl);
			this.database.AddInParameter(sqlStringCommand, "Details", System.Data.DbType.String, page.Details);
			this.database.AddInParameter(sqlStringCommand, "IsShowMenu", System.Data.DbType.Boolean, page.IsShowMenu);
			this.database.AddInParameter(sqlStringCommand, "CreateTime", System.Data.DbType.DateTime, page.CreateTime);
			this.database.AddInParameter(sqlStringCommand, "PV", System.Data.DbType.Int32, page.PV);
			this.database.AddInParameter(sqlStringCommand, "DraftName", System.Data.DbType.String, page.DraftName);
			this.database.AddInParameter(sqlStringCommand, "DraftDetails", System.Data.DbType.String, page.DraftDetails);
			this.database.AddInParameter(sqlStringCommand, "DraftPageUrl", System.Data.DbType.String, page.DraftPageUrl);
			this.database.AddInParameter(sqlStringCommand, "DraftIsShowMenu", System.Data.DbType.Boolean, page.DraftIsShowMenu);
			return Convert.ToInt32(this.database.ExecuteScalar(sqlStringCommand));
		}

		public CustomPage GetCustomPageByID(int id)
		{
			System.Data.Common.DbCommand sqlStringCommand = this.database.GetSqlStringCommand("SELECT * FROM Hishop_CustomPage WHERE Id = @Id");
			this.database.AddInParameter(sqlStringCommand, "Id", System.Data.DbType.Int32, id);
			CustomPage result = null;
			using (System.Data.IDataReader dataReader = this.database.ExecuteReader(sqlStringCommand))
			{
				result = ReaderConvert.ReaderToModel<CustomPage>(dataReader);
			}
			return result;
		}

		public CustomPage GetCustomPageByPath(string path)
		{
			System.Data.Common.DbCommand sqlStringCommand = this.database.GetSqlStringCommand("SELECT top 1 * FROM Hishop_CustomPage WHERE PageUrl = @PageUrl");
			this.database.AddInParameter(sqlStringCommand, "PageUrl", System.Data.DbType.String, path);
			CustomPage result = null;
			using (System.Data.IDataReader dataReader = this.database.ExecuteReader(sqlStringCommand))
			{
				result = ReaderConvert.ReaderToModel<CustomPage>(dataReader);
			}
			return result;
		}

		public CustomPage GetCustomDraftPageByPath(string path)
		{
			System.Data.Common.DbCommand sqlStringCommand = this.database.GetSqlStringCommand("SELECT top 1 * FROM Hishop_CustomPage WHERE DraftPageUrl = @DraftPageUrl");
			this.database.AddInParameter(sqlStringCommand, "DraftPageUrl", System.Data.DbType.String, path);
			CustomPage result = null;
			using (System.Data.IDataReader dataReader = this.database.ExecuteReader(sqlStringCommand))
			{
				result = ReaderConvert.ReaderToModel<CustomPage>(dataReader);
			}
			return result;
		}

		public bool DeletePage(int Id)
		{
			System.Data.Common.DbCommand sqlStringCommand = this.database.GetSqlStringCommand("DELETE FROM Hishop_CustomPage WHERE Id = @Id");
			this.database.AddInParameter(sqlStringCommand, "Id", System.Data.DbType.Int32, Id);
			return this.database.ExecuteNonQuery(sqlStringCommand) > 0;
		}

		public bool Update(CustomPage page)
		{
			System.Data.Common.DbCommand sqlStringCommand = this.database.GetSqlStringCommand("UPDATE Hishop_CustomPage SET Name = @Name, TempIndexName = @TempIndexName,PageUrl = @PageUrl,IsShowMenu=@IsShowMenu,Details=@Details,Status = @Status, FormalJson = @FormalJson, DraftJson = @DraftJson,PV=@PV,DraftName=@DraftName,DraftDetails=@DraftDetails,DraftPageUrl=@DraftPageUrl,DraftIsShowMenu=@DraftIsShowMenu WHERE Id = @Id");
			this.database.AddInParameter(sqlStringCommand, "Name", System.Data.DbType.String, page.Name);
			this.database.AddInParameter(sqlStringCommand, "Status", System.Data.DbType.Int32, page.Status);
			this.database.AddInParameter(sqlStringCommand, "FormalJson", System.Data.DbType.String, page.FormalJson);
			this.database.AddInParameter(sqlStringCommand, "DraftJson", System.Data.DbType.String, page.DraftJson);
			this.database.AddInParameter(sqlStringCommand, "Id", System.Data.DbType.Int32, page.Id);
			this.database.AddInParameter(sqlStringCommand, "TempIndexName", System.Data.DbType.String, page.TempIndexName);
			this.database.AddInParameter(sqlStringCommand, "IsShowMenu", System.Data.DbType.Boolean, page.IsShowMenu);
			this.database.AddInParameter(sqlStringCommand, "Details", System.Data.DbType.String, page.Details);
			this.database.AddInParameter(sqlStringCommand, "PageUrl", System.Data.DbType.String, page.PageUrl);
			this.database.AddInParameter(sqlStringCommand, "PV", System.Data.DbType.Int32, page.PV);
			this.database.AddInParameter(sqlStringCommand, "DraftName", System.Data.DbType.String, page.DraftName);
			this.database.AddInParameter(sqlStringCommand, "DraftDetails", System.Data.DbType.String, page.DraftDetails);
			this.database.AddInParameter(sqlStringCommand, "DraftPageUrl", System.Data.DbType.String, page.DraftPageUrl);
			this.database.AddInParameter(sqlStringCommand, "DraftIsShowMenu", System.Data.DbType.Boolean, page.DraftIsShowMenu);
			return this.database.ExecuteNonQuery(sqlStringCommand) > 0;
		}
	}
}
