using Hidistro.ControlPanel.Commodities;
using Hidistro.ControlPanel.Members;
using Hidistro.ControlPanel.Promotions;
using Hidistro.Core;
using Hidistro.Core.Entities;
using Hidistro.Entities.Commodities;
using Hidistro.Entities.Members;
using Hidistro.Entities.Promotions;
using Hidistro.Messages;
using Hidistro.SqlDal.Promotions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ControlPanel.Promotions
{
	public static class GameHelper
	{
		private static Random Rnd;

		static GameHelper()
		{
			GameHelper.Rnd = new Random();
		}

		public static bool AddPrizeLog(PrizeResultInfo model)
		{
			if (model == null)
			{
				throw new ArgumentNullException("参数model不能不null");
			}
			return (new PrizeResultDao()).AddPrizeLog(model);
		}

		private static GamePrizeInfo ChouJiang(IList<GamePrizeInfo> prizeLists)
		{
			GamePrizeInfo gamePrizeInfo = (
				from x in Enumerable.Range(0, 10000)
				let prizeInfo0 = prizeLists[GameHelper.Rnd.Next(prizeLists.Count<GamePrizeInfo>())]
				let rnd = GameHelper.Rnd.Next(0, 100)
				where rnd < prizeInfo0.PrizeRate
				select prizeInfo0).First<GamePrizeInfo>();
			return gamePrizeInfo;
		}

		public static bool Create(GameInfo model, out int gameId)
		{
			if (model == null)
			{
				throw new ArgumentNullException("model参数不能为null");
			}
			Globals.EntityCoding(model, true);
			bool flag = (new GameDao()).Create(model, out gameId);
			if (flag)
			{
			}
			return flag;
		}

		public static bool CreatePrize(GamePrizeInfo model)
		{
			if (model == null)
			{
				throw new ArgumentNullException("model参数不能为null");
			}
			bool flag = (new GamePrizeDao()).Create(model);
			if (flag)
			{
			}
			return flag;
		}

		public static bool CreateWinningPool(GameWinningPoolInfo model)
		{
			if (model == null)
			{
				throw new ArgumentNullException("model参数不能为null");
			}
			bool flag = (new GamePrizeDao()).CreateWinningPool(model);
			if (flag)
			{
			}
			return flag;
		}

		public static void CreateWinningPools(float PrizeRate, int prizeCount, int gameId)
		{
			GameWinningPoolInfo gameWinningPoolInfo;
			IList<GamePrizeInfo> gamePrizeListsByGameId = GameHelper.GetGamePrizeListsByGameId(gameId);
			List<int> pool = GameHelper.GetPool(gamePrizeListsByGameId.ToList<GamePrizeInfo>());
			int num = (int)((float)prizeCount / (PrizeRate / 100f));
			float single = (float)prizeCount % (PrizeRate / 100f);
			for (int i = 1; i < num + 1; i++)
			{
				gameWinningPoolInfo = new GameWinningPoolInfo()
				{
					GameId = gameId,
					Number = i,
					GamePrizeId = (i < prizeCount + 1 ? pool[i - 1] : 0),
					IsReceive = 0
				};
				GameHelper.CreateWinningPool(gameWinningPoolInfo);
			}
			if (single > 0f)
			{
				gameWinningPoolInfo = new GameWinningPoolInfo()
				{
					GameId = gameId,
					Number = num + 1,
					GamePrizeId = 0,
					IsReceive = 0
				};
				GameHelper.CreateWinningPool(gameWinningPoolInfo);
			}
		}

		public static bool Delete(params int[] gameIds)
		{
			if ((gameIds == null ? true : gameIds.Count<int>() <= 0))
			{
				throw new ArgumentNullException("参数gameIds不能为空！");
			}
			return (new GameDao()).Delete(gameIds);
		}

		public static bool DeletePrizesDelivery(int[] ids)
		{
			return (new GameDao()).DeletePrizesDelivery(ids);
		}

		public static bool DeletePromotionGamePrize(GamePrizeInfo model)
		{
			if (model == null)
			{
				throw new ArgumentNullException("model参数不能为null");
			}
			bool flag = (new GamePrizeDao()).Delete(model);
			if (flag)
			{
			}
			return flag;
		}

		public static bool DeleteWinningPools(int gameId)
		{
			return (new GamePrizeDao()).DeleteWinningPools(gameId);
		}

		public static DbQueryResult GetAllPrizesDeliveryList(PrizesDeliveQuery query, string ExtendLimits = "", string selectFields = "*")
		{
			return (new GameDao()).GetAllPrizesDeliveryList(query, ExtendLimits, selectFields);
		}

		public static GameInfo GetGameInfoById(int gameId)
		{
			return (new GameDao()).GetGameInfoById(gameId);
		}

		public static DbQueryResult GetGameList(GameSearch search)
		{
			return (new GameDao()).GetGameList(search);
		}

		public static DbQueryResult GetGameListByView(GameSearch search)
		{
			return (new GameDao()).GetGameListByView(search);
		}

		public static GamePrizeInfo GetGamePrizeInfoById(int prizeId)
		{
			if (prizeId < 0)
			{
				throw new ArgumentNullException("参数错误");
			}
			return (new GamePrizeDao()).GetGamePrizeInfoById(prizeId);
		}

		public static IList<GamePrizeInfo> GetGamePrizeListsByGameId(int gameId)
		{
			IList<GamePrizeInfo> gamePrizeListsByGameId;
			if (gameId > 0)
			{
				gamePrizeListsByGameId = (new GamePrizeDao()).GetGamePrizeListsByGameId(gameId);
			}
			else
			{
				gamePrizeListsByGameId = null;
			}
			return gamePrizeListsByGameId;
		}

		public static string GetGameTypeName(string GameType)
		{
			string name = Enum.GetName(typeof(Hidistro.Entities.Promotions.GameType), int.Parse(GameType));
			return name;
		}

		public static IEnumerable<GameInfo> GetLists(GameSearch search)
		{
			return (new GameDao()).GetLists(search);
		}

		public static GameInfo GetModelByGameId(int gameId)
		{
			GameInfo modelByGameId = (new GameDao()).GetModelByGameId(gameId);
			Globals.EntityCoding(modelByGameId, false);
			return modelByGameId;
		}

		public static GameInfo GetModelByGameId(string keyWord)
		{
			GameInfo modelByGameId = (new GameDao()).GetModelByGameId(keyWord);
			Globals.EntityCoding(modelByGameId, false);
			return modelByGameId;
		}

		public static GamePrizeInfo GetModelByPrizeGradeAndGameId(PrizeGrade grade, int gameId)
		{
			return (new GamePrizeDao()).GetModelByPrizeGradeAndGameId(grade, gameId);
		}

		public static int GetOppNumber(int userId, int gameId)
		{
			return (new GamePrizeDao()).GetOppNumber(userId, gameId);
		}

		public static int GetOppNumberByToday(int userId, int gameId)
		{
			return (new GamePrizeDao()).GetOppNumberByToday(userId, gameId);
		}

		public static DataSet GetOrdersAndLines(string orderIds, string pIds)
		{
			return (new GameDao()).GetOrdersAndLines(orderIds, pIds);
		}

		private static List<int> GetPool(List<GamePrizeInfo> list)
		{
			List<int> nums = new List<int>();
			if ((list == null ? false : list.Count > 0))
			{
				foreach (GamePrizeInfo gamePrizeInfo in list)
				{
					if ((gamePrizeInfo == null ? false : gamePrizeInfo.PrizeCount > 0))
					{
						for (int i = 0; i < gamePrizeInfo.PrizeCount; i++)
						{
							nums.Add(gamePrizeInfo.PrizeId);
						}
					}
				}
			}
			return nums;
		}

		public static string GetPrizeFullName(PrizeResultViewInfo item)
		{
			string couponName;
			switch (item.PrizeType)
			{
				case PrizeType.赠送积分:
				{
					couponName = string.Format("{0} 积分", item.GivePoint);
					break;
				}
				case PrizeType.赠送优惠券:
				{
					CouponInfo coupon = CouponHelper.GetCoupon(int.Parse(item.GiveCouponId));
					if (coupon == null)
					{
						couponName = string.Concat("优惠券", item.GiveCouponId, "[已删除]");
						break;
					}
					else
					{
						couponName = coupon.CouponName;
						break;
					}
				}
				case PrizeType.赠送商品:
				{
					ProductInfo productBaseInfo = ProductHelper.GetProductBaseInfo(int.Parse(item.GiveShopBookId));
					if (productBaseInfo == null)
					{
						couponName = "赠送商品[已删除]";
						break;
					}
					else
					{
						couponName = productBaseInfo.ProductName;
						break;
					}
				}
				default:
				{
					couponName = "";
					break;
				}
			}
			return couponName;
		}

		public static string GetPrizeGradeName(string PrizeGrade)
		{
			string name = Enum.GetName(typeof(Hidistro.Entities.Promotions.PrizeGrade), int.Parse(PrizeGrade));
			return name;
		}

		public static DataSet GetPrizeListByLogIDList(string orderlist, string pidlist)
		{
			return (new GameDao()).GetPrizeListByLogIDList(orderlist, pidlist);
		}

		public static IList<PrizeResultViewInfo> GetPrizeLogLists(int gameId, int pageIndex, int pageSize)
		{
			return (new PrizeResultDao()).GetPrizeLogLists(gameId, pageIndex, pageSize);
		}

		public static DbQueryResult GetPrizeLogLists(PrizesDeliveQuery query)
		{
			return (new PrizeResultDao()).GetPrizeLogLists(query);
		}

		public static string GetPrizeName(Hidistro.Entities.Promotions.PrizeType PrizeType, string FullName)
		{
			string fullName;
			switch (PrizeType)
			{
				case Hidistro.Entities.Promotions.PrizeType.赠送优惠券:
				case Hidistro.Entities.Promotions.PrizeType.赠送商品:
				{
					fullName = Globals.SubStr(FullName, 12, "..");
					break;
				}
				default:
				{
					fullName = FullName;
					break;
				}
			}
			return fullName;
		}

		public static DbQueryResult GetPrizesDeliveryList(PrizesDeliveQuery query, string ExtendLimits = "", string selectFields = "*")
		{
			return (new GameDao()).GetPrizesDeliveryList(query, ExtendLimits, selectFields);
		}

		public static DataTable GetPrizesDeliveryNum()
		{
			return (new GameDao()).GetPrizesDeliveryNum();
		}

		public static string GetPrizesDeliveStatus(string status, string isLogistics, string type, string gametype)
		{
			string str = "未定义";
			string str1 = status;
			if (str1 != null)
			{
				if (str1 == "0")
				{
					if ((type == "2" ? false : !(gametype == "5")))
					{
						str = (!(isLogistics == "0") ? "待填写收货地址" : "已收货");
					}
					else
					{
						str = "待填写收货地址";
					}
				}
				else if (str1 == "1")
				{
					str = "待发货";
				}
				else if (str1 == "2")
				{
					str = "已发货";
				}
				else if (str1 == "3")
				{
					str = "已收货";
				}
				else if (str1 == "4")
				{
					str = "已收货";
				}
			}
			return str;
		}

		public static List<GameWinningPool> GetWinningPoolList(int gameId)
		{
			return (new GameDao()).GetWinningPoolList(gameId);
		}

		public static bool IsCanPrize(int gameId, int userid)
		{
			return (new PrizeResultDao()).IsCanPrize(gameId, userid);
		}

		public static bool SetPrintOrderExpress(string orderId, string pid, string expressCompanyName, string expressCompanyAbb, string shipOrderNumber)
		{
			bool flag = (new GameDao()).SetPrintOrderExpress(orderId, pid, expressCompanyName, expressCompanyAbb, shipOrderNumber);
			return flag;
		}

		public static bool Update(GameInfo model)
		{
			if (model == null)
			{
				throw new ArgumentNullException("model参数不能为null");
			}
			Globals.EntityCoding(model, true);
			bool flag = (new GameDao()).Update(model, true);
			if (flag)
			{
			}
			return flag;
		}

		public static bool UpdateOneyuanDelivery(PrizesDeliveQuery query)
		{
			return (new GameDao()).UpdatePrizesDelivery(query);
		}

		public static bool UpdateOutOfDateStatus()
		{
			return (new GameDao()).UpdateOutOfDateStatus();
		}

		public static bool UpdatePrize(GamePrizeInfo model)
		{
			if (model == null)
			{
				throw new ArgumentNullException("model参数不能为null");
			}
			bool flag = (new GamePrizeDao()).Update(model);
			if (flag)
			{
			}
			return flag;
		}

		public static bool UpdatePrizeLogStatus(int logId)
		{
			return (new PrizeResultDao()).UpdatePrizeLogStatus(logId);
		}

		public static bool UpdatePrizesDelivery(PrizesDeliveQuery query)
		{
			bool flag = (new GameDao()).UpdatePrizesDelivery(query);
			if (flag)
			{
				string str = "";
				int num = 0;
				string str1 = "";
				(new GameDao()).GetPrizesDeliveInfo(query, out str, out num, out str1);
				try
				{
					MemberInfo member = MemberHelper.GetMember(num);
					if (member != null)
					{
						Messenger.SendWeiXinMsg_PrizeRelease(member, str, str1);
					}
				}
				catch (Exception exception)
				{
				}
			}
			return flag;
		}

		public static bool UpdateStatus(int gameId, GameStatus status)
		{
			bool flag = (new GameDao()).UpdateStatus(gameId, status);
			if (flag)
			{
			}
			return flag;
		}

		public static bool UpdateWinningPoolIsReceive(int winningPoolId)
		{
			return (new GameDao()).UpdateWinningPoolIsReceive(winningPoolId);
		}

		public static GamePrizeInfo UserPrize(int gameId, int useId)
		{
			int[] numArray = new int[] { 67, 112, 202, 292, 337 };
			IList<GamePrizeInfo> gamePrizeListsByGameId = GameHelper.GetGamePrizeListsByGameId(gameId);
			int num = gamePrizeListsByGameId.Max<GamePrizeInfo>((GamePrizeInfo p) => p.PrizeRate);
			GamePrizeInfo gamePrizeInfo = new GamePrizeInfo()
			{
				PrizeId = 0,
				PrizeRate = (num >= 100 ? 0 : 100),
				PrizeGrade = PrizeGrade.未中奖
			};
			GamePrizeInfo gamePrizeInfo1 = gamePrizeInfo;
			gamePrizeListsByGameId.Add(gamePrizeInfo1);
			GamePrizeInfo gamePrizeInfo2 = GameHelper.ChouJiang(gamePrizeListsByGameId);
			if ((gamePrizeInfo2.PrizeId == 0 ? false : gamePrizeInfo2.PrizeCount <= 0))
			{
				gamePrizeInfo2 = gamePrizeInfo1;
			}
			if ((gamePrizeInfo2.PrizeId == 0 ? false : gamePrizeInfo2.PrizeType == PrizeType.赠送优惠券))
			{
				if (CouponHelper.IsCanSendCouponToMember(int.Parse(gamePrizeInfo2.GiveCouponId), useId) != SendCouponResult.正常领取)
				{
					gamePrizeInfo2 = gamePrizeInfo1;
				}
			}
			PrizeResultDao prizeResultDao = new PrizeResultDao();
			PrizeResultInfo prizeResultInfo = new PrizeResultInfo()
			{
				GameId = gameId,
				PrizeId = gamePrizeInfo2.PrizeId,
				UserId = useId
			};
			prizeResultDao.AddPrizeLog(prizeResultInfo);
			return gamePrizeInfo2;
		}
	}
}