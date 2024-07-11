namespace demo_chatHub
{
    public interface IChatClient
    {
        Task ReceiveMessage(string message);
    }
}
