//using Mirror;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;
//using UnityEngine.Networking;

//namespace ZB.Gameplay.PVP
//{
//    [RequireComponent(typeof(GameNetworkDiscovery))]
//    public class GameNetworkManager : BaseNetworkGameManager
//    {
//        public static new GameNetworkManager Singleton
//        {
//            get { return singleton as GameNetworkManager; }
//        }

//        private JoinMessage MakeJoinMessage()
//        {
//            var msg = new JoinMessage();
//            msg.playerName = PlayerSave.GetPlayerName();
//            msg.selectHead = GameInstance.GetAvailableHead(PlayerSave.GetHead()).GetId();
//            msg.selectCharacter = GameInstance.GetAvailableCharacter(PlayerSave.GetCharacter()).GetId();
//            msg.selectBomb = GameInstance.GetAvailableBomb(PlayerSave.GetBomb()).GetId();
//            return msg;
//        }

//        public override void OnClientConnect()
//        {
//            //if the client connected but did not load the online scene
//            if (!clientLoadedScene)
//            {
//                //Ready/AddPlayer is usually triggered by a scene load completing (see OnClientSceneChanged).
//                //if no scene was loaded, maybe because we don't have separate online/offline scenes, then do it here.
//                NetworkClient.Ready();
//                if (autoCreatePlayer && NetworkClient.localPlayer == null)
//                    NetworkClient.connection.Send(MakeJoinMessage());
//            }
//        }

//        public override void OnClientSceneChanged()
//        {
//            Debug.Log("OnClientSceneChanged");
//            // always become ready.
//            NetworkClient.Ready();

//            bool addPlayer = (NetworkClient.spawned.Count == 0);
//            bool foundPlayer = NetworkClient.spawned.Any(x => x.Value.gameObject != null);

//            // there are players, but their game objects have all been deleted
//            if (!foundPlayer)
//                addPlayer = true;

//            if (addPlayer)
//                NetworkClient.connection.Send(MakeJoinMessage());
//        }

//        protected override BaseNetworkGameCharacter NewCharacter(JoinMessage joinMessage)
//        {
//            var character = Instantiate(GameInstance.Singleton.characterPrefab);
//            character.playerName = joinMessage.playerName;
//            character.selectHead = joinMessage.selectHead;
//            character.selectBomb = joinMessage.selectBomb;
//            character.selectCharacter = joinMessage.selectCharacter;
//            character.extra = joinMessage.extra;
//            return character;
//        }

//        protected override void UpdateScores(NetworkGameScore[] scores)
//        {
//            var uiGameplay = FindObjectOfType<UIGameplay>();
//            if (uiGameplay != null)
//                uiGameplay.UpdateRankings(scores);
//        }

//        protected override void KillNotify(string killerName, string victimName, string weaponId)
//        {
//            var uiGameplay = FindObjectOfType<UIGameplay>();
//            if (uiGameplay != null)
//                uiGameplay.KillNotify(killerName, victimName, weaponId);
//        }
//    }
//}