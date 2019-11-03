//--TFL Road Status Code
//--Developed By Sofiul Alam
//--Date: 03/11/2019
using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;

namespace RoadStatus
{
	class Program
	{
		static void Main(string[] args)
		{
			bool IsMenu = true;
			while (IsMenu)
			{
				IsMenu = MainMenu();
			}
		}

		private static bool MainMenu()
		{
			Console.Clear();
				Console.WriteLine();
				Console.WriteLine("1. Enter Keys and road name");
				Console.WriteLine("2. Run Application");
				Console.WriteLine("3. Exit");
				Console.Write("\r\nSelect an option: ");

			switch (Console.ReadLine())
			{
				case "1":
					Console.WriteLine("You have choosen Option 1");
						string appid = string.Empty;
						string appkey = string.Empty;
						string road_name = string.Empty;
					Console.Write("Please enter a appid: ");
						appid = Console.ReadLine();
					Console.Write("Please enter a appkey: ");
						appkey = Console.ReadLine();
					Console.Write("Please enter a Road name: ");
						road_name = Console.ReadLine();
						Run(road_name, appid, appkey);
					Console.ReadLine();
					return true;
				case "2":
					Console.WriteLine("You have choosen Option 2");
						string roadname = string.Empty;
					Console.Write("Please enter a Road name: ");
						roadname = Console.ReadLine();
						Run(roadname);
					Console.ReadLine();
					return true;
				case "3":
					Console.WriteLine("You have choosen Option 3");
					return false;
				default:
					return true;
			}
		}

		public static string Run(string roadName, string appId = null, string appKey = null)
		{
			#region Fields
				string apiUri = string.Empty;
				string strJson = string.Empty;
				string displayName = string.Empty;
				string statusSeverity = string.Empty;
				string statusSeverityDescription = string.Empty;
				string exceptionType = string.Empty;
				string httpStatusCode = string.Empty;
				string result = string.Empty;
			#endregion

			#region Methods
			//Check values
			if (roadName == null)
				return result;
			if (appId == null)
				//Set to default
				appId = ConfigurationManager.AppSettings["appId"];
			if (appKey == null)
				//Set to default
				appKey = ConfigurationManager.AppSettings["appKey"];
			//Construct URI
				apiUri = ConfigurationManager.AppSettings["TFLUri"] + roadName + "?app_id=" + appId + "&app_key=" + appKey;
			//Request Object
			ServicePointManager.Expect100Continue = true;
			ServicePointManager.DefaultConnectionLimit = 9999;
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
			//Get Method
			using (var client = new HttpClient())
			{
				client.BaseAddress = new Uri(apiUri);
				client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
					HttpResponseMessage response = client.GetAsync(apiUri).Result;
				strJson = response.Content.ReadAsStringAsync().Result;
					var token = JToken.Parse(strJson);
				if (token.Type == JTokenType.Array)
				{
					JArray rss = JArray.Parse(strJson.Replace('"', '\''));
					dynamic data = JObject.Parse(rss[0].ToString());
					displayName = (string)data["displayName"];
					statusSeverity = (string)data["statusSeverity"];
					statusSeverityDescription = (string)data["statusSeverityDescription"];
						Console.WriteLine();
						Console.WriteLine("The status of the {0} is as follows:\n", displayName);
						Console.WriteLine("		Road Status is {0}", statusSeverity);
						Console.WriteLine("		Road Status Description is {0}", statusSeverityDescription);
						Console.ReadLine();
						Environment.Exit(0);
				}
				else if (token.Type == JTokenType.Object)
				{
					JObject rss = JObject.Parse(strJson.Replace('"', '\''));
					httpStatusCode = (string)rss["httpStatusCode"];
					if (Convert.ToInt32(httpStatusCode) == 404)
					{
						Console.WriteLine();
						Console.WriteLine("{0} is not a valid road\n", roadName);
							result = "{0} is not a valid road" + roadName;
						Console.ReadLine();
						Environment.Exit(1);
					}
				}
				else
				{
					Console.WriteLine($"Neither, it's actually a {token.Type}");
				}
			}
			#endregion
			return result;
		}
	}
}
