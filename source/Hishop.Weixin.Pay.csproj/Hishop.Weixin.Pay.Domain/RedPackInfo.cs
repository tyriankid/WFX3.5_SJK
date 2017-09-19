using Newtonsoft.Json;
using System;

namespace Hishop.Weixin.Pay.Domain
{
	public class RedPackInfo
	{
		public string return_code
		{
			get;
			set;
		}

		public string return_msg
		{
			get;
			set;
		}

		public string sign
		{
			get;
			set;
		}

		public string result_code
		{
			get;
			set;
		}

		public string err_code
		{
			get;
			set;
		}

		public string mch_billno
		{
			get;
			set;
		}

		public string mch_id
		{
			get;
			set;
		}

		public string detail_id
		{
			get;
			set;
		}

		public string status
		{
			get;
			set;
		}

		public string openid
		{
			get;
			set;
		}

		public string send_type
		{
			get;
			set;
		}

		public DateTime send_time
		{
			get;
			set;
		}

		public DateTime? refund_time
		{
			get;
			set;
		}

		public redPackStatus Getstatus()
		{
			string status = this.status;
			redPackStatus result = redPackStatus.异常;
			string text = status;
			if (text != null)
			{
				if (!(text == "SENDING"))
				{
					if (!(text == "SENT"))
					{
						if (!(text == "FAILED"))
						{
							if (!(text == "RECEIVED"))
							{
								if (text == "REFUND")
								{
									result = redPackStatus.已退款;
								}
							}
							else
							{
								result = redPackStatus.已领取;
							}
						}
						else
						{
							result = redPackStatus.发放失败;
						}
					}
					else
					{
						result = redPackStatus.已发放待领取;
					}
				}
				else
				{
					result = redPackStatus.发放中;
				}
			}
			return result;
		}

		public new string ToString()
		{
			return JsonConvert.SerializeObject(this);
		}
	}
}
