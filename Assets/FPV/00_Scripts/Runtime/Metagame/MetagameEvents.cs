namespace FPV
{
    internal class StartSinglePlayerModeEvent : AppEvent { }
    
    internal class MatchLoadingEvent : AppEvent { }
    internal class ExitMatchLoadingEvent : AppEvent { }
    
    internal class CreateRelayEvent : AppEvent { }
    
    internal class JoinRelayEvent : AppEvent { 
        public string RelayId { get; private set; }
        public JoinRelayEvent(string relayId)
        {
            RelayId = relayId;
        }
    }
    
    internal class PlayerSignedOut : AppEvent { }
    internal class PlayerSignedIn : AppEvent
    {
        public bool Success { get; private set; }
        public string PlayerId { get; private set; }

        public PlayerSignedIn(bool success, string playerId)
        {
            Success = success;
            PlayerId = playerId;
        }
    }
}