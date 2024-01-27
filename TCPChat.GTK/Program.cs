using Gtk;
using System;
using TCPChat.GTK.Views;

namespace TCPChat.GTK
{
    internal class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            Application.Init();
            var app = new Application("org.TCPChat.GTK.TCPChat.GTK", GLib.ApplicationFlags.None);
            ////app.Register(GLib.Cancellable.Current);
            var win = new MainWindow();
            app.AddWindow(win);
            win.Show();
            Application.Run();
        }
    }
}
