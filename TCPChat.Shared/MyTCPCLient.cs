using System.Diagnostics;
using System.Net.Sockets;
using TCPChat.Shared.ViewModels;

namespace TCPChat.Shared
{
    public class MyTCPClient
    {
        public TcpClient TcpClient { get; set; }
        public MyTCPServer Server { get; set; }
        public MainWindowViewModel MainWindowViewModel;
        public MyTCPClient(TcpClient tcpClient, MyTCPServer server)
        {
            this.TcpClient = tcpClient;
            this.Server = server;
            this.MainWindowViewModel = server.MainWindowViewModel;
        }
        public MyTCPClient(TcpClient tcpClient, MainWindowViewModel mainWindowViewModel)
        {
            this.TcpClient = tcpClient;
            this.MainWindowViewModel = mainWindowViewModel;
        }
        public StreamWriter Writer { get; set; }
        public StreamReader Reader { get; set; }
        public async Task ReceiveMessageAsync()
        {
            // получаем имя пользователя
            string message;
            //await this.Server.TcpClient.SendMessageAsync("[Сообщение клиента] Клиент вошел в чат");
            //this.MainWindowViewModel.ChatData = "[Сообщение клиента] Клиент вошел в чат";
            while (true)
            {
                try
                {
                    message = await this.Reader.ReadLineAsync();
                    if (message == "Сервер отключен")
                    {
                        this.MainWindowViewModel.ConnectButtonContent = "Подключиться";
                        this.MainWindowViewModel.ConnectButtonIsEnabled = true;
                    }
                    if (string.IsNullOrEmpty(message)) continue;
                    message = $"Он: {message}";
                    this.MainWindowViewModel.ChatData = $"{message}";
                }
                catch
                {
                    message = $"Он покинул чат";
                    this.MainWindowViewModel.ChatData = $"{message}";
                    break;
                }
                finally
                {
                    //this.Server.CloseConnection(); // Сделать общий CloseConnection
                    ////this.CloseConnection();
                }
            }
        }
        public async Task SendMessageAsync(string message)
        {
            this.MainWindowViewModel.ChatData = $"Вы: {message}";
            await this.Writer.WriteLineAsync(message);
            await this.Writer.FlushAsync();
        }
        public void CloseConnection()
        {
            this.Writer?.Close();
            this.Reader?.Close();
            this.TcpClient.Close();
        }
    }
}
