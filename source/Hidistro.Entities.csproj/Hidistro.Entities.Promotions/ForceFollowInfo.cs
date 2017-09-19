using System;

namespace Hidistro.Entities.Promotions
{
	public class ForceFollowInfo
	{
		public bool EnableGuidePageSet
		{
			get;
			set;
		}

		public bool IsAutoGuide
		{
			get;
			set;
		}

		public bool IsMustConcern
		{
			get;
			set;
		}

		public int GuideConcernType
		{
			get;
			set;
		}

		public string GuidePageSet
		{
			get;
			set;
		}

		public string ConcernMsg
		{
			get;
			set;
		}

		public int FollowInfo
		{
			get;
			set;
		}
	}
}
