namespace IRO.MessageBus.Abstract
{
    public interface IBusHandler<TBusRequest, TBusResponse>:IBusHandler
          where TBusRequest : IBusRequest
          where TBusResponse : IBusResponse<TBusRequest>
    {
        Task<TBusResponse> Handle(TBusRequest request);
    }

    public interface IBusHandler
    {
    }
}