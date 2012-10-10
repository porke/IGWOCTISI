﻿namespace Client.State
{
    using View;
    using View.Lobby;
    using System;
    using Common;
    using System.Collections.Generic;
    using Client.Model;

    class LobbyState : GameState
    {
        public LobbyState(IGWOCTISI game) : base(game)
        {
            var menuBackground = new LobbyBackground(this);
            var lobbyMenu = new MainLobbyView(this);

            ViewMgr.PushLayer(menuBackground);
            ViewMgr.PushLayer(lobbyMenu);

            eventHandlers.Add("LeaveGameLobby", LeaveGameLobby);
            eventHandlers.Add("CreateGame", CreateGame);
            eventHandlers.Add("CancelCreateGame", CancelCreateGame);
            eventHandlers.Add("EnterCreateGameView", EnterCreateGameView);
            eventHandlers.Add("Logout", Logout);
            eventHandlers.Add("JoinGame", JoinGame);
            eventHandlers.Add("BeginGame", BeginGame);
            eventHandlers.Add("RefreshGameList", RefreshGameList);
        }

        public override void OnEnter()
        {
            var network = Client.Network;
            this.RefreshGameList(new SenderEventArgs(ViewMgr.PeekLayer()));
        }        

        #region Event handlers

        private void LeaveGameLobby(EventArgs args)
        {
            ViewMgr.PopLayer(); // pop game lobby
            ViewMgr.PushLayer(new MainLobbyView(this));
        }

        private void CreateGame(EventArgs args)
        {
            ViewMgr.PopLayer();     // pop main lobby window
            ViewMgr.PushLayer(new GameLobbyView(this));
        }

        private void CancelCreateGame(EventArgs args)
        {
            ViewMgr.PopLayer();     // pop create game view
            ViewMgr.PushLayer(new MainLobbyView(this));
        }

        private void EnterCreateGameView(EventArgs args)
        {
            ViewMgr.PopLayer();     // pop main lobby view
            ViewMgr.PushLayer(new CreateGameView(this));
        }

        private void Logout(EventArgs args)
        {
            Client.Network.BeginDisconnect(OnLogout, null);
        }

        private void JoinGame(EventArgs args)
        {
            //TODO: Implement join game
        }

        private void BeginGame(EventArgs args)
        {
            Game.ChangeState(new PlayState(Game));
        }

        private void RefreshGameList(EventArgs args)
        {
            var messageBox = new MessageBox(MessageBoxButtons.None)
            {
                Title = "Loading Main Lobby",
                Message = "Downloading game list..."
            };            

            ViewMgr.PushLayer(messageBox);

            var mainLobbyView = (args as SenderEventArgs).Sender;
            Client.Network.BeginGetGameList(OnGetGameList, mainLobbyView);
        }

        #endregion

        #region Network async callbacks

        private void OnLogout(IAsyncResult result)
        {
            Client.Network.EndDisconnect(result);
            Client.ChangeState(new MenuState(Game));
        }
                
        private void OnGetGameList(IAsyncResult result)
        {            
            var gameNames = Game.Network.EndGetGameList(result);
            var lobbyWindow = result.AsyncState as MainLobbyView;
            lobbyWindow.Invoke(lobbyWindow.RefreshGameList, gameNames);

            ViewMgr.PopLayer(); // MessageBox            
        }

        #endregion
    }
}
