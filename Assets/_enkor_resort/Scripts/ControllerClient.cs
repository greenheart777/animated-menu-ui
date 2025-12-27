using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class ControllerClient : MonoBehaviour
{
    private TcpClient client;
    private NetworkStream stream;

    private void Start()
    {
        ConnectToDisplayApp();
    }

    private void ConnectToDisplayApp()
    {
        client = new TcpClient("127.0.0.1", 13000);
        stream = client.GetStream();
    }

    public void SendCommand(string command)
    {
        byte[] data = Encoding.UTF8.GetBytes(command);
        stream.Write(data, 0, data.Length);
    }

    private void OnDestroy()
    {
        stream?.Close();
        client?.Close();
    }
}