using System;
using System.Collections.Generic;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace Hidistro.Core
{
	public sealed class HiCryptographer
	{
		public static string Md5Encrypt(string sourceData)
		{
			Encoding encoding = new UTF8Encoding();
			byte[] bytes = encoding.GetBytes("12345678");
			byte[] rgbIV = new byte[]
			{
				1,
				2,
				3,
				4,
				5,
				6,
				8,
				7
			};
			string result;
			try
			{
				DESCryptoServiceProvider dESCryptoServiceProvider = new DESCryptoServiceProvider();
				ICryptoTransform cryptoTransform = dESCryptoServiceProvider.CreateEncryptor(bytes, rgbIV);
				byte[] bytes2 = encoding.GetBytes(sourceData);
				byte[] inArray = cryptoTransform.TransformFinalBlock(bytes2, 0, bytes2.Length);
				string text = Convert.ToBase64String(inArray);
				result = text;
			}
			catch
			{
				throw;
			}
			return result;
		}

		public static string Encrypt(string text)
		{
			string result;
			using (RijndaelManaged rijndaelManaged = new RijndaelManaged())
			{
				rijndaelManaged.Key = Convert.FromBase64String(ConfigurationManager.AppSettings["Key"]);
				rijndaelManaged.IV = Convert.FromBase64String(ConfigurationManager.AppSettings["IV"]);
				ICryptoTransform cryptoTransform = rijndaelManaged.CreateEncryptor();
				byte[] bytes = Encoding.UTF8.GetBytes(text);
				byte[] inArray = cryptoTransform.TransformFinalBlock(bytes, 0, bytes.Length);
				cryptoTransform.Dispose();
				result = Convert.ToBase64String(inArray);
			}
			return result;
		}

		public static string Decrypt(string text)
		{
			string @string;
			using (RijndaelManaged rijndaelManaged = new RijndaelManaged())
			{
				rijndaelManaged.Key = Convert.FromBase64String(ConfigurationManager.AppSettings["Key"]);
				rijndaelManaged.IV = Convert.FromBase64String(ConfigurationManager.AppSettings["IV"]);
				ICryptoTransform cryptoTransform = rijndaelManaged.CreateDecryptor();
				byte[] array = Convert.FromBase64String(text);
				byte[] bytes = cryptoTransform.TransformFinalBlock(array, 0, array.Length);
				cryptoTransform.Dispose();
				@string = Encoding.UTF8.GetString(bytes);
			}
			return @string;
		}

		private static byte[] CreateHash(byte[] plaintext)
		{
			MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
			return mD5CryptoServiceProvider.ComputeHash(plaintext);
		}

		public static string CreateHash(string plaintext)
		{
			byte[] array = HiCryptographer.CreateHash(Encoding.ASCII.GetBytes(plaintext));
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < array.Length; i++)
			{
				stringBuilder.Append(array[i].ToString("x2"));
			}
			return stringBuilder.ToString();
		}

		public static string SignTopRequest(IDictionary<string, string> parameters, string appSecret)
		{
			IDictionary<string, string> dictionary = new SortedDictionary<string, string>(parameters, StringComparer.Ordinal);
			IEnumerator<KeyValuePair<string, string>> enumerator = dictionary.GetEnumerator();
			StringBuilder stringBuilder = new StringBuilder();
			while (enumerator.MoveNext())
			{
				KeyValuePair<string, string> current = enumerator.Current;
				string key = current.Key;
				current = enumerator.Current;
				string value = current.Value;
				if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
				{
					stringBuilder.Append(key).Append(value);
				}
			}
			if (!string.IsNullOrEmpty(appSecret))
			{
				stringBuilder.Append(appSecret);
			}
			MD5 mD = MD5.Create();
			byte[] array = mD.ComputeHash(Encoding.UTF8.GetBytes(stringBuilder.ToString()));
			StringBuilder stringBuilder2 = new StringBuilder();
			for (int i = 0; i < array.Length; i++)
			{
				stringBuilder2.Append(array[i].ToString("X2"));
			}
			return stringBuilder2.ToString();
		}
	}
}
