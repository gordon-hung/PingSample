using System.Net;
using System.Net.NetworkInformation;
using DnsClient;
using Microsoft.AspNetCore.Mvc;

namespace PingSample.Controllers;

[Route("[controller]")]
[ApiController]
public class PingController : ControllerBase
{
	/// <summary>
	/// Gets the ping asynchronous.
	/// </summary>
	/// <param name="lookupClient">The lookup client.</param>
	/// <param name="ping">The ping.</param>
	/// <param name="domain">The domain.</param>
	/// <param name="queryType">Type of the query.</param>
	/// <param name="count">The count.</param>
	/// <returns></returns>
	/// <exception cref="System.ArgumentException"></exception>
	[HttpGet("{domain}")]
	public async IAsyncEnumerable<string> GetPingAsync(
		[FromServices] ILookupClient lookupClient,
		[FromServices] Ping ping,
		string domain = "google.com",
		[FromQuery] string queryType = "A",
		[FromQuery] int count = 4)
	{
		var ttl = 0;

		// Ping the host continuously

		var result = await lookupClient.QueryAsync(
			query: domain,
			queryType: (QueryType)Enum.Parse(typeof(QueryType), queryType),
			cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(false);

		if (!IPAddress.TryParse(result.Answers.ARecords()?.FirstOrDefault()?.Address.ToString(), out var ipAddress))
			throw new ArgumentException(string.Format("{0} is not a valid IP address.", domain));

		do
		{
			var reply = await ping.SendPingAsync(
				address: ipAddress,
				timeout: TimeSpan.FromSeconds(5),
				buffer: null,
				options: null,
				cancellationToken: HttpContext.RequestAborted)
				.ConfigureAwait(false);

			yield return reply != null
			? $"Reply from {reply.Address}: " +
			$"Bytes={reply.Buffer.Length} " +
			$"Time={reply.RoundtripTime}ms " +
			$"TTL={reply.Options?.Ttl}"
			: "Ping failed.";

			ttl++;

			await Task.Delay(
				delay: TimeSpan.FromSeconds(1),
				cancellationToken: HttpContext.RequestAborted)
				.ConfigureAwait(false);
		} while (ttl < count);
	}
}
