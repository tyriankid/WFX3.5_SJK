using System;

namespace Hidistro.Messages
{
	public class GetIndustry_Item
	{
		public string first_class
		{
			get;
			set;
		}

		public string second_class
		{
			get;
			set;
		}

		public IndustryCode ConvertToIndustryCode()
		{
			string value = string.Format("{0}_{1}", this.first_class, this.second_class.Replace("|", "_").Replace("/", "_"));
			IndustryCode industryCode;
			IndustryCode result;
			if (!Enum.TryParse<IndustryCode>(value, true, out industryCode))
			{
				result = IndustryCode.其它_其它;
			}
			else
			{
				result = industryCode;
			}
			return result;
		}
	}
}
