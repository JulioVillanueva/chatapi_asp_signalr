using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client2
{
    public class SignalrService
    {

        private Action<string> _receiveMessage;
        public Action<string> ConnectionReconnected;
        public Action<Exception> ConnectionReconnecting;
        public Action<Exception> ConnectionClosed;
        HubConnection _connection;
        public void SetupConnection(Action<string> receiveMessage, string url)
        {
            _receiveMessage = receiveMessage;
            _connection = new HubConnectionBuilder()
            .WithUrl(url)
            .Build();
            _connection.Closed += OnConnectionClosed;
            _connection.Reconnecting += OnConnectionReconnecting;
            _connection.Reconnected += OnConnectionReconnected;
            _connection.On<string>("ReceiveMessage", _receiveMessage);
            
        }
        public async Task StopConnection()
        {
            await _connection.StopAsync();
        }
        public async Task StartConnection()
        {
            try
            {
                await _connection.StartAsync();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public async Task SendMessage(string message)
        {
            await _connection.SendAsync("Broadcast",message);
        }

        private async Task OnConnectionReconnected(string arg)
        {
            this.ConnectionReconnected?.Invoke(arg);
        }

        private async Task OnConnectionReconnecting(Exception exception)
        {
            this.ConnectionReconnecting?.Invoke(exception);
        }

        private async Task OnConnectionClosed(Exception exception)
        {
            this.ConnectionClosed?.Invoke(exception);
        }
    }
}
