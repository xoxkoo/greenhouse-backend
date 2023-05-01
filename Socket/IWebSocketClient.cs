namespace Socket;

public interface IWebSocketClient
{
	public Task Send(string hexData);
}
