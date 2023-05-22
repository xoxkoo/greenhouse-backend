namespace SocketServer;

public interface IWebSocketServer
{
	public Task Connect();
	public Task Disconnect();
	public Task Send(string hexData);
}
