using System;
using System.IO;
using System.Text;
using System.Web.UI;

namespace Hidistro.UI.Web.Admin.Fenxiao
{
	public class test : System.Web.UI.Page
	{
		protected void Page_Load(object sender, System.EventArgs e)
		{
			base.Response.Write("");
		}

		public string Read(string path)
		{
			System.IO.StreamReader streamReader = new System.IO.StreamReader(path, System.Text.Encoding.Default);
			System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
			string value;
			while ((value = streamReader.ReadLine()) != null)
			{
				stringBuilder.Append(value);
			}
			streamReader.Close();
			streamReader.Dispose();
			return stringBuilder.ToString();
		}
	}
}
