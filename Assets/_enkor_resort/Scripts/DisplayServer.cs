using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Concurrent;
using UnityEngine;
using UnityEngine.Video;

public class DisplayServer : MonoBehaviour
{
    private TcpListener listener;
    private TcpClient client;
    private NetworkStream stream;
    public VideoPlayer videoPlayer;

    // Очередь для безопасной передачи команд между потоками
    private ConcurrentQueue<string> commandQueue = new ConcurrentQueue<string>();
    private bool isConnected = false;
    private Thread receiveThread;

    void Start()
    {
        StartServer();
    }

    void StartServer()
    {
        try
        {
            listener = new TcpListener(IPAddress.Any, 13000);
            listener.Start();
            Debug.Log("Server started on port 13000");

            // Запускаем ожидание подключения в отдельном потоке
            Thread connectThread = new Thread(new ThreadStart(AcceptClient));
            connectThread.IsBackground = true;
            connectThread.Start();
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to start server: " + e.Message);
        }
    }

    void AcceptClient()
    {
        try
        {
            client = listener.AcceptTcpClient();
            stream = client.GetStream();
            isConnected = true;
            Debug.Log("Client connected!");

            // Запускаем поток для приема команд
            receiveThread = new Thread(new ThreadStart(ReceiveCommands));
            receiveThread.IsBackground = true;
            receiveThread.Start();
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error accepting client: " + e.Message);
        }
    }

    void ReceiveCommands()
    {
        byte[] buffer = new byte[1024];

        while (isConnected && client != null && client.Connected)
        {
            try
            {
                // Проверяем, есть ли данные для чтения
                if (stream != null && stream.DataAvailable)
                {
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead > 0)
                    {
                        string command = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        // Добавляем команду в очередь (потокобезопасно)
                        commandQueue.Enqueue(command);
                        Debug.Log("Command received: " + command);
                    }
                }
                else
                {
                    // Небольшая задержка, чтобы не нагружать CPU
                    Thread.Sleep(10);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error receiving command: " + e.Message);
                isConnected = false;
                break;
            }
        }
    }

    void Update()
    {
        // Обрабатываем команды в главном потоке Unity
        while (commandQueue.TryDequeue(out string command))
        {
            ProcessCommand(command);
        }

        // Проверяем соединение
        if (!isConnected && client != null)
        {
            CleanupConnection();
        }
    }

    void ProcessCommand(string command)
    {
        try
        {
            string[] parts = command.Trim().Split(' ');

            if (parts.Length == 0) return;

            switch (parts[0].ToUpper())
            {
                case "PLAY":
                    if (parts.Length > 1)
                    {
                        videoPlayer.url = parts[1];
                        videoPlayer.Play();
                        Debug.Log("Playing: " + parts[1]);
                    }
                    break;

                case "PAUSE":
                    videoPlayer.Pause();
                    Debug.Log("Video paused");
                    break;

                case "STOP":
                    videoPlayer.Stop();
                    Debug.Log("Video stopped");
                    break;

                case "SEEK":
                    if (parts.Length > 1 && float.TryParse(parts[1], out float time))
                    {
                        videoPlayer.time = time;
                        Debug.Log("Seek to: " + time);
                    }
                    break;

                case "VOLUME":
                    if (parts.Length > 1 && float.TryParse(parts[1], out float volume))
                    {
                        videoPlayer.SetDirectAudioVolume(0, Mathf.Clamp01(volume));
                        Debug.Log("Volume set to: " + volume);
                    }
                    break;

                case "LOOP":
                    if (parts.Length > 1 && bool.TryParse(parts[1], out bool loop))
                    {
                        videoPlayer.isLooping = loop;
                        Debug.Log("Loop set to: " + loop);
                    }
                    break;

                default:
                    Debug.LogWarning("Unknown command: " + command);
                    break;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error processing command '" + command + "': " + e.Message);
        }
    }

    void CleanupConnection()
    {
        try
        {
            if (receiveThread != null && receiveThread.IsAlive)
                receiveThread.Abort();

            stream?.Close();
            client?.Close();

            Debug.Log("Connection closed");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error cleaning up connection: " + e.Message);
        }
        finally
        {
            stream = null;
            client = null;
            isConnected = false;
        }
    }

    void OnApplicationQuit()
    {
        CleanupConnection();

        if (listener != null)
        {
            listener.Stop();
            Debug.Log("Server stopped");
        }
    }

    void OnDestroy()
    {
        CleanupConnection();
    }
}