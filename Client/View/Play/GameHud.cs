﻿namespace Client.View.Play
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Client.Input.Controls;
	using Client.Model;
	using Common;
	using Input;
	using Nuclex.UserInterface;
	using Nuclex.UserInterface.Controls;
	using Nuclex.UserInterface.Controls.Desktop;
	using State;

    class GameHud : BaseView
	{
		#region Protected members

		private WrappableListControl _commandList;
        private LabelControl _playerNameValue;
        private LabelControl _fleetCountValue;
        private LabelControl _fleetIncomeValue;
		private LabelControl _techPointsValue;
        private ListControl _playerList;
        private LabelControl _timer;
        private WrappableListControl _messageList;
        private CommandInputControl _chatMessage;

        protected void CreateChildControls()
        {
            #region Player list section

            var playerListHeader = new LabelControl("Players")
            {
                Bounds = new UniRectangle(new UniScalar(0.875f, 0), new UniScalar(0.05f, 0), new UniScalar(0.1f, 0), new UniScalar(0.05f, 0))
            };

            _playerList = new ListControl()
            {
                SelectionMode = ListSelectionMode.None,
                Bounds = new UniRectangle(new UniScalar(0.85f, 0), new UniScalar(0.1f, 0), new UniScalar(0.125f, 0), new UniScalar(0.2f, 0))
            };

            #endregion

            #region Orders section

            var ordersHeader = new LabelControl("Orders")
            {                
                Bounds = new UniRectangle(new UniScalar(0.075f, 0), new UniScalar(0.2f, 0), new UniScalar(0.1f, 0), new UniScalar(0.1f, 0))
            };

            _commandList = new WrappableListControl()
            {
                Bounds = new UniRectangle(new UniScalar(0.01f, 0), new UniScalar(0.28f, 0), new UniScalar(0.21f, 0), new UniScalar(0.3f, 0))
            };            

            var btnDeleteOrder = new ButtonControl()
            {
                Text = "Delete",
				Bounds = new UniRectangle(new UniScalar(0.12f, 0), new UniScalar(0.59f, 0), new UniScalar(0.1f, 0), new UniScalar(0.05f, 0))
            };
            btnDeleteOrder.Pressed += DeleteCommand_Pressed;

            #endregion

            #region Resource values

            var playerNameDesc = new LabelControl("Player:")
            {
                Bounds = new UniRectangle(new UniScalar(0.01f, 0), new UniScalar(0.0f, 0), new UniScalar(0.1f, 0), new UniScalar(0.1f, 0))
            };

            _playerNameValue = new LabelControl()
            {
                Bounds = new UniRectangle(new UniScalar(0.175f, 0), new UniScalar(0.0f, 0), new UniScalar(0.1f, 0), new UniScalar(0.1f, 0))
            };

            var fleetCountDesc = new LabelControl("Deployable fleets:")
            {
                Bounds = new UniRectangle(new UniScalar(0.01f, 0), new UniScalar(0.05f, 0), new UniScalar(0.1f, 0), new UniScalar(0.1f, 0))
            };

            _fleetCountValue = new LabelControl("0")
            {
                Bounds = new UniRectangle(new UniScalar(0.175f, 0), new UniScalar(0.05f, 0), new UniScalar(0.1f, 0), new UniScalar(0.1f, 0))
            };

            var fleetIncomeDesc = new LabelControl("Fleets per turn:")
            {
                Bounds = new UniRectangle(new UniScalar(0.01f, 0), new UniScalar(0.1f, 0), new UniScalar(0.1f, 0), new UniScalar(0.1f, 0))
            };

            _fleetIncomeValue = new LabelControl("0")
            {                
                Bounds = new UniRectangle(new UniScalar(0.175f, 0), new UniScalar(0.1f, 0), new UniScalar(0.1f, 0), new UniScalar(0.1f, 0))
            };

			var techPointsDesc = new LabelControl("Tech points:")
			{
				Bounds = new UniRectangle(new UniScalar(0.01f, 0), new UniScalar(0.15f, 0), new UniScalar(0.1f, 0), new UniScalar(0.1f, 0))
			};

			_techPointsValue = new LabelControl("0")
			{
				Bounds = new UniRectangle(new UniScalar(0.175f, 0), new UniScalar(0.15f, 0), new UniScalar(0.1f, 0), new UniScalar(0.1f, 0))
			};

            #endregion

            #region Game buttons

            var btnLeaveGame = new ButtonControl()
            {
                Text = "Leave",
                Bounds = new UniRectangle(new UniScalar(0.01f, 0), new UniScalar(0.93f, 0), new UniScalar(0.1f, 0), new UniScalar(0.05f, 0))
            };
            btnLeaveGame.Pressed += LeaveGame_Pressed;

            var btnSendOrders = new ButtonControl()
            {
                Text = "Send",
				Bounds = new UniRectangle(new UniScalar(0.01f, 0), new UniScalar(0.59f, 0), new UniScalar(0.1f, 0), new UniScalar(0.05f, 0))                
            };
            btnSendOrders.Pressed += SendCommands_Pressed;

            #endregion

            #region Timer

            _timer = new LabelControl("0:00")
            {
                Bounds = new UniRectangle(new UniScalar(0.89f, 0), new UniScalar(0.0f, 0), new UniScalar(0.1f, 0), new UniScalar(0.07f, 0))               
            };
            
            #endregion

            #region Message box & chat

            _chatMessage = new CommandInputControl
            {
                Bounds = new UniRectangle(new UniScalar(0.3f, 0), new UniScalar(0.93f, 0), new UniScalar(0.6f, 0), new UniScalar(0.05f, 0))
            };
            _chatMessage.OnCommandHandler += new EventHandler(ChatMessage_Execute);

            _messageList = new WrappableListControl
            {
                SelectionMode = ListSelectionMode.None,
                Bounds = new UniRectangle(new UniScalar(0.3f, 0), new UniScalar(0.75f, 0), new UniScalar(0.675f, 0), new UniScalar(0.16f, 0))
            };

            var btnClearMessage = new ButtonControl
            {
                Text = "C",
                Bounds = new UniRectangle(new UniScalar(0.925f, 0), new UniScalar(0.93f, 0), new UniScalar(0.05f, 0), new UniScalar(0.05f, 0))
            };
            btnClearMessage.Pressed += ClearMessageList;

            #endregion

            screen.Desktop.Children.AddRange(
                new Control[] 
                {
                    ordersHeader, 
                    _commandList, 
                    
                    playerListHeader,
                    _playerList,

                    _playerNameValue,
                    playerNameDesc,
                    _fleetIncomeValue, 
                    _fleetCountValue, 
                    fleetCountDesc,
                    fleetIncomeDesc,
					techPointsDesc,
					_techPointsValue,
                    
                    btnDeleteOrder, 
                    btnLeaveGame, 
                    btnSendOrders,
                    
                    _messageList,
                    _chatMessage,
                    btnClearMessage,

                    _timer
                });
        }        

        #endregion

        #region Event handlers

        private void LeaveGame_Pressed(object sender, EventArgs e)
        {
			PlayState.LeaveGame();
        }

        private void SendCommands_Pressed(object sender, EventArgs e)
        {
			PlayState.SendCommands();
        }

        private void DeleteCommand_Pressed(object sender, EventArgs e)
        {
            if (_commandList.SelectedItems.Count > 0)
            {
                var selectedOrderIndex = _commandList.SelectedItems[0];
				PlayState.DeleteCommand(selectedOrderIndex);
            }
        }

        private void ChatMessage_Execute(object sender, EventArgs e)
        {
            if (_chatMessage.Text.Trim().Length > 0)
            {
                PlayState.SendChatMessage(_chatMessage.Text);
                _chatMessage.Text = string.Empty;
            }
        }

        #endregion

        #region Update requests

        public void UpdatePlayerList(List<Player> players)
        {
            _playerList.Items.Clear();
            _playerList.Items.AddRange(players.Select(player => player.Username));
        }

        public void UpdateClientPlayerResourceData(Player player)
        {
            _playerNameValue.Text = player.Username;
            _fleetCountValue.Text = Convert.ToString(player.DeployableFleets);
            _fleetIncomeValue.Text = Convert.ToString(player.FleetIncomePerTurn);
			_techPointsValue.Text = Convert.ToString(player.TechPoints);
        }

        public void UpdateTimer(int secondsLeft)
        {
            int mins = secondsLeft / 60;
            int secs = secondsLeft % 60;

            // Display Timer in format 0:00
            _timer.Text = mins.ToString() + (secs < 10 ? ":0" : ":") + secs.ToString();
        }

        public void UpdateCommandList(List<UserCommand> commands, int selectedCommand = -1)
        {
            _commandList.Clear();

			// Not using sort, because it's unstable
			var orderedCmds = commands.OrderByDescending(cmd => (int)cmd.Type);
			commands = new List<UserCommand>(orderedCmds);
			foreach (var cmd in commands)
            {
                if (cmd.Type == UserCommand.CommandType.Deploy)
                {
                    _commandList.AddItem(string.Format("D: {0} to {1}", cmd.FleetCount, cmd.TargetPlanet.Name));
                }
                else if (cmd.Type == UserCommand.CommandType.Move)
                {
                    _commandList.AddItem(string.Format("M: {0} from {1} to {2}", cmd.FleetCount, cmd.SourcePlanet.Name, cmd.TargetPlanet.Name));
                }
                else if (cmd.Type == UserCommand.CommandType.Attack)
                {
                    _commandList.AddItem(string.Format("A: {0} from {1} to {2}", cmd.FleetCount, cmd.SourcePlanet.Name, cmd.TargetPlanet.Name));
                }
            }

            _commandList.SelectItem(selectedCommand);
        }

        public void ClearMessageList(object sender, EventArgs args)
        {
            _messageList.Clear();
        }

        public void AddMessage(string message)
        {
            _messageList.AddItem(message);
        }

        #endregion

		public PlayState PlayState { get; protected set; }

        public GameHud(PlayState state) : base(state)
        {
			PlayState = state;
            IsTransparent = true;            
            InputReceiver = new NuclexScreenInputReceiver(screen, false);            

            CreateChildControls();
			State = ViewState.Loaded;
        }
    }
}
