﻿using System.IO;
using UberStrok.Core.Serialization;
using UberStrok.Core.Serialization.Views;
using UberStrok.Core.Views;

namespace UberStrok.Realtime.Server.Comm
{
    public class CommPeerEvents : BaseEventSender
    {
        public LobbyRoomEvents Lobby { get; }

        public CommPeerEvents(BasePeer peer) : base(peer)
        {
            Lobby = new LobbyRoomEvents(peer);
        }

        public void SendDisconnectAndDisablePhoton(string message = "An error occured that forced UberStrike to halt.")
        {
            using (var bytes = new MemoryStream())
            {
                StringProxy.Serialize(bytes, message);
                SendEvent((byte)ICommPeerEventsType.DisconnectAndDisablePhoton, bytes);
            }
        }

        public void SendHeartbeatChallenge(string challengeHash)
        {
            using (var bytes = new MemoryStream())
            {
                StringProxy.Serialize(bytes, challengeHash);
                SendEvent((byte)ICommPeerEventsType.HeartbeatChallenge, bytes);
            }
        }

        public void SendLoadData(ServerConnectionView serverConnection)
        {
            using (var bytes = new MemoryStream())
            {
                ServerConnectionViewProxy.Serialize(bytes, serverConnection);
                SendEvent((byte)ICommPeerEventsType.LoadData, bytes);
            }
        }
       
        public void SendLobbyEntered()
        {
            using (var bytes = new MemoryStream())
                SendEvent((byte)ICommPeerEventsType.LobbyEntered, bytes);
        }
    }
}
