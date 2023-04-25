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

            byte[] receiveBuffer = new byte[1024];
            while (_webSocket.State == WebSocketState.Open)
            {
	            Console.WriteLine("Listening...");
	            WebSocketReceiveResult receiveResult = await _webSocket.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);
	            if (receiveResult.MessageType == WebSocketMessageType.Text)
	            {
		            string message = Encoding.ASCII.GetString(receiveBuffer, 0, receiveResult.Count);
		            Console.WriteLine($"Received message: {message}");

		            try
		            {
			            // deserialize message into object
						dynamic? response = JsonConvert.DeserializeObject(message);
						if (response != null)
						{
							if (! response["data"].Equals(""))
							{

								// call convertor method to convert data from hexadecimal representation
								var converterResponse = await _converter.ConvertFromHex(response["data"].ToString());
								Console.WriteLine($"Convertor: {converterResponse}");
							}

						}
		            }
		            catch (Exception e)
		            {
			            Console.WriteLine("Object was not deserialized: " + e);
			            throw;
		            }

	            }
            }

            await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
        }

        public async Task Send(string message)
        {
	        await Connect();

            byte[] sendBuffer = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(message));
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
