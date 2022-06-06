//using Mirror;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.Networking;

//namespace ZB.Gameplay.PVP
//{
//    public abstract class BaseNetworkGameManager : NetworkManager
//    {
//        public static event System.Action<int> onClientError;

//        public BaseNetworkGameRule gameRule;
//        protected float updateScoreTime;
//        protected float updateMatchTime;
//        protected bool canUpdateGameRule;
//        public readonly List<BaseNetworkGameCharacter> Characters = new List<BaseNetworkGameCharacter>();
//        public float RemainsMatchTime { get; protected set; }
//        public bool IsMatchEnded { get; protected set; }
//        public float MatchEndedAt { get; protected set; }

//        public int CountAliveCharacters()
//        {
//            var count = 0;
//            foreach (var character in Characters)
//            {
//                if (character == null)
//                    continue;
//                if (!character.IsDead)
//                    ++count;
//            }
//            return count;
//        }

//        public int CountDeadCharacters()
//        {
//            var count = 0;
//            foreach (var character in Characters)
//            {
//                if (character == null)
//                    continue;
//                if (character.IsDead)
//                    ++count;
//            }
//            return count;
//        }

//        protected override void Update()
//        {
//            base.Update();
//            if (NetworkServer.active)
//                ServerUpdate();
//            if (NetworkClient.active)
//                ClientUpdate();
//        }

//        protected virtual void ServerUpdate()
//        {
//            if (gameRule != null && canUpdateGameRule)
//                gameRule.OnUpdate();

//            if (Time.unscaledTime - updateScoreTime >= 1f)
//            {
//                var msgSendScores = new OpMsgSendScores();
//                msgSendScores.scores = GetSortedScores();
//                NetworkServer.SendToAll(msgSendScores);
//                updateScoreTime = Time.unscaledTime;
//            }

//            if (gameRule != null && Time.unscaledTime - updateMatchTime >= 1f)
//            {
//                RemainsMatchTime = gameRule.RemainsMatchTime;
//                var msgMatchStatus = new OpMsgMatchStatus();
//                msgMatchStatus.remainsMatchTime = gameRule.RemainsMatchTime;
//                msgMatchStatus.isMatchEnded = gameRule.IsMatchEnded;
//                NetworkServer.SendToAll(msgMatchStatus);

//                if (!IsMatchEnded && gameRule.IsMatchEnded)
//                {
//                    IsMatchEnded = true;
//                    MatchEndedAt = Time.unscaledTime;
//                }

//                updateMatchTime = Time.unscaledTime;
//            }
//        }

//        protected virtual void ClientUpdate()
//        {

//        }

//        public void SendKillNotify(string killerName, string victimName, string weaponId)
//        {
//            if (!NetworkServer.active)
//                return;

//            var msgKillNotify = new OpMsgKillNotify();
//            msgKillNotify.killerName = killerName;
//            msgKillNotify.victimName = victimName;
//            msgKillNotify.weaponId = weaponId;
//            NetworkServer.SendToAll(msgKillNotify);
//        }

//        public NetworkGameScore[] GetSortedScores()
//        {
//            for (var i = Characters.Count - 1; i >= 0; --i)
//            {
//                var character = Characters[i];
//                if (character == null)
//                    Characters.RemoveAt(i);
//            }
//            Characters.Sort();
//            var scores = new NetworkGameScore[Characters.Count];
//            for (var i = 0; i < Characters.Count; ++i)
//            {
//                var character = Characters[i];
//                var ranking = new NetworkGameScore();
//                ranking.netId = character.netId;
//                ranking.playerName = character.playerName;
//                ranking.score = character.Score;
//                ranking.killCount = character.KillCount;
//                ranking.assistCount = character.AssistCount;
//                ranking.dieCount = character.DieCount;
//                scores[i] = ranking;
//            }
//            return scores;
//        }

//        public void RegisterCharacter(BaseNetworkGameCharacter character)
//        {
//            if (character == null || Characters.Contains(character))
//                return;
//            character.RegisterNetworkGameManager(this);
//            Characters.Add(character);
//        }

//        public bool CanCharacterRespawn(BaseNetworkGameCharacter character, params object[] extraParams)
//        {
//            if (gameRule != null)
//                return gameRule.CanCharacterRespawn(character, extraParams);
//            return true;
//        }

//        public bool RespawnCharacter(BaseNetworkGameCharacter character, params object[] extraParams)
//        {
//            if (gameRule != null)
//                return gameRule.RespawnCharacter(character, extraParams);
//            return true;
//        }

//        public void OnUpdateCharacter(BaseNetworkGameCharacter character)
//        {
//            if (gameRule != null)
//                gameRule.OnUpdateCharacter(character);
//        }

//        public override void OnStartClient()
//        {
//            base.OnStartClient();
//            NetworkClient.RegisterHandler<OpMsgSendScores>(ReadMsgSendScores);
//            NetworkClient.RegisterHandler<OpMsgGameRule>(ReadMsgGameRule);
//            NetworkClient.RegisterHandler<OpMsgMatchStatus>(ReadMsgMatchStatus);
//            NetworkClient.RegisterHandler<OpMsgKillNotify>(ReadMsgKillNotify);
//            if (gameRule != null)
//                gameRule.InitialClientObjects();
//        }


//        public override void OnClientError(System.Exception exception)
//        {
//            base.OnClientError(exception);
//            if (onClientError != null)
//                onClientError(0);
//        }

//        protected void ReadMsgSendScores(OpMsgSendScores netMsg)
//        {
//            UpdateScores(netMsg.scores);
//        }

//        protected void ReadMsgGameRule(OpMsgGameRule netMsg)
//        {
//            BaseNetworkGameRule foundGameRule;
//            if (BaseNetworkGameInstance.GameRules.TryGetValue(netMsg.gameRuleName, out foundGameRule))
//            {
//                gameRule = foundGameRule;
//                gameRule.InitialClientObjects();
//            }
//        }

//        protected void ReadMsgMatchStatus(OpMsgMatchStatus netMsg)
//        {
//            RemainsMatchTime = netMsg.remainsMatchTime;
//            if (!IsMatchEnded && netMsg.isMatchEnded)
//            {
//                IsMatchEnded = true;
//                MatchEndedAt = Time.unscaledTime;
//            }
//        }

//        protected void ReadMsgKillNotify(OpMsgKillNotify netMsg)
//        {
//            KillNotify(netMsg.killerName, netMsg.victimName, netMsg.weaponId);
//        }

//        public override void OnStartServer()
//        {
//            base.OnStartServer();
//            NetworkServer.RegisterHandler<JoinMessage>(OnServerAddPlayer);
//            base.OnStartServer();
//            if (gameRule != null)
//                gameRule.OnStartServer(this);

//            updateScoreTime = 0f;
//            updateMatchTime = 0f;
//            RemainsMatchTime = 0f;
//            IsMatchEnded = false;
//            MatchEndedAt = 0f;
//            // If online scene == offline scene or online scene is empty assume that it can update game rule immediately
//            canUpdateGameRule = (string.IsNullOrEmpty(onlineScene) || offlineScene.Equals(onlineScene));
//        }

//        public override void OnServerReady(NetworkConnectionToClient conn)
//        {
//            base.OnServerReady(conn);
//            var msgSendScores = new OpMsgSendScores();
//            msgSendScores.scores = GetSortedScores();
//            NetworkServer.SendToReadyObservers(conn.identity, msgSendScores);
//            if (gameRule != null)
//            {
//                var msgGameRule = new OpMsgGameRule();
//                msgGameRule.gameRuleName = gameRule.name;
//                NetworkServer.SendToReadyObservers(conn.identity, msgGameRule);
//                var msgMatchTime = new OpMsgMatchStatus();
//                msgMatchTime.remainsMatchTime = gameRule.RemainsMatchTime;
//                msgMatchTime.isMatchEnded = gameRule.IsMatchEnded;
//                NetworkServer.SendToReadyObservers(conn.identity, msgMatchTime);
//            }
//        }

//        private void OnServerAddPlayer(NetworkConnectionToClient conn, JoinMessage joinMessage)
//        {
//            var character = NewCharacter(joinMessage);
//            NetworkServer.AddPlayerForConnection(conn, character.gameObject);
//            RegisterCharacter(character);
//        }

//        public override void OnServerDisconnect(NetworkConnectionToClient conn)
//        {
//            DestroyPlayerForConnection(conn);
//        }

//        public override void OnStopServer()
//        {
//            base.OnStopServer();
//            Characters.Clear();
//        }

//        public void DestroyPlayerForConnection(NetworkConnectionToClient conn)
//        {
//            var character = conn.identity.gameObject.GetComponent<BaseNetworkGameCharacter>();
//            Characters.Remove(character);
//            NetworkServer.DestroyPlayerForConnection(conn);
//        }

//        public override void OnServerSceneChanged(string sceneName)
//        {
//            base.OnServerSceneChanged(sceneName);
//            canUpdateGameRule = true;
//            if (gameRule != null)
//                gameRule.InitialClientObjects();
//        }

//        public override void OnClientSceneChanged()
//        {
//            base.OnClientSceneChanged();
//            if (gameRule != null)
//                gameRule.InitialClientObjects();
//        }

//        protected abstract BaseNetworkGameCharacter NewCharacter(JoinMessage joinMessage);
//        protected abstract void UpdateScores(NetworkGameScore[] scores);
//        protected abstract void KillNotify(string killerName, string victimName, string weaponId);

//        [System.Serializable]
//        public struct JoinMessage : NetworkMessage
//        {
//            public string playerName;
//            public string selectHead;
//            public string selectCharacter;
//            public string selectBomb;
//            public string extra;
//        }
//    }
//}