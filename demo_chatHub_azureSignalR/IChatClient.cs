namespace demo_chatHub_azureSignalR
{
    public interface IChatClient
    {
        Task ReceiveMessage(string message);
    }
}
