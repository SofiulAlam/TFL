using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using System.Configuration;
using System.Net;
using System.Net.Http.Headers;

namespace RoadStatusTests
{
	[TestFixture]
	class RoadStatusTest
	{
		private HttpClient client;
		private HttpResponseMessage response;
		private string roadName = "A2";
		string apiUri = string.Empty;
		string appId = string.Empty;
		string appKey = string.Empty;

		[SetUp]
		public void SetUP()
		{
			appId = ConfigurationManager.AppSettings["appId"];
			appKey = ConfigurationManager.AppSettings["appKey"];
			apiUri = ConfigurationManager.AppSettings["TFLUri"] + roadName + "?app_id=" + ConfigurationManager.AppSettings["appId"] + "&app_key=" + ConfigurationManager.AppSettings["appKey"];
			//Request Object
			ServicePointManager.Expect100Continue = true;
			ServicePointManager.DefaultConnectionLimit = 9999;
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
			using (client = new HttpClient())
			{
				client.BaseAddress = new Uri(apiUri);
				client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
				response = client.GetAsync(apiUri).Result;
			}
		}
		[Test]
		public void GetResponseIsSuccess()
		{
			//Arrang
			//Act
			//Assert
			Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
		}

		[Test]
		public void GettheResponseIsJson()
		{
			client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			Assert.AreEqual("application/json", response.Content.Headers.ContentType.MediaType);
		}

		[Test]
		public void GetResponseIsJson()
		{
			client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			Assert.AreEqual("application/json", response.Content.Headers.ContentType.MediaType);
		}

		[Test]
		public void GetAuthenticationStatus()
		{
			Assert.AreNotEqual(HttpStatusCode.Unauthorized, response.StatusCode);
		}
	}
}
