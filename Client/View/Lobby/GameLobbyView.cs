﻿namespace Client.View.Lobby
{
    using System;
    using System.Collections.Generic;
    using Client.Input.Controls;
    using Client.Model;
    using Common;
    using Input;
    using Nuclex.UserInterface;
    using Nuclex.UserInterface.Controls;
    using Nuclex.UserInterface.Controls.Desktop;
    using State;

    class GameLobbyView : BaseView
    {
        #region Protected members

        private ListControl _messageList;
        private ListControl _playerList;
        private CommandInputControl _currentMessage;

        protected void CreateChildControls()
        {
            var btnBeginGame = new ButtonControl()
            {
                Text = "Begin Game",
                Bounds = new UniRectangle(new UniScalar(0.7f, 0), new UniScalar(0.05f, 0), new UniScalar(0.25f, 0), new UniScalar(0.1f, 0))
            };
            btnBeginGame.Pressed += BeginGame_Pressed;

            var btnLeaveGame = new ButtonControl()
            {
                Text = "Leave Game",
                Bounds = new UniRectangle(new UniScalar(0.7f, 0), new UniScalar(0.2f, 0), new UniScalar(0.25f, 0), new UniScalar(0.1f, 0))
            };
            btnLeaveGame.Pressed += LeaveGame_Pressed;

            var btnKickPlayer = new ButtonControl()
            {
                Text = "Kick player",
                Bounds = new UniRectangle(new UniScalar(0.7f, 0), new UniScalar(0.35f, 0), new UniScalar(0.25f, 0), new UniScalar(0.1f, 0))
            };
            btnKickPlayer.Pressed += KickPlayer_Pressed;

            var btnSendChatMessage = new ButtonControl()
            {
                Text = "Send",
                Bounds = new UniRectangle(new UniScalar(0.85f, 0), new UniScalar(0.925f, 0), new UniScalar(0.1f, 0), new UniScalar(0.1f, 0))
            };
            btnSendChatMessage.Pressed += SendChatMessage_Pressed;

            _messageList = new ListControl()
            {
                Bounds = new UniRectangle(new UniScalar(0.05f, 0), new UniScalar(0.5f, 0), new UniScalar(0.9f, 0), new UniScalar(0.4f, 0))
            };

            _currentMessage = new CommandInputControl()
            {
                Text = "",
                Bounds = new UniRectangle(new UniScalar(0.05f, 0), new UniScalar(0.925f, 0), new UniScalar(0.8f, 0), new UniScalar(0.1f, 0))
            };
            _currentMessage.OnCommandHandler += SendChatMessage_Pressed;

            _playerList = new ListControl()
            {
                SelectionMode = ListSelectionMode.Single,
                Bounds = new UniRectangle(new UniScalar(0.05f, 0), new UniScalar(0.05f, 0), new UniScalar(0.6f, 0), new UniScalar(0.4f, 0))
            };

            screen.Desktop.Children.AddRange(new Control[] {
                btnBeginGame, btnLeaveGame, btnKickPlayer, btnSendChatMessage, _messageList, _currentMessage, _playerList
            });
        }

        #endregion        

        #region Event handlers

        private void BeginGame_Pressed(object sender, EventArgs e)
        {
            state.HandleViewEvent("BeginGame", e);
        }

        private void LeaveGame_Pressed(object sender, EventArgs e)
        {
            state.HandleViewEvent("LeaveGameLobby", e);
        }

        private void KickPlayer_Pressed(object sender, EventArgs e)
        {
            // TODO: Remove player - available only for the host
        }

        private void SendChatMessage_Pressed(object sender, EventArgs e)
        {
            var msgArgs = new ChatMessageArgs(_currentMessage.Text);
            _currentMessage.Text = "";
            state.HandleViewEvent("SendChatMessage", msgArgs);
        }

        #endregion

        #region UpdateRequests

        public void RefreshPlayerList(List<string> newPlayerList)
        {
            _playerList.Items.Clear();            
            _playerList.Items.AddRange(newPlayerList);
        }

        public void ChatMessageReceived(ChatMessage message)
        {
            _messageList.Items.Add(string.Format("<{0}/{1}>: {2}", message.Username, message.Time, message.Message));
        }

        #endregion
        
        public GameLobbyView(GameState state)
            : base(state)
        {            
            IsLoaded = true;
            IsTransparent = true;
            screen.Desktop.Bounds = new UniRectangle(new UniScalar(0.2f, 0), new UniScalar(0.25f, 0), new UniScalar(0.6f, 0), new UniScalar(0.5f, 0));
            InputReceiver = new NuclexScreenInputReceiver(screen, false);

            CreateChildControls();
        }
    }
}
