namespace IRO.MessageBus.Abstract
{
    public interface IBusResponse
    {
    }

    public interface IBusResponse<TBusRequest>: IBusResponse
        where TBusRequest : IBusRequest
    {
    }
}