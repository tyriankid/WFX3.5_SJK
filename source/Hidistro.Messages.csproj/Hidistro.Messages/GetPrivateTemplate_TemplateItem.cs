using System;

namespace Hidistro.Messages
{
	public class GetPrivateTemplate_TemplateItem
	{
		public string template_id
		{
			get;
			set;
		}

		public string title
		{
			get;
			set;
		}

		public string primary_industry
		{
			get;
			set;
		}

		public string deputy_industry
		{
			get;
			set;
		}

		public string content
		{
			get;
			set;
		}

		public string example
		{
			get;
			set;
		}

		public IndustryCode ConvertToIndustryCode()
		{
			string value = string.Format("{0}_{1}", this.primary_industry, this.deputy_industry.Replace("|", "_").Replace("/", "_"));
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
