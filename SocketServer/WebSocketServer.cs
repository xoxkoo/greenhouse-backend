using System.Net.WebSockets;
using System.Text;
using Newtonsoft.Json;

namespace SocketServer;

public class WebSocketServer : IWebSocketServer
{
	private readonly ClientWebSocket _webSocket;

	public WebSocketServer()
	{
		_webSocket = new ClientWebSocket();
	}

	/**
	 * Connect to the websocket server with  uri
	 */
	private async Task Connect()
	{
		try
		{
			Console.WriteLine("Connecting to WebSocket server...");
			Uri serverUri = new Uri("wss://iotnet.teracom.dk/app?token=vnoUcQAAABFpb3RuZXQudGVyYWNvbS5ka-iuwG5H1SHPkGogk2YUH3Y=");

			await _webSocket.ConnectAsync(serverUri, CancellationToken.None);

			Console.WriteLine("Connected :)");
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			throw;
		}
	}

	public async Task Send(string hexData)
	{
		await Connect();

		if (_webSocket.State == WebSocketState.Open)
		{
			var json = JsonConvert.SerializeObject(new
			{
				cmd = "tx",
				EUI = "0004A30B00E7E072",
				port = 6,
				confirmed = true,
				data = hexData
			});

			byte[] sendBuffer = Encoding.UTF8.GetBytes(json);
			await _webSocket.SendAsync(new ArraySegment<byte>(sendBuffer), WebSocketMessageType.Text, true, CancellationToken.None);

			Console.WriteLine("Message sent!");
		}
		else
		{
			Console.WriteLine("WebSocket connection is not open!");
		}
	}
}
