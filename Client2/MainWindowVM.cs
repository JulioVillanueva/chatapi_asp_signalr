using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WPFtoolkitFramework.ViewModels;

namespace Client2
{
    public class MainWindowVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged([CallerMemberName] string name = "")
        {
            this.PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(name));
        }
        private string _messageList;

        public string MesageList
        {
            get { return _messageList; }
            set { _messageList = value; NotifyPropertyChanged(); }
        }

        public RelayCommand<object> ConnectCommand { get; }
        public RelayCommand<object> SendCommand { get; }
        public RelayCommand<object> CloseCommand { get; }

        private SignalrService _signalrService;
        private bool _connectedOrConnecting;
        private const string _url = "https://app-demochatsigrservice-uksouth-dev-001.azurewebsites.net/public-chat/chat-hub";
        public MainWindowVM()
        {
            MesageList = ""; 
            ConnectCommand = new RelayCommand<object>(Connect, CanConnect);
            SendCommand = new RelayCommand<object>(Send, CanSend);
            CloseCommand = new RelayCommand<object>(Close);
            _signalrService = new SignalrService();
            _connectedOrConnecting = false;
            _signalrService.SetupConnection(AddMessage, _url);
            _signalrService.ConnectionClosed += (_) => _connectedOrConnecting = false;
            _signalrService.ConnectionReconnected += (_) => _connectedOrConnecting = true;
            _signalrService.ConnectionReconnecting += (_) => _connectedOrConnecting = true;
        }

        private bool CanSend(object obj)
        {
            return _connectedOrConnecting == true;
        }

        private void Close(object obj)
        {
            _signalrService.StopConnection();
        }

        private void Send(object obj)
        {
            _signalrService.SendMessage(obj as string);
        }

        private void AddMessage(string message)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>  MesageList += $"\n\r {message}"));
        }

        private bool CanConnect(object obj)
        {
            return _connectedOrConnecting == false;
        }

        private void Connect(object obj)
        {
            ConnectAsync();
        }

        private async Task ConnectAsync()
        {
            _connectedOrConnecting = true;
            try
            {
                await _signalrService.StopConnection();
                await _signalrService.StartConnection();
                _connectedOrConnecting = true;

            }
            catch (Exception)
            {
                _connectedOrConnecting = false;
                throw;
            }
        }
    }
}
