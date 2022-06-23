namespace IRO.MessageBus.Abstract
{
    public interface IBus
    {
        Task Call<TBusRequest>(TBusRequest req, CancellationToken cancellationToken = default);

        Task Call(string requestName, IBusRequest request, CancellationToken cancellationToken = default);
    }
}