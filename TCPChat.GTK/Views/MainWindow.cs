using Gtk;
using GtkSharp.Mvvm;
using TCPChat.Shared.ViewModels;
using UI = Gtk.Builder.ObjectAttribute;

namespace TCPChat.GTK.Views
{
    public class MainWindow : Window
    {
        #region UI Connect Layout
        [UI] private Gtk.Label ConnectLabel = null;
        [UI] private Gtk.Entry ConnectEntry = null;
        [UI] private Gtk.CheckButton ConnectCheckButton = null;
        [UI] private Gtk.Button ConnectButton = null;
        #endregion
        #region UI Chat Layout
        [UI] private Gtk.TextView ChatTextView = null;
        [UI] private Gtk.Entry ChatSendMessageEntry = null;
        [UI] private Gtk.Button ChatSendMessageButton = null;
        #endregion

        private MainWindowViewModel ViewModel = null;
        public MainWindow() : this(new Builder("MainWindow.glade")) { }
        private MainWindow(Builder builder) : base(builder.GetRawOwnedObject("MainWindow"))
        {
            builder.Autoconnect(this);
            this.DeleteEvent += Window_DeleteEvent;
            this.ViewModel = new MainWindowViewModel();
            BinableUIToViewModelProps();
        }

        private void BinableUIToViewModelProps()
        {
            this.ConnectLabel.Text = "Введите IP Адрес:";

            this.ConnectEntry.Bind(x => x.Text, this.ViewModel, x => x.IPAddressText);
            this.ConnectEntry.BindBack(x => x.Text, () => this.ViewModel.IPAddressText);

            this.ConnectCheckButton.Bind(x => x.Active, this.ViewModel, x => x.IsServer);
            this.ConnectCheckButton.BindBack(x => x.Active, () => this.ViewModel.IsServer);

            this.ConnectButton.Bind(x => x.Label, this.ViewModel, x => x.ConnectButtonContent);
            this.ConnectButton.Bind(x => x.Sensitive, this.ViewModel, x => x.ConnectButtonIsEnabled);
            this.ConnectButton.BindCommand(this.ViewModel, x => x.ConnectButtonCommand);

            //this.ChatTextView.Bind(x => x.Buffer.Text, this.ViewModel, x => x.ChatData);

            this.ViewModel.ObservePath(x => x.ChatData)
                .Subscribe(x => this.ChatTextView.Buffer.Text = x);

            this.ChatSendMessageEntry.Bind(x => x.Text, this.ViewModel, x => x.MessageData);
            this.ChatSendMessageEntry.BindBack(x => x.Text, () => this.ViewModel.MessageData);

            this.ChatSendMessageButton.Bind(x => x.Sensitive, this.ViewModel, x => x.SendMessageBtnIsEnabled);
            this.ChatSendMessageButton.BindCommand(this.ViewModel, x => x.SendMessageCommand);
        }

        private void Window_DeleteEvent(object sender, DeleteEventArgs a)
        {
            Application.Quit();
        }
    }
}
