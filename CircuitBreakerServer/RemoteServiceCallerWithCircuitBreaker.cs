namespace CircuitBreakerServer;

public class RemoteServiceCallerWithCircuitBreaker(
    [FromKeyedServices("bare")] IRemoteServiceCaller remoteServiceCaller,
    CircuitManager circuitManager
    ) : IRemoteServiceCaller
{
    public string CallInvokeOnRemoteServer()
    {
        try
        {
            if (circuitManager.CanSendRequest())
            {
                circuitManager.CloseCircuit();
                return remoteServiceCaller.CallInvokeOnRemoteServer();
            }

            return "circuit is open";
        }
        catch (Exception e)
        {
            circuitManager.OpenCircuit();
            return "circuit is open";
        }
    }
}