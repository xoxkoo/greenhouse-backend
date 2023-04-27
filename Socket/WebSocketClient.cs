using System.Net.WebSockets;
using System.Text;
using Application.LogicInterfaces;
using Newtonsoft.Json;

namespace Socket
{
    public class WebSocketClient
    {
        private readonly ClientWebSocket _webSocket;
        private readonly IConverter _converter;

        public WebSocketClient(IConverter converter)
        {

	        _webSocket = new ClientWebSocket();
            _converter = converter;
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


        /**
         * Listen and receive message from the websocket server
         */
        private async Task ConnectAndListen()
        {

			await Connect();

            byte[] receiveBuffer = new byte[256];

	        Console.WriteLine("Listening...");

            while (_webSocket.State == WebSocketState.Open)
            {

	            WebSocketReceiveResult receiveResult = await _webSocket.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);
	            if (receiveResult.MessageType == WebSocketMessageType.Text)
	            {
		            string message = Encoding.ASCII.GetString(receiveBuffer, 0, receiveResult.Count);

		            //todo maybe this should be handled differently
		            // check for the object that we want to receive
		            if (message.Substring(1,10).Equals("\"cmd\":\"rx\""))
		            {
			            Console.WriteLine($"Received message: {message}");

			            try
			            {
				            // deserialize message into object
				            dynamic? response = JsonConvert.DeserializeObject(message);
				            if (response != null)
				            {
					            // check if response[cmd] is equal to 'rx'
					            // checking for response we need
					            if (response["cmd"] == "rx")
					            {
						            if (! response["data"].Equals(""))
						            {

							            try
							            {
											// call convertor method to convert data from hexadecimal representation
								            var converterResponse = await _converter.ConvertFromHex(response["data"].ToString());
								            Console.WriteLine($"Convertor: {converterResponse}");
							            }
							            catch (Exception e)
							            {
								            Console.WriteLine(e);

							            }
						            }
					            }

				            }
				            Console.WriteLine("Listening...");
			            }
			            catch (Exception e)
			            {
				            Console.WriteLine("Object was not deserialized: " + e);
			            }

		            }


	            }


            }

            await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
        }

        public async Task Send(string message)
        {
	        await Connect();

	        var json = JsonConvert.SerializeObject(new
	        {
		        cmd = "tx",
		        EUI = "0004A30B00E7E072",
		        port = 1,
		        confirmed = true,
		        data = "0102AABB"
	        });

	        var buffer = Encoding.UTF8.GetBytes(json);
	        var segment = new ArraySegment<byte>(buffer);
	        //await _webSocket.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);

            byte[] sendBuffer = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(json));
            await _webSocket.SendAsync(new ArraySegment<byte>(sendBuffer), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        /**
         * Start websocket client
         */
        public void Run()
        {
	        var task = Task.Run(ConnectAndListen);
	        task.Wait();
        }

    }
}
