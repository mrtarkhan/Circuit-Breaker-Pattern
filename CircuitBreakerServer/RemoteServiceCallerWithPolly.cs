using Polly;
using Polly.CircuitBreaker;

namespace CircuitBreakerServer;

public class RemoteServiceCallerWithPolly([FromKeyedServices("bare")] IRemoteServiceCaller remoteServiceCaller) : IRemoteServiceCaller
{
    public string CallInvokeOnRemoteServer()
    {
        // var policy = Policy
        //         .Handle<Exception>()
        //         .CircuitBreaker(3, TimeSpan.FromSeconds(30))
        //         .Execute(remoteServiceCaller.CallInvokeOnRemoteServer)
        //     ;

        try
        {
            return remoteServiceCaller.CallInvokeOnRemoteServer();
        }
        catch (BrokenCircuitException e)
        {
            return "circuit is open";
        }
    }
}