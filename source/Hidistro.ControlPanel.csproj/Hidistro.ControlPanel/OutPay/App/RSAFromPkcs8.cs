using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Hidistro.ControlPanel.OutPay.App
{
	public sealed class RSAFromPkcs8
	{
		public RSAFromPkcs8()
		{
		}

		private static bool CompareBytearrays(byte[] a, byte[] b)
		{
			bool flag;
			if ((int)a.Length == (int)b.Length)
			{
				int num = 0;
				byte[] numArray = a;
				int num1 = 0;
				while (num1 < (int)numArray.Length)
				{
					if (numArray[num1] == b[num])
					{
						num++;
						num1++;
					}
					else
					{
						flag = false;
						return flag;
					}
				}
				flag = true;
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		private static RSAParameters ConvertFromPrivateKey(string pemFileConent)
		{
			byte[] numArray = Convert.FromBase64String(pemFileConent);
			if ((int)numArray.Length < 609)
			{
				throw new ArgumentException("pem file content is incorrect.");
			}
			int num = 11;
			byte[] numArray1 = new byte[128];
			Array.Copy(numArray, num, numArray1, 0, 128);
			num = num + 128;
			num = num + 2;
			byte[] numArray2 = new byte[3];
			Array.Copy(numArray, num, numArray2, 0, 3);
			num = num + 3;
			num = num + 4;
			byte[] numArray3 = new byte[128];
			Array.Copy(numArray, num, numArray3, 0, 128);
			num = num + 128;
			num = num + (numArray[num + 1] == 64 ? 2 : 3);
			byte[] numArray4 = new byte[64];
			Array.Copy(numArray, num, numArray4, 0, 64);
			num = num + 64;
			num = num + (numArray[num + 1] == 64 ? 2 : 3);
			byte[] numArray5 = new byte[64];
			Array.Copy(numArray, num, numArray5, 0, 64);
			num = num + 64;
			num = num + (numArray[num + 1] == 64 ? 2 : 3);
			byte[] numArray6 = new byte[64];
			Array.Copy(numArray, num, numArray6, 0, 64);
			num = num + 64;
			num = num + (numArray[num + 1] == 64 ? 2 : 3);
			byte[] numArray7 = new byte[64];
			Array.Copy(numArray, num, numArray7, 0, 64);
			num = num + 64;
			num = num + (numArray[num + 1] == 64 ? 2 : 3);
			byte[] numArray8 = new byte[64];
			Array.Copy(numArray, num, numArray8, 0, 64);
			RSAParameters rSAParameter = new RSAParameters()
			{
				Modulus = numArray1,
				Exponent = numArray2,
				D = numArray3,
				P = numArray4,
				Q = numArray5,
				DP = numArray6,
				DQ = numArray7,
				InverseQ = numArray8
			};
			return rSAParameter;
		}

		private static RSAParameters ConvertFromPublicKey(string pemFileConent)
		{
			byte[] numArray = Convert.FromBase64String(pemFileConent);
			if ((int)numArray.Length < 162)
			{
				throw new ArgumentException("pem file content is incorrect.");
			}
			byte[] numArray1 = new byte[128];
			byte[] numArray2 = new byte[3];
			Array.Copy(numArray, 29, numArray1, 0, 128);
			Array.Copy(numArray, 159, numArray2, 0, 3);
			return new RSAParameters()
			{
				Modulus = numArray1,
				Exponent = numArray2
			};
		}

		private static RSACryptoServiceProvider DecodePemPrivateKey(string pemstr)
		{
			RSACryptoServiceProvider rSACryptoServiceProvider;
			byte[] numArray = Convert.FromBase64String(pemstr);
			if (numArray == null)
			{
				rSACryptoServiceProvider = null;
			}
			else
			{
				rSACryptoServiceProvider = RSAFromPkcs8.DecodePrivateKeyInfo(numArray);
			}
			return rSACryptoServiceProvider;
		}

		private static RSACryptoServiceProvider DecodePrivateKeyInfo(byte[] pkcs8)
		{
			RSACryptoServiceProvider rSACryptoServiceProvider;
			byte[] numArray = new byte[] { 48, 13, 6, 9, 42, 134, 72, 134, 247, 13, 1, 1, 1, 5, 0 };
			byte[] numArray1 = new byte[15];
			MemoryStream memoryStream = new MemoryStream(pkcs8);
			int length = (int)memoryStream.Length;
			BinaryReader binaryReader = new BinaryReader(memoryStream);
			byte num = 0;
			ushort num1 = 0;
			try
			{
				try
				{
					num1 = binaryReader.ReadUInt16();
					if (num1 == 33072)
					{
						binaryReader.ReadByte();
					}
					else if (num1 != 33328)
					{
						rSACryptoServiceProvider = null;
						return rSACryptoServiceProvider;
					}
					else
					{
						binaryReader.ReadInt16();
					}
					num = binaryReader.ReadByte();
					if (num == 2)
					{
						num1 = binaryReader.ReadUInt16();
						if (num1 != 1)
						{
							rSACryptoServiceProvider = null;
						}
						else if (RSAFromPkcs8.CompareBytearrays(binaryReader.ReadBytes(15), numArray))
						{
							num = binaryReader.ReadByte();
							if (num == 4)
							{
								num = binaryReader.ReadByte();
								if (num == 129)
								{
									binaryReader.ReadByte();
								}
								else if (num == 130)
								{
									binaryReader.ReadUInt16();
								}
								byte[] numArray2 = binaryReader.ReadBytes((int)((long)length - memoryStream.Position));
								rSACryptoServiceProvider = RSAFromPkcs8.DecodeRSAPrivateKey(numArray2);
							}
							else
							{
								rSACryptoServiceProvider = null;
							}
						}
						else
						{
							rSACryptoServiceProvider = null;
						}
					}
					else
					{
						rSACryptoServiceProvider = null;
					}
				}
				catch (Exception exception)
				{
					rSACryptoServiceProvider = null;
				}
			}
			finally
			{
				binaryReader.Close();
			}
			return rSACryptoServiceProvider;
		}

		private static RSACryptoServiceProvider DecodeRSAPrivateKey(byte[] privkey)
		{
			RSACryptoServiceProvider rSACryptoServiceProvider;
			BinaryReader binaryReader = new BinaryReader(new MemoryStream(privkey));
			ushort num = 0;
			try
			{
				try
				{
					num = binaryReader.ReadUInt16();
					if (num == 33072)
					{
						binaryReader.ReadByte();
					}
					else if (num != 33328)
					{
						rSACryptoServiceProvider = null;
						return rSACryptoServiceProvider;
					}
					else
					{
						binaryReader.ReadInt16();
					}
					num = binaryReader.ReadUInt16();
					if (num != 258)
					{
						rSACryptoServiceProvider = null;
					}
					else if (binaryReader.ReadByte() == 0)
					{
						byte[] numArray = binaryReader.ReadBytes(RSAFromPkcs8.GetIntegerSize(binaryReader));
						byte[] numArray1 = binaryReader.ReadBytes(RSAFromPkcs8.GetIntegerSize(binaryReader));
						byte[] numArray2 = binaryReader.ReadBytes(RSAFromPkcs8.GetIntegerSize(binaryReader));
						byte[] numArray3 = binaryReader.ReadBytes(RSAFromPkcs8.GetIntegerSize(binaryReader));
						byte[] numArray4 = binaryReader.ReadBytes(RSAFromPkcs8.GetIntegerSize(binaryReader));
						byte[] numArray5 = binaryReader.ReadBytes(RSAFromPkcs8.GetIntegerSize(binaryReader));
						byte[] numArray6 = binaryReader.ReadBytes(RSAFromPkcs8.GetIntegerSize(binaryReader));
						byte[] numArray7 = binaryReader.ReadBytes(RSAFromPkcs8.GetIntegerSize(binaryReader));
						RSACryptoServiceProvider rSACryptoServiceProvider1 = new RSACryptoServiceProvider();
						RSAParameters rSAParameter = new RSAParameters()
						{
							Modulus = numArray,
							Exponent = numArray1,
							D = numArray2,
							P = numArray3,
							Q = numArray4,
							DP = numArray5,
							DQ = numArray6,
							InverseQ = numArray7
						};
						rSACryptoServiceProvider1.ImportParameters(rSAParameter);
						rSACryptoServiceProvider = rSACryptoServiceProvider1;
					}
					else
					{
						rSACryptoServiceProvider = null;
					}
				}
				catch (Exception exception)
				{
					rSACryptoServiceProvider = null;
				}
			}
			finally
			{
				binaryReader.Close();
			}
			return rSACryptoServiceProvider;
		}

		private static byte[] decrypt(byte[] data, string privateKey, string input_charset)
		{
			RSACryptoServiceProvider rSACryptoServiceProvider = RSAFromPkcs8.DecodePemPrivateKey(privateKey);
			SHA1 sHA1CryptoServiceProvider = new SHA1CryptoServiceProvider();
			return rSACryptoServiceProvider.Decrypt(data, false);
		}

		public static string decryptData(string resData, string privateKey, string input_charset)
		{
			byte[] numArray = Convert.FromBase64String(resData);
			List<byte> nums = new List<byte>();
			for (int i = 0; i < (int)numArray.Length / 128; i++)
			{
				byte[] numArray1 = new byte[128];
				for (int j = 0; j < 128; j++)
				{
					numArray1[j] = numArray[j + 128 * i];
				}
				nums.AddRange(RSAFromPkcs8.decrypt(numArray1, privateKey, input_charset));
			}
			byte[] array = nums.ToArray();
			char[] chrArray = new char[Encoding.GetEncoding(input_charset).GetCharCount(array, 0, (int)array.Length)];
			Encoding.GetEncoding(input_charset).GetChars(array, 0, (int)array.Length, chrArray, 0);
			return new string(chrArray);
		}

		private static int GetIntegerSize(BinaryReader binr)
		{
			int num;
			byte num1 = 0;
			byte num2 = 0;
			byte num3 = 0;
			int num4 = 0;
			num1 = binr.ReadByte();
			if (num1 == 2)
			{
				num1 = binr.ReadByte();
				if (num1 == 129)
				{
					num4 = binr.ReadByte();
				}
				else if (num1 != 130)
				{
					num4 = num1;
				}
				else
				{
					num3 = binr.ReadByte();
					num2 = binr.ReadByte();
					byte[] numArray = new byte[] { num2, num3, 0, 0 };
					num4 = BitConverter.ToInt32(numArray, 0);
				}
				while (binr.ReadByte() == 0)
				{
					num4--;
				}
				binr.BaseStream.Seek((long)-1, SeekOrigin.Current);
				num = num4;
			}
			else
			{
				num = 0;
			}
			return num;
		}

		public static string sign(string content, string privateKey, string input_charset)
		{
			byte[] bytes = Encoding.GetEncoding(input_charset).GetBytes(content);
			RSACryptoServiceProvider rSACryptoServiceProvider = RSAFromPkcs8.DecodePemPrivateKey(privateKey);
			SHA1 sHA1CryptoServiceProvider = new SHA1CryptoServiceProvider();
			return Convert.ToBase64String(rSACryptoServiceProvider.SignData(bytes, sHA1CryptoServiceProvider));
		}

		public static bool verify(string content, string signedString, string publicKey, string input_charset)
		{
			byte[] bytes = Encoding.GetEncoding(input_charset).GetBytes(content);
			byte[] numArray = Convert.FromBase64String(signedString);
			RSAParameters rSAParameter = RSAFromPkcs8.ConvertFromPublicKey(publicKey);
			RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider();
			rSACryptoServiceProvider.ImportParameters(rSAParameter);
			return rSACryptoServiceProvider.VerifyData(bytes, new SHA1CryptoServiceProvider(), numArray);
		}
	}
}