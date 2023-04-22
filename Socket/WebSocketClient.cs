using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading.Tasks;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Application.Logic;
using Application.LogicInterfaces;
using EfcDataAccess;
using EfcDataAccess.DAOs;
using Newtonsoft.Json;

namespace Socket
{
    public class WebSocketClient
    {
        private ClientWebSocket _webSocket;
        private readonly IConverter _converter;

        public WebSocketClient(IConverter converter)
        {

	        _webSocket = new ClientWebSocket();
            _converter = converter;
        }

        public async Task Run()
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

            Console.WriteLine('a');

            byte[] receiveBuffer = new byte[1024];
            while (_webSocket.State == WebSocketState.Open)
            {
	            Console.WriteLine("Listening...");
	            WebSocketReceiveResult receiveResult = await _webSocket.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);
	            if (receiveResult.MessageType == WebSocketMessageType.Text)
	            {
		            string message = Encoding.ASCII.GetString(receiveBuffer, 0, receiveResult.Count);
		            Console.WriteLine($"Received message: {message}");

		            // todo validation
		            dynamic response = JsonConvert.DeserializeObject(message);
		            Console.WriteLine(response["data"]);

		            // todo print that data was saved
		            await _converter.ConvertFromHex(response["data"].ToString());
	            }
            }

            Console.WriteLine('b');
            await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
        }

        public async Task Send(string message)
        {
	        try
	        {
		        _webSocket = new ClientWebSocket();
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

            byte[] sendBuffer = Encoding.ASCII.GetBytes(message);
            await _webSocket.SendAsync(new ArraySegment<byte>(sendBuffer), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        // public static void StartWebSocketClient()
        // {
	       //  // Run the WebSocket client in a separate thread
	       //  Thread webSocketThread = new Thread(async () =>
	       //  {
		      //   await WebSocketClient.Connect();
	       //  });
        //
	       //  webSocketThread.Start();
        // }
    }
}
