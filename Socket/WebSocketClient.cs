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
    class WebSocketClient
    {
        public static async Task Main()
        {

                ClientWebSocket webSocket = new ClientWebSocket();
                Context context = new Context();
                IConverter converter = new Converter(new TemperatureLogic(new TemperatureEfcDao(context)), new CO2Logic(new CO2EfcDao(context)), new HumidityLogic(new HumidityEfcDao(context)));

                // Connect to the WebSocket server
                try
                {
                    Console.WriteLine("Connecting to WebSocket server...");
                    Uri serverUri = new Uri("wss://iotnet.teracom.dk/app?token=vnoUcQAAABFpb3RuZXQudGVyYWNvbS5ka-iuwG5H1SHPkGogk2YUH3Y=");
                    await webSocket.ConnectAsync(serverUri, CancellationToken.None);
                    Console.WriteLine("connected :)");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }

                // Send a message to the server
                async void Send()
                {
                    string messageToSend = "Hello server!";
                    byte[] sendBuffer = Encoding.ASCII.GetBytes(messageToSend);
                    await webSocket.SendAsync(new ArraySegment<byte>(sendBuffer), WebSocketMessageType.Text, true, CancellationToken.None);
                }

                // Continuously listen for incoming messages
                byte[] receiveBuffer = new byte[1024];
                while (webSocket.State == WebSocketState.Open)
                {
                    Console.WriteLine("listening...");
                    WebSocketReceiveResult receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);
                    if (receiveResult.MessageType == WebSocketMessageType.Text)
                    {
                        string message = Encoding.ASCII.GetString(receiveBuffer, 0, receiveResult.Count);
                        dynamic response = JsonConvert.DeserializeObject(message);
                        Console.WriteLine(response["data"]);
                        await converter.ConvertFromHex(response["data"].ToString());
                        Console.WriteLine($"Received message: {message}");
                    }
                }

                // Close the WebSocket connection
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
            }


    }
}
