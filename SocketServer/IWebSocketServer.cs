namespace SocketServer;

public interface IWebSocketServer
{
	public Task Send(string hexData);
}
