﻿using System;
using UberStrok.Realtime.Server.Game.Core;

namespace UberStrok.Realtime.Server.Game
{
    public abstract class PeerState : State
    {
        private readonly GamePeer _peer;

        public PeerState(GamePeer peer)
        {
            if (peer == null)
                throw new ArgumentNullException(nameof(peer));

            _peer = peer;
        }

        protected GamePeer Peer => _peer;
        protected GameRoom Room => _peer.Room;

        public enum Id
        {
            None,
            Overview,
            WaitingForPlayers,
            Countdown,
            Playing,
            Killed
        }
    }
}
