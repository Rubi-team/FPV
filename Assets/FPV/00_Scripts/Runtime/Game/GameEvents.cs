using FPV.Runtime;

namespace FPV
{
    internal class StartMatchEvent : AppEvent
    {
        public bool IsServer { get; private set; }
        public bool IsClient { get; private set; }

        public StartMatchEvent(bool isServer, bool isClient)
        {
            IsServer = isServer;
            IsClient = isClient;
        }
    }

    internal class EndMatchEvent : AppEvent
    {
        public PlayerApplication Winner { get; private set; }

        public EndMatchEvent(PlayerApplication winner)
        {
            Winner = winner;
        }
    }

    internal class MatchResultComputedEvent : AppEvent
    {
        public ulong WinnerClientId { get; private set; }

        public MatchResultComputedEvent(ulong winnerClientId)
        {
            WinnerClientId = winnerClientId;
        }
    }

    internal class PlayerDisconnected : AppEvent
    {
        public ulong ClientId { get; private set; }
        public PlayerDisconnected(ulong clientId)
        {
            ClientId = clientId;
        }
    }
}