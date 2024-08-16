namespace CircuitBreakerServer;

public class CircuitManager
{
    public int StayInOpenStateForXSeconds { get; set; } = 30;
    public StateEnum State { get; set; } = StateEnum.Close;
    public DateTime? LastFailure { get; set; } = null;
    public bool ThereIsRequestInHalfOpenState { get; set; } = false;

    public void OpenCircuit()
    {
        if (State == StateEnum.Open)
            return;
        
        State = StateEnum.Open;
        LastFailure = DateTime.Now;
    }
    
    private void HalfOpenCircuit()
    {
        State = StateEnum.HalfOpen;
    }
    
    public void CloseCircuit()
    {
        if (State != StateEnum.Close)
        {
            ThereIsRequestInHalfOpenState = false;
            State = StateEnum.Close;
        }
    }
    
    public bool CanSendRequest()
    {
        if (State == StateEnum.Close)
            return true;

        if (State == StateEnum.Open)
        {
            if ((DateTime.Now - LastFailure!.Value).Seconds < StayInOpenStateForXSeconds)
            {
                return false;
            }

            HalfOpenCircuit();
        }

        if (State == StateEnum.HalfOpen && ThereIsRequestInHalfOpenState == false)
        {
            ThereIsRequestInHalfOpenState = true;
            return true;
        }

        return false;

    }
}

public enum StateEnum
{
    Close,
    Open,
    HalfOpen
}