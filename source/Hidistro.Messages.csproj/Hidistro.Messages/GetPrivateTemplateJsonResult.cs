using System;
using System.Collections.Generic;

namespace Hidistro.Messages
{
	public class GetPrivateTemplateJsonResult : WxJsonResult
	{
		public List<GetPrivateTemplate_TemplateItem> template_list
		{
			get;
			set;
		}
	}
}
