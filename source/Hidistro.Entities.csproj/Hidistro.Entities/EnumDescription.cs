using System;
using System.ComponentModel;
using System.Reflection;

namespace Hidistro.Entities
{
	public class EnumDescription
	{
		public static string GetEnumDescription(System.Enum enumSubitem, int index)
		{
			string text = enumSubitem.ToString();
			System.Reflection.FieldInfo field = enumSubitem.GetType().GetField(text);
			object[] customAttributes = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
			string result;
			if (customAttributes == null || customAttributes.Length == 0)
			{
				result = text;
			}
			else
			{
				DescriptionAttribute descriptionAttribute = (DescriptionAttribute)customAttributes[0];
				result = descriptionAttribute.Description.Split(new char[]
				{
					'|'
				})[index];
			}
			return result;
		}

		public static string GetEnumDescription(System.Enum enumSubitem)
		{
			string text = enumSubitem.ToString();
			System.Reflection.FieldInfo field = enumSubitem.GetType().GetField(text);
			object[] customAttributes = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
			string result;
			if (customAttributes == null || customAttributes.Length == 0)
			{
				result = text;
			}
			else
			{
				DescriptionAttribute descriptionAttribute = (DescriptionAttribute)customAttributes[0];
				result = descriptionAttribute.Description;
			}
			return result;
		}

		public static bool GetEnumValue<TEnum>(string enumDescription, ref TEnum currentfiled)
		{
			bool result = false;
			System.Type typeFromHandle = typeof(TEnum);
			System.Reflection.FieldInfo[] fields = typeFromHandle.GetFields();
			for (int i = 1; i < fields.Length - 1; i++)
			{
				DescriptionAttribute descriptionAttribute = fields[i].GetCustomAttributes(typeof(DescriptionAttribute), false)[0] as DescriptionAttribute;
				if (descriptionAttribute.Description.Contains(enumDescription))
				{
					currentfiled = (TEnum)((object)fields[i].GetValue(typeof(TEnum)));
					result = true;
					break;
				}
			}
			return result;
		}
	}
}
