﻿using log4net;
using System;
using System.Collections.Generic;
using UberStrok.Core.Views;

namespace UberStrok.Realtime.Server.Game
{
    public class GamePeerOperationHandler : BaseGamePeerOperationHandler
    {
        public GamePeerOperationHandler(GamePeer peer) : base(peer)
        {
            // Space
        }

        protected override void OnGetGameListUpdates()
        {
            //TODO: Don't use that, cause apparently there is a FullGameList event which we can send to wipe stuff and things.
            // Clear the client list of games available.
            Peer.Events.SendGameListUpdateEnd();

            var rooms = new List<GameRoomDataView>(GameManager.Instance.Rooms.Count);
            foreach (var room in GameManager.Instance.Rooms)
                rooms.Add(room.Value.Data);

            LogManager.GetLogger(typeof(GamePeerOperationHandler)).Info($"Room count -> {rooms.Count}");
            Peer.Events.SendGameListUpdate(rooms, new List<int>());
        }

        protected override void OnGetServerLoad()
        {
            var view = new PhotonServerLoadView
            {
                Latency = Peer.RoundTripTime / 2, // UberStrike does not care about this value, it uses its client side value.
                State = PhotonServerLoadView.Status.Alive,
                MaxPlayerCount = 100,

                PeersConnected = GameApplication.Instance.PeerCount,
                PlayersConnected = 1, // UberStrike also does not care about this value, it uses PeersConnected.

                RoomsCreated = GameManager.Instance.Rooms.Count,
                TimeStamp = DateTime.UtcNow
            };

            Peer.Events.SendServerLoadData(view);
        }

        protected override void OnCreateRoom(GameRoomDataView roomData, string password, string clientVersion, string authToken, string magicHash)
        {
            //TODO: Talk to the web server to get the player data off the authToken.
            GameManager.Instance.AddRoom(roomData, password);

            LogManager.GetLogger(typeof(GamePeerOperationHandler)).Info($"Creating new room -> {roomData.Name}:{roomData.Number}");
        }

        protected override void OnJoinRoom(int roomId, string password, string clientVersion, string authToken, string magicHash)
        {
            var room = default(GameManager.Room);
            if (GameManager.Instance.Rooms.TryGetValue(roomId, out room))
            {
                LogManager.GetLogger(typeof(GamePeerOperationHandler)).Info("Joining stuff ey!");
                if (room.Data.IsPasswordProtected)
                {
                    //TODO: Check password.
                    Peer.Events.SendRoomEntered(room.Data);
                }
                else
                {
                    Peer.Game = new GameRoom(Peer, room);
                    Peer.Events.SendRoomEntered(room.Data);

                    Peer.Game.Room.Data.ConnectedPlayers++;
                }
            }
            else
            {
                // SendRoomEnterFailed
            }
        }

        protected override void OnLeaveRoom()
        {
            //TODO: Kill room if the number of connected players is 0.
            //TODO: Thread safety cause we need it, possibly.

            Peer.Game.Room.Data.ConnectedPlayers--;
            Peer.Game = null;
        }

        protected override void OnUpdatePing(ushort ping)
        {
            //TODO: Client should not have the ability to change its ping but it can cause uberstrike.
            Peer.Ping = ping;
        }
    }
}