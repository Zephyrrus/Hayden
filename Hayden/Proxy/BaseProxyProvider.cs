﻿using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Nito.AsyncEx;

namespace Hayden.Proxy
{
	/// <summary>
	/// A class that holds information about a specific <see cref="HttpClient"/> used as a proxy.
	/// </summary>
	public class HttpClientProxy
	{
		/// <summary>
		/// The client used for HTTP calls.
		/// </summary>
		public HttpClient Client { get; }

		/// <summary>
		/// The user-friendly name of the client.
		/// </summary>
		public string Name { get; }

		public HttpClientProxy(HttpClient client, string name)
		{
			Client = client;
			Name = name;
		}
	}

	/// <summary>
	/// Provides <see cref="HttpClientProxy"/> objects for use in the scraper.
	/// </summary>
	public abstract class ProxyProvider
	{
		protected Action<HttpClientHandler> ConfigureClientHandlerAction { get; }
		protected virtual AsyncCollection<HttpClientProxy> ProxyClients { get; } = new AsyncCollection<HttpClientProxy>();

		protected ProxyProvider(Action<HttpClientHandler> configureClientHandlerHandlerAction = null)
		{
			ConfigureClientHandlerAction = configureClientHandlerHandlerAction;
		}

		/// <summary>
		/// Rents a <see cref="HttpClientProxy"/> object, encapsulated in a <see cref="PoolObject{HttpClientProxy}"/> object.
		/// </summary>
		/// <returns></returns>
		public virtual async Task<PoolObject<HttpClientProxy>> RentHttpClient()
		{
			return new PoolObject<HttpClientProxy>(await ProxyClients.TakeAsync(), proxy => ProxyClients.Add(proxy));
		}

		/// <summary>
		/// Creates a new <see cref="HttpClient"/> object with some default values, and with the <see cref="IWebProxy"/> object attached.
		/// </summary>
		/// <param name="proxy">The proxy to use for the <see cref="HttpClient"/>.</param>
		/// <returns>A new and configured <see cref="HttpClient"/> instance.</returns>
		protected virtual HttpClient CreateNewClient(IWebProxy proxy)
		{
			var handler = new HttpClientHandler
			{
				MaxConnectionsPerServer = 24,
				Proxy = proxy,
				UseCookies = false,
				UseProxy = true,
				AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
			};

			ConfigureClientHandlerAction?.Invoke(handler);

			var httpClient = new HttpClient(handler, true);

			httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Hayden/0.7.0");
			httpClient.DefaultRequestHeaders.AcceptEncoding.ParseAdd("gzip, deflate");

			return httpClient;
		}
	}
}