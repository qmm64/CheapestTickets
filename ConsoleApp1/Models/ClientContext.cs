using System.Net.Sockets;

internal class ClientContext
{
    public Guid Id { get; }
    public string Ip { get; }
    public TcpClient TcpClient { get; }
    public CancellationTokenSource TokenSource { get; }

    public NetworkStream Stream => TcpClient.GetStream();

    public ClientContext(TcpClient tcpClient)
    {
        TcpClient = tcpClient;
        Ip = tcpClient.Client.RemoteEndPoint?.ToString() ?? "неизвестный IP";
        Id = Guid.NewGuid();
        TokenSource = new CancellationTokenSource();
    }

    public void Cancel()
    {
        TokenSource.Cancel();
        Stream.Close();
        TcpClient.Close();
    }
}
