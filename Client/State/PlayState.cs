﻿namespace Client.State
{
    using System;
    using System.Collections.Generic;
    using Model;
    using View.Play;
    using Client.View;

    class PlayState : GameState
    {
        public Scene Scene { get; protected set; }

        private GameViewport _gameViewport;
        private GameHud _gameHud;

        private List<UserCommand> _commands = new List<UserCommand>();
        private Player _clientPlayer;

        public PlayState(IGWOCTISI game, Map loadedMap, Player clientPlayer)
            : base(game)
        {
            Scene = new Scene(loadedMap, new List<Player> { _clientPlayer});
            _clientPlayer = clientPlayer;

            _gameViewport = new GameViewport(this);
            _gameHud = new GameHud(this);

            ViewMgr.PushLayer(_gameViewport);
            ViewMgr.PushLayer(_gameHud);

            eventHandlers.Add("LeaveGame", LeaveGame);
            eventHandlers.Add("SendOrders", SendOrders);
            eventHandlers.Add("SelectPlanet", SelectPlanet);

            Client.Network.OnRoundStarted += new Action(Network_OnRoundStarted);
            Client.Network.OnRoundEnded += new Action(Network_OnRoundEnded);
            Client.Network.OnGameEnded += new Action(Network_OnGameEnded);
            Client.Network.OnOtherPlayerLeft += new Action<string, DateTime>(Network_OnOtherPlayerLeft);
            Client.Network.OnDisconnected += new Action<string>(Network_OnDisconnected);
        }

        #region View event handlers

        private void LeaveGame(EventArgs args)
        {
            Client.ChangeState(new LobbyState(Game, _clientPlayer));
        }

        private void SendOrders(EventArgs args)
        {
            // TODO: Send orders
            _commands.Clear();
        }

        private void SelectPlanet(EventArgs args)
        {
            var selectedPlanetArg = args as SelectPlanetArgs;
            _gameHud.UpdateSelectedPlanet(selectedPlanetArg.Planet);
        }

        #endregion

        #region Async network callbacks
        
        void Network_OnRoundStarted()
        {
            throw new NotImplementedException();
        }

        void Network_OnRoundEnded()
        {
            throw new NotImplementedException();
        }

        void Network_OnGameEnded()
        {
            throw new NotImplementedException();
        }

        void Network_OnOtherPlayerLeft(string username, DateTime time)
        {
            throw new NotImplementedException();
        }

        void Network_OnDisconnected(string reason)
        {
            InvokeOnMainThread(obj =>
            {
                var menuState = new MenuState(Game);
                Client.ChangeState(menuState);
                menuState.HandleViewEvent("OnDisconnected", new MessageBoxArgs("Disconnection", "You were disconnected from the server."));
            });
        }

        #endregion
    }
}
