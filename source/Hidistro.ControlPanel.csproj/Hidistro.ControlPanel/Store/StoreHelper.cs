using Hidistro.Core;
using Hidistro.Entities.Promotions;
using Hidistro.SqlDal;
using Hidistro.SqlDal.Promotions;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Web;
using System.Xml;

namespace Hidistro.ControlPanel.Store
{
	public static class StoreHelper
	{
		public static string BackupData()
		{
			string str = (new BackupRestoreDao()).BackupData(HttpContext.Current.Request.MapPath(string.Concat(Globals.ApplicationPath, "/Storage/data/Backup/")));
			return str;
		}

		public static int CreateVote(VoteInfo vote)
		{
			int num = 0;
			VoteDao voteDao = new VoteDao();
			long num1 = voteDao.CreateVote(vote);
			if (num1 > (long)0)
			{
				num = 1;
				if (vote.VoteItems != null)
				{
					foreach (VoteItemInfo voteItem in vote.VoteItems)
					{
						voteItem.VoteId = num1;
						voteItem.ItemCount = 0;
						num = num + voteDao.CreateVoteItem(voteItem, null);
					}
				}
			}
			return num;
		}

		public static bool DeleteBackupFile(string backupName)
		{
			bool flag;
			string str = HttpContext.Current.Request.MapPath(string.Concat(Globals.ApplicationPath, "/config/BackupFiles.config"));
			try
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.Load(str);
				foreach (XmlNode childNode in xmlDocument.SelectSingleNode("root").ChildNodes)
				{
					if (((XmlElement)childNode).GetAttribute("BackupName") == backupName)
					{
						xmlDocument.SelectSingleNode("root").RemoveChild(childNode);
					}
				}
				xmlDocument.Save(str);
				flag = true;
			}
			catch
			{
				flag = false;
			}
			return flag;
		}

		public static void DeleteImage(string imageUrl)
		{
			if (!string.IsNullOrEmpty(imageUrl))
			{
				try
				{
					string str = HttpContext.Current.Request.MapPath(string.Concat(Globals.ApplicationPath, imageUrl));
					Globals.DelImgByFilePath(str);
				}
				catch
				{
				}
			}
		}

		public static int DeleteVote(long voteId)
		{
			return (new VoteDao()).DeleteVote(voteId);
		}

		public static DataTable GetBackupFiles()
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("BackupName", typeof(string));
			dataTable.Columns.Add("Version", typeof(string));
			dataTable.Columns.Add("FileSize", typeof(string));
			dataTable.Columns.Add("BackupTime", typeof(string));
			string str = HttpContext.Current.Request.MapPath(string.Concat(Globals.ApplicationPath, "/config/BackupFiles.config"));
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load(str);
			foreach (XmlNode childNode in xmlDocument.SelectSingleNode("root").ChildNodes)
			{
				XmlElement xmlElement = (XmlElement)childNode;
				DataRow attribute = dataTable.NewRow();
				attribute["BackupName"] = xmlElement.GetAttribute("BackupName");
				attribute["Version"] = xmlElement.GetAttribute("Version");
				attribute["FileSize"] = xmlElement.GetAttribute("FileSize");
				attribute["BackupTime"] = xmlElement.GetAttribute("BackupTime");
				dataTable.Rows.Add(attribute);
			}
			return dataTable;
		}

		public static VoteInfo GetVoteById(long voteId)
		{
			return (new VoteDao()).GetVoteById(voteId);
		}

		public static int GetVoteCounts(long voteId)
		{
			return (new VoteDao()).GetVoteCounts(voteId);
		}

		public static IList<VoteItemInfo> GetVoteItems(long voteId)
		{
			return (new VoteDao()).GetVoteItems(voteId);
		}

		public static IList<VoteInfo> GetVoteList()
		{
			return (new VoteDao()).GetVoteList();
		}

		public static bool InserBackInfo(string fileName, string version, long fileSize)
		{
			bool flag;
			string str = HttpContext.Current.Request.MapPath(string.Concat(Globals.ApplicationPath, "/config/BackupFiles.config"));
			try
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.Load(str);
				XmlNode xmlNodes = xmlDocument.SelectSingleNode("root");
				XmlElement xmlElement = xmlDocument.CreateElement("backupfile");
				xmlElement.SetAttribute("BackupName", fileName);
				xmlElement.SetAttribute("Version", version.ToString());
				xmlElement.SetAttribute("FileSize", fileSize.ToString());
				xmlElement.SetAttribute("BackupTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
				xmlNodes.AppendChild(xmlElement);
				xmlDocument.Save(str);
				flag = true;
			}
			catch
			{
				flag = false;
			}
			return flag;
		}

		public static bool RestoreData(string bakFullName)
		{
			BackupRestoreDao backupRestoreDao = new BackupRestoreDao();
			bool flag = backupRestoreDao.RestoreData(bakFullName);
			backupRestoreDao.Restor();
			return flag;
		}

		public static bool UpdateVote(VoteInfo vote)
		{
			bool flag;
			VoteDao voteDao = new VoteDao();
			DbConnection dbConnection = DatabaseFactory.CreateDatabase().CreateConnection();
			try
			{
				dbConnection.Open();
				DbTransaction dbTransaction = dbConnection.BeginTransaction();
				try
				{
					try
					{
						if (!voteDao.UpdateVote(vote, dbTransaction))
						{
							dbTransaction.Rollback();
							flag = false;
						}
						else if (voteDao.DeleteVoteItem(vote.VoteId, dbTransaction))
						{
							int num = 0;
							if (vote.VoteItems != null)
							{
								foreach (VoteItemInfo voteItem in vote.VoteItems)
								{
									voteItem.VoteId = vote.VoteId;
									voteItem.ItemCount = 0;
									num = num + voteDao.CreateVoteItem(voteItem, dbTransaction);
								}
								if (num < vote.VoteItems.Count)
								{
									dbTransaction.Rollback();
									flag = false;
									return flag;
								}
							}
							dbTransaction.Commit();
							flag = true;
						}
						else
						{
							dbTransaction.Rollback();
							flag = false;
						}
					}
					catch
					{
						dbTransaction.Rollback();
						flag = false;
					}
				}
				finally
				{
					dbConnection.Close();
				}
			}
			finally
			{
				if (dbConnection != null)
				{
					((IDisposable)dbConnection).Dispose();
				}
			}
			return flag;
		}
	}
}