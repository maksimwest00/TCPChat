using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using TCPChat.Shared.ViewModels;

namespace TCPChat.Shared
{
    public class MyTCPServer
    {
        public TcpListener TcpListener;
        public MyTCPClient TcpClient;
        public MainWindowViewModel MainWindowViewModel;
        public MyTCPServer(MainWindowViewModel mainWindowViewModel)
        {
            this.TcpListener = new TcpListener(IPAddress.Any, 8888);
            this.MainWindowViewModel = mainWindowViewModel;
        }
        public async Task ListenAsync()
        {
            try
            {
                this.TcpListener.Start();
                this.MainWindowViewModel.ChatData = "[Сообщение на сервере] Сервер запущен.Ожидание подключений...";
                while (true)
                {
                    TcpClient tcpClient = await this.TcpListener.AcceptTcpClientAsync();
                    this.TcpClient = new MyTCPClient(tcpClient, this);
                    this.TcpClient.Reader = new StreamReader(this.TcpClient.TcpClient.GetStream());
                    this.TcpClient.Writer = new StreamWriter(this.TcpClient.TcpClient.GetStream());
                    this.MainWindowViewModel.ChatData = "[Сообщение на сервере] Клиент подключен";
                    await this.TcpClient.SendMessageAsync("Вы подключены");
                    await Task.Run(this.TcpClient.ReceiveMessageAsync);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                DisconnectClient();
            }
        }
        public async Task DisconnectClient()
        {
            if(this.TcpClient != null)
            {
                await this.TcpClient.SendMessageAsync("Сервер отключен");
                this.TcpClient.CloseConnection();
                this.MainWindowViewModel.ChatData = "[Сообщение на сервере] Клиент отключен";
            }       
        }
        public async void StopServer()
        {
            await this.DisconnectClient();
            this.TcpListener.Stop();
            this.MainWindowViewModel.ChatData = "[Сообщение на сервере] Сервер отключен";
        }
    }
}
