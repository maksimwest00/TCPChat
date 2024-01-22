using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;

namespace TCPChat.Shared.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        #region Properties

        #region ConnectionChapterProperties
        private string _ipAddressText = IPAddress.Any.ToString();
        public string IPAddressText 
        {
            get
            {
                return _ipAddressText;
            }
            set
            {
                _ipAddressText = value;
                OnPropertyChanged();
            }
        }
        private bool _isServer;
        public bool IsServer
        {
            get
            {
                return _isServer;
            }
            set
            {
                if(value is true)
                {
                    this.ConnectButtonContent = "Включить сервер";
                } else
                {
                    this.ConnectButtonContent = "Подключиться";
                }
                _isServer = value;
                OnPropertyChanged();
            }
        }
        private string _connectButtonContent = "Подключиться";
        public string ConnectButtonContent
        {
            get
            {
                return _connectButtonContent;
            }
            set
            {
                _connectButtonContent = value;
                OnPropertyChanged();
            }
        }
        private bool _connectButtonIsEnabled = true;
        public bool ConnectButtonIsEnabled
        {
            get
            {
                return _connectButtonIsEnabled;
            }
            set
            {
                _connectButtonIsEnabled = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region ChatChapterProperties
        private string _chatData = string.Empty;
        public string ChatData
        {
            get
            {
                return _chatData;
            }
            set
            {
                _chatData += $"\n {value}";
                OnPropertyChanged();
            }
        }
        private string _messageData;
        public string MessageData
        {
            get
            {
                return _messageData;
            }
            set
            {
                _messageData = value;
                if(value != string.Empty)
                {
                    SendMessageBtnIsEnabled = true;
                } 
                else
                {
                    SendMessageBtnIsEnabled = false;
                }
                OnPropertyChanged();
            }
        }
        private bool _sendMessageBtnIsEnabled = false;
        public bool SendMessageBtnIsEnabled
        {
            get
            {
                return _sendMessageBtnIsEnabled;
            }
            set
            {
                _sendMessageBtnIsEnabled = value;
                OnPropertyChanged();
            }
        }
        #endregion


        #endregion

        #region Commands
        public RelayCommand ConnectButtonCommand { get; set; }
        public RelayCommand SendMessageCommand { get; set; }
        #endregion

        #region Commands Actions
        private async void ConnectButtonCommandAction(object obj = null)
        {
            this.ConnectButtonIsEnabled = false;
            if(this.ConnectButtonContent is "Подключиться" ||
               this.ConnectButtonContent is "Включить сервер")
            {
                if (this.IsServer)
                {
                    this.ConnectButtonContent = "Отключить сервер";
                    MyTCPServer server = new MyTCPServer(this);
                    this.serverPerem = server;
                    this.ConnectButtonIsEnabled = true;
                    await server.ListenAsync();
                }
                else
                {
                    this.ConnectButtonContent = "Отключиться";
                    MyTCPClient client = new MyTCPClient(new TcpClient(), this);
                    this.clientPerem = client;
                    try
                    {
                        await Task.Run(async () =>
                        {
                            client.TcpClient.Connect("127.0.0.1", 8888);
                            client.Reader = new StreamReader(client.TcpClient.GetStream());
                            client.Writer = new StreamWriter(client.TcpClient.GetStream());
                            await Task.Run(this.clientPerem.ReceiveMessageAsync);
                        });
                        this.ConnectButtonIsEnabled = true;
                    }
                    catch
                    {
                        this.ChatData = "Подключение не удалось";
                        this.ConnectButtonContent = "Подключиться";            
                    }
                    finally
                    {
                        this.ConnectButtonIsEnabled = true;
                    }
                }           
            } 
            else
            {
                if (this.IsServer)
                {
                    this.serverPerem.StopServer();
                    this.ConnectButtonContent = "Включить сервер";
                }
                else
                {
                    this.ConnectButtonContent = "Подключиться";
                    this.clientPerem.CloseConnection();
                }
                this.ConnectButtonIsEnabled = true;
            }
        }
        public MyTCPServer serverPerem { get; set; }
        public MyTCPClient clientPerem { get; set; }
        private async void SendMessageCommandAction(object obj = null)
        {
            if (IsServer)
            {
                await serverPerem.TcpClient.SendMessageAsync(this.MessageData);
            }
            else
            {
                await clientPerem.SendMessageAsync(this.MessageData);
            }       
            this.MessageData = string.Empty;
        }
        #endregion
        public MainWindowViewModel()
        {
            this.ConnectButtonCommand = new RelayCommand(ConnectButtonCommandAction);
            this.SendMessageCommand = new RelayCommand(SendMessageCommandAction);
        }   
    }
}
