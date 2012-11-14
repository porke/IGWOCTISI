﻿namespace Client.View.Play
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Client.View.Controls;
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
        private ListControl _playerList;
        private WrappableListControl _messageList;
        private CommandInputControl _chatMessage;
		private ButtonControl btnSendCommands;
		private ButtonControl btnDeleteCommand;

		private TopPanel _topPanel;

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
                Bounds = new UniRectangle(new UniScalar(0.075f, 0), new UniScalar(0.37f, 0), new UniScalar(0.1f, 0), new UniScalar(0.1f, 0))
            };

            _commandList = new WrappableListControl()
            {
                Bounds = new UniRectangle(new UniScalar(0.01f, 0), new UniScalar(0.45f, 0), new UniScalar(0.21f, 0), new UniScalar(0.3f, 0))
            };            

            btnDeleteCommand = new ButtonControl()
            {
                Text = "Delete",
				Bounds = new UniRectangle(new UniScalar(0.12f, 0), new UniScalar(0.76f, 0), new UniScalar(0.1f, 0), new UniScalar(0.05f, 0))
            };
            btnDeleteCommand.Pressed += DeleteCommand_Pressed;

            #endregion

			_topPanel = new TopPanel();

			#region Technology section

			var techHeader = new LabelControl("Technology")
			{
				Bounds = new UniRectangle(new UniScalar(0.06f, 0), new UniScalar(0.21f, 0), new UniScalar(0.1f, 0), new UniScalar(0.1f, 0))
			};

			var raiseAttackTech = new ButtonControl()
			{
				Text = "+",
				Name = TechnologyType.Offensive.ToString(),
				Bounds = new UniRectangle(new UniScalar(0.02f, 0), new UniScalar(0.32f, 0), new UniScalar(0.05f, 0), new UniScalar(0.05f, 0))
			};
			raiseAttackTech.Pressed += RaiseTech_Pressed;

			var raiseDefenseTech = new ButtonControl()
			{
				Text = "+",
				Name = TechnologyType.Defensive.ToString(),
				Bounds = new UniRectangle(new UniScalar(0.08f, 0), new UniScalar(0.32f, 0), new UniScalar(0.05f, 0), new UniScalar(0.05f, 0))
			};
			raiseDefenseTech.Pressed += RaiseTech_Pressed;

			var raiseEconomyTech = new ButtonControl()
			{
				Text = "+",
				Name = TechnologyType.Economic.ToString(),
				Bounds = new UniRectangle(new UniScalar(0.14f, 0), new UniScalar(0.32f, 0), new UniScalar(0.05f, 0), new UniScalar(0.05f, 0))
			};
			raiseEconomyTech.Pressed += RaiseTech_Pressed;

			#endregion

			#region Game buttons

			var btnLeaveGame = new ButtonControl()
            {
                Text = "Leave",
                Bounds = new UniRectangle(new UniScalar(0.01f, 0), new UniScalar(0.93f, 0), new UniScalar(0.1f, 0), new UniScalar(0.05f, 0))
            };
            btnLeaveGame.Pressed += LeaveGame_Pressed;

            btnSendCommands = new ButtonControl()
            {
                Text = "Send",
				Bounds = new UniRectangle(new UniScalar(0.01f, 0), new UniScalar(0.76f, 0), new UniScalar(0.1f, 0), new UniScalar(0.05f, 0))                
            };
            btnSendCommands.Pressed += SendCommands_Pressed;

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
					_topPanel,

                    ordersHeader, 
                    _commandList, 
                    
                    playerListHeader,
                    _playerList,

					techHeader,
					raiseAttackTech,
					raiseDefenseTech,
					raiseEconomyTech,

                    btnDeleteCommand, 
                    btnSendCommands,
                    btnLeaveGame, 
                    
                    _messageList,
                    _chatMessage,
                    btnClearMessage,
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
			btnSendCommands.Enabled = false;
			btnDeleteCommand.Enabled = false;

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

		private void RaiseTech_Pressed(object sender, EventArgs e)
		{
			var senderName = (sender as Control).Name;			
			var techType = (TechnologyType) Enum.Parse(typeof(TechnologyType), senderName);
			PlayState.RaiseTechnology(techType);
		}

        #endregion

        #region Update requests

        public void UpdatePlayerList(List<Player> players)
        {
            _playerList.Items.Clear();
            _playerList.Items.AddRange(players.Select(player => player.Username));
        }

        public void UpdateResourceData(Player player)
        {
			_topPanel.UpdateResources(player);
        }

        public void UpdateTimer(int secondsLeft)
        {
			_topPanel.UpdateTimer(secondsLeft);
        }

        public void UpdateCommandList(List<UserCommand> commands, int selectedCommand = -1)
        {
            _commandList.Clear();

			// Not using sort, because it'stat unstable
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
				else if (cmd.Type == UserCommand.CommandType.Tech)
				{
					_commandList.AddItem(string.Format("T: Research {0} tech", cmd.TechType));
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

		public void EnableOrderButtons()
		{
			btnDeleteCommand.Enabled = true;
			btnSendCommands.Enabled = true;
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
