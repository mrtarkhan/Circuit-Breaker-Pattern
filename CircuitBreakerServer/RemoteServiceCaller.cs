namespace CircuitBreakerServer;

public class RemoteServiceCaller(HttpClient httpClient) : IRemoteServiceCaller
{

    public string CallInvokeOnRemoteServer()
    {
        HttpResponseMessage result = httpClient.GetAsync("invoke").GetAwaiter().GetResult();

        result.EnsureSuccessStatusCode();

        string okResult = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();

        return okResult;

    }
    
}