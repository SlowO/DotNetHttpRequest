using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace ApiResources
{
	public class ApiBase
	{
		private static HttpClient HttpClient { get; set; }

		/// <summary>
		/// Set HttpClient header's with user, password authentication.
		/// </summary>
		/// <param name="userName"></param>
		/// <param name="password"></param>
		private static void SetUserNamePasswordAuth(string userName, string password)
		{
			HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
				"Basic", Convert.ToBase64String(
					Encoding.ASCII.GetBytes($"{userName}:{password}")
					));
		}

		/// <summary>
		/// Set HttpClient header's with token authentication.
		/// </summary>
		/// <param name="token"></param>
		private static void SetTokenAuth(string token)
		{
			// The API libraries require 'Bearer' for its authorization type instead of 'Token' "("Bearer", token)".
			HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", token);
		}

		/// <summary>
		/// Set HttpClient header's authentication according to the provided arguments.
		/// </summary>
		/// <param name="passOrToken"></param>
		/// <param name="userName"></param>
		public static void SelectAuthType(string passOrToken, string userName = null)
		{
			if (userName == null)
			{
				SetTokenAuth(passOrToken);
			}
			else
			{
				SetUserNamePasswordAuth(userName, passOrToken);
			}
		}

		/// <summary>
		/// Method that does the actual request send.
		/// </summary>
		/// <param name="url"></param>
		/// <param name="httpMethod"></param>
		/// <param name="content"></param>
		/// <returns>HttpResponseMessage Result</returns>
		private static HttpRequestMessage SendHttpRequest(string url, HttpMethod httpMethod, string content = null)
		{
			var httpRequestMessage = new HttpRequestMessage { RequestUri = new Uri(url), Method = httpMethod };
			if (httpMethod != HttpMethod.Get && content != null)
			{
				httpRequestMessage.Content = new StringContent(content, Encoding.UTF8, "application/json");
			}

			return httpRequestMessage;
		}

		/// <summary>
		/// Method to get the response message and return its Result.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="httpRequestMessage"></param>
		/// <returns></returns>
		public static string GetHttpResponseMessageResult(HttpClient client, HttpRequestMessage httpRequestMessage)
		{
			var httpResponseMessage = client.SendAsync(httpRequestMessage).Result;
			using (httpResponseMessage)
			{
				return httpResponseMessage.Content.ReadAsStringAsync().Result;
			}
		}

		/// <summary>
		/// Method to send a request and get response message's result.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="url"></param>
		/// <param name="httpMethod"></param>
		/// <param name="content"></param>
		/// <returns>HttpResponseMessage Result</returns>
		private static string SendRequestGetResponse(HttpClient client, string url, HttpMethod httpMethod, string content = null)
		{
			var httpRequestMessage = SendHttpRequest(url, httpMethod, content);

			return GetHttpResponseMessageResult(client, httpRequestMessage);
		}

		/// <summary>
		/// Sends the HTTP request without authentication and return the response message as a string.
		/// </summary>
		/// <param name="url">Full URL for the request</param>
		/// <param name="method">GET, POST, PATCH, PUT, or DELETE</param>
		/// <param name="content">Request body (optional)</param>
		/// <returns>HttpResponseMessage Result</returns>
		public static string SendHttpRequest(string url, string method, string content = null)
		{
			using (HttpClient = new HttpClient())
			{
				var httpMethod = new HttpMethod(method);

				return SendRequestGetResponse(HttpClient, url, httpMethod, content);
			}
		}

		/// <summary>
		/// Sends the HTTP request with authentication and return the response message as a string.
		/// </summary>
		/// <param name="url">Full URL for the request</param>
		/// <param name="method">GET, POST, PATCH, PUT, or DELETE</param>
		/// <param name="passOrToken">Password or token for autentication</param>
		/// <param name="userName">User name for authentication (optional if using a token)</param>
		/// <param name="content">Request body (optional)</param>
		/// <returns>HttpResponseMessage Result</returns>
		public static string SendHttpRequest(string url, string method, string passOrToken, string userName = null, string content = null)
		{
			using (HttpClient = new HttpClient())
			{
				SelectAuthType(passOrToken, userName);
				var httpMethod = new HttpMethod(method);

				return SendRequestGetResponse(HttpClient, url, httpMethod, content);
			}
		}

	}
}
