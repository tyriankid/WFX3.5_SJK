using System;
using System.ComponentModel;
using System.Reflection;

namespace Hishop.Open.Api
{
	public static class OpenApiErrorMessage
	{
		public static string GetEnumDescription(Enum enumSubitem)
		{
			string text = enumSubitem.ToString();
			FieldInfo field = enumSubitem.GetType().GetField(text);
			object[] customAttributes = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
			if (customAttributes == null || customAttributes.Length == 0)
			{
				return text;
			}
			DescriptionAttribute descriptionAttribute = (DescriptionAttribute)customAttributes[0];
			return descriptionAttribute.Description;
		}

		public static string ShowErrorMsg(Enum enumSubitem, string fields)
		{
			string text = OpenApiErrorMessage.GetEnumDescription(enumSubitem).Replace("_", " ");
			string format = "{{\"error_response\":{{\"code\":\"{0}\",\"msg\":\"{1}:{2}\",\"sub_msg\":\"{3}\"}}}}";
			return string.Format(format, new object[]
			{
				Convert.ToInt16(enumSubitem).ToString(),
				enumSubitem.ToString().Replace("_", " "),
				fields,
				text
			});
		}
	}
}
