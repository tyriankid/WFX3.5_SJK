using Hidistro.Core;
using System;
using System.Web.UI;

namespace Hidistro.UI.Web.Admin
{
	public class testWX : System.Web.UI.Page
	{
		protected void Page_Load(object sender, System.EventArgs e)
		{
			string text = HiCryptographer.Md5Encrypt("888999");
		}
	}
}
