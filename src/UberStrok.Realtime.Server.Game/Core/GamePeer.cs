﻿using Photon.SocketServer;
using System.Collections.Generic;
using UberStrok.Core;
using UberStrok.Core.Views;

namespace UberStrok.Realtime.Server.Game
{
    public class GamePeer : BasePeer
    {
        private readonly GamePeerEvents _events;
        private readonly StateMachineOld<PeerState.Id> _state;

        public GamePeer(InitRequest initRequest) : base(initRequest)
        {
            _events = new GamePeerEvents(this);

            _state = new StateMachineOld<PeerState.Id>();
            _state.Register(PeerState.Id.None, null);
            _state.Register(PeerState.Id.Overview, new OverviewPeerState(this));
            _state.Register(PeerState.Id.WaitingForPlayers, new WaitingForPlayersPeerState(this));
            _state.Register(PeerState.Id.Countdown, new CountdownPeerState(this));
            _state.Register(PeerState.Id.Playing, new PlayingPeerState(this));
            _state.Register(PeerState.Id.Killed, new KilledPeerState(this));

            KnownActors = new List<int>(16);
            /* Could make GamePeerOperationHandler a singleton but what ever. */
            AddOperationHandler(new GamePeerOperationHandler());
        }

        public string AuthToken { get; set; }
        public ushort Ping { get; set; }
        public GameActor Actor { get; set; }

        /* TODO: Not really sure if we need this. But might want to turn it into a HashSet. */
        public List<int> KnownActors { get; set; }
        public BaseGameRoom Room { get; set; }
        public LoadoutView Loadout { get; set; }
        public UberstrikeUserView Member { get; set; }

        public GamePeerEvents Events => _events;
        public StateMachineOld<PeerState.Id> State => _state;
    }
}
