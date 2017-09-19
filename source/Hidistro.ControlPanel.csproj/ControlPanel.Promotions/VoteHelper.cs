using Hidistro.Core.Entities;
using Hidistro.Entities.Promotions;
using Hidistro.SqlDal.Promotions;
using System;
using System.Linq;

namespace ControlPanel.Promotions
{
	public class VoteHelper
	{
		private static VoteDao _vote;

		static VoteHelper()
		{
			VoteHelper._vote = new VoteDao();
		}

		public VoteHelper()
		{
		}

		public static long Create(VoteInfo vote)
		{
			return VoteHelper._vote.CreateVote(vote);
		}

		public static bool Delete(long Id)
		{
			return VoteHelper._vote.DeleteVote(Id) > 0;
		}

		public static VoteInfo GetVote(long Id)
		{
			return VoteHelper._vote.GetVoteById(Id);
		}

		public static int GetVoteAttends(long voteId)
		{
			return VoteHelper._vote.GetVoteAttends(voteId);
		}

		public static int GetVoteCounts(long voteId)
		{
			return VoteHelper._vote.GetVoteCounts(voteId);
		}

		public static bool IsVote(int voteId)
		{
			return (new VoteDao()).IsVote(voteId);
		}

		public static DbQueryResult Query(VoteSearch query)
		{
			return VoteHelper._vote.Query(query);
		}

		public static bool Update(VoteInfo vote, bool isUpdateItems = true)
		{
			bool flag;
			flag = (!isUpdateItems ? VoteHelper._vote.UpdateVote(vote) : VoteHelper._vote.UpdateVoteAll(vote));
			return flag;
		}

		public static bool Vote(int voteId, string itemIds)
		{
			bool flag;
			if (VoteHelper.IsVote(voteId))
			{
				throw new Exception("已投过票！");
			}
			VoteInfo vote = VoteHelper.GetVote((long)voteId);
			if (vote.IsMultiCheck)
			{
				if (vote.MaxCheck <= 0)
				{
					flag = true;
				}
				else
				{
					int maxCheck = vote.MaxCheck;
					char[] chrArray = new char[] { ',' };
					flag = maxCheck >= itemIds.Split(chrArray).Count<string>();
				}
				if (!flag)
				{
					throw new Exception(string.Format("对不起！您最多能选{0}项...", vote.MaxCheck));
				}
			}
			return (new VoteDao()).Vote(voteId, itemIds);
		}
	}
}