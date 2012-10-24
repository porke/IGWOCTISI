﻿namespace Client.State
{
	using System;
	using System.Collections.Generic;
	using Client.View;
	using Model;
	using View.Play;

	class PlayState : GameState
	{
		public Scene Scene { get; protected set; }

		private GameViewport _gameViewport;
		private GameHud _gameHud;

		private List<UserCommand> _commands = new List<UserCommand>();
		private Player _clientPlayer;
		private List<Player> _players;
		private double _secondsLeft = 0;
		private HudState _hudState = HudState.Initializing;
		private object _hudStateLocker = new object();

		private enum HudState
		{
			Initializing,
			WaitingForRoundStart,
			WaitingForRoundEnd,
			AnimatingSimulationResult
		}

		public PlayState(IGWOCTISI game, Map loadedMap, Player clientPlayer, List<Player> players)
			: base(game)
		{
			_clientPlayer = clientPlayer;
			_players = players;

			Scene = new Scene(loadedMap, _players);
			_gameViewport = new GameViewport(this);
			_gameHud = new GameHud(this);
            _gameHud.UpdateClientPlayerFleetData(_clientPlayer);
			_gameHud.UpdatePlayerList(_players);

			ViewMgr.PushLayer(_gameViewport);
			ViewMgr.PushLayer(_gameHud);

			Client.Network.OnRoundStarted += Network_OnRoundStarted;
			Client.Network.OnRoundEnded += Network_OnRoundEnded;
			Client.Network.OnGameEnded += Network_OnGameEnded;
			Client.Network.OnOtherPlayerLeft += Network_OnOtherPlayerLeft;
			Client.Network.OnDisconnected += Network_OnDisconnected;

			InvokeOnMainThread((obj) => { _hudState = HudState.WaitingForRoundStart; });
		}

		public override void OnUpdate(double delta, double time)
		{
			base.OnUpdate(delta, time);
			
			// Update timer
			if (_secondsLeft > 0)
			{
				if (_secondsLeft - delta <= 0)
				{
					_secondsLeft = 0;

					lock (_hudStateLocker)
					{
						if (_hudState == HudState.WaitingForRoundEnd)
						{
							// Create message box that will be shown until server's roundEnd or gameEnd message arrives.
							var messageBox = new MessageBox(MessageBoxButtons.OK)
							{
								Title = "Round simulating",
								Message = "Waiting for server to simulate the turn."
									+ Environment.NewLine + Environment.NewLine
									+ "(This OK button will disappear)"
							};
							messageBox.OkPressed += (sender, e) => { ViewMgr.PopLayer(); };//TODO to be removed (no OK button!!)
							ViewMgr.PushLayer(messageBox);
						}
					}
				}
				else
				{
					_secondsLeft -= delta;
				}
			}

			_gameHud.UpdateTimer((int)_secondsLeft);
		}

		#region View event handlers

		internal void DeleteCommand(int orderIndex)
		{
            var deletedCommand = _commands[orderIndex];

            // Remove dependant commands
            // ex. a move dependant on an earlier deploy
            if (!deletedCommand.CanRevert())
            {
                var dependantCommands = _commands.FindAll(cmd => cmd.Type == UserCommand.CommandType.Move && cmd.SourceId == deletedCommand.TargetId);

                foreach (var item in dependantCommands)
                {
                    item.Revert();
                    _commands.Remove(item);
                }
                deletedCommand.Revert();
            }

			_commands.RemoveAt(orderIndex);
			_gameHud.UpdateCommandList(_commands);
            _gameHud.UpdateClientPlayerFleetData(_clientPlayer);
		}
		internal void LeaveGame()
		{
			var messageBox = new MessageBox(MessageBoxButtons.None)
			{
				Message = "Leaving game..."
			};
			ViewMgr.PushLayer(messageBox);

			Client.Network.BeginLeaveGame(OnLeaveGame, messageBox);
		}
		internal void SendCommands()
		{
			Client.Network.BeginSendCommands(_commands, OnSendOrders, null);
			_commands.Clear();

			InvokeOnMainThread((obj) =>
			{
                if (_secondsLeft > 0)
                {
                    _secondsLeft = 0.001;
                }
			});
		}
		internal void SelectPlanet(Planet selectedPlanet)
		{
			Scene.SelectedPlanet = selectedPlanet.Id;
		}
		internal void OnHoverPlanet(Planet hoverPlanet)
		{
			Scene.HoveredPlanet = hoverPlanet.Id;
		}
		internal void UnhoverPlanets()
		{
			Scene.HoveredPlanet = 0;
		}
		internal void DeployFleet(Planet planet)
		{
			var gameHud = ViewMgr.PeekLayer() as GameHud;

			// Deploment is only possible on clients own planet
			if (planet.Owner == null) return;
            if (!planet.Owner.Username.Equals(_clientPlayer.Username)) return;
            if (_clientPlayer.DeployableFleets == 0) return;

			var command = _commands.Find(cmd => cmd.Type == UserCommand.CommandType.Deploy && cmd.TargetId == planet.Id);
			if (command == null)
			{
				command = new UserCommand(planet, 1);
				_commands.Add(command);
			}
			else
			{
				command.FleetCount++;
			}

			planet.NumFleetsPresent++;
			_clientPlayer.DeployableFleets--;
			_gameHud.UpdateCommandList(_commands);
			_gameHud.UpdateClientPlayerFleetData(_clientPlayer);
		}
		internal void UndeployFleet(Planet planet)
		{
			var gameHud = ViewMgr.PeekLayer() as GameHud;

			var command = _commands.Find(cmd => cmd.Type == UserCommand.CommandType.Deploy && cmd.TargetId == planet.Id);
			if (command != null)
			{
				command.FleetCount--;
				planet.NumFleetsPresent--;
				_clientPlayer.DeployableFleets++;

				if (command.FleetCount == 0)
				{
					_commands.Remove(command);
				}

				_gameHud.UpdateCommandList(_commands);
				_gameHud.UpdateClientPlayerFleetData(_clientPlayer);
			}
		}
		internal void OnHoverLink(PlanetLink hoverLink)
		{
			Scene.HoveredLink = hoverLink;
		}
		internal void UnhoverLinks()
		{
			Scene.HoveredLink = null;
		}
		internal void MoveFleet(PlanetLink link)
		{
            var source = Scene.Map.GetPlanetById(link.SourcePlanet);
            var target = Scene.Map.GetPlanetById(link.TargetPlanet);

            // Defensive coding
            if (source.Owner == null) return;
            if (source.NumFleetsPresent < 2) return;
            if (!_clientPlayer.Username.Equals(source.Owner.Username)) return;
            if (_clientPlayer.DeployableFleets == 0) return;

            var command = _commands.Find(cmd => cmd.SourceId == source.Id && cmd.TargetId == target.Id);
            if (command == null)
            {
                command = new UserCommand(source, target);
                command.FleetCount = 1;
                _commands.Add(command);
            }
            else
            {
                command.FleetCount++;
            }

            source.NumFleetsPresent--;
            target.NumFleetsPresent++;
            _gameHud.UpdateCommandList(_commands);
		}
        internal void RevertMoveFleet(PlanetLink link)
        {
            var source = Scene.Map.GetPlanetById(link.SourcePlanet);
            var target = Scene.Map.GetPlanetById(link.TargetPlanet);

            var targetCommand = _commands.Find(cmd => cmd.SourceId == source.Id && cmd.TargetId == target.Id);
            if (targetCommand != null)
            {
                targetCommand.FleetCount--;
                source.NumFleetsPresent++;
                target.NumFleetsPresent--;

                if (targetCommand.FleetCount == 0)
                {
                    _commands.Remove(targetCommand);
                }
                _gameHud.UpdateCommandList(_commands);
            }
        }

		#endregion

		#region Async network callbacks

		void OnLeaveGame(IAsyncResult result)
		{
			InvokeOnMainThread(obj =>
			{
				var messageBox = result.AsyncState as MessageBox;

				try
				{
					Client.Network.EndLeaveGame(result);
					ViewMgr.PopLayer(); // MessageBox
					Client.ChangeState(new LobbyState(Game, _clientPlayer));
				}
				catch (Exception exc)
				{
					messageBox.Buttons = MessageBoxButtons.OK;
					messageBox.Message = exc.Message;
					messageBox.OkPressed += (sender, e) =>
					{
						ViewMgr.PopLayer();
						Client.ChangeState(new LobbyState(Game, _clientPlayer));
					};
				}
			});
		}

		void OnSendOrders(IAsyncResult result)
		{
			InvokeOnMainThread(obj =>
			{
				Client.Network.EndSendCommands(result);
			});
		}

		bool Network_OnRoundStarted(NewRoundInfo roundInfo)
		{
			lock (_hudStateLocker)
			{
				if (_hudState == HudState.WaitingForRoundStart)
				{
					_secondsLeft = roundInfo.RoundTime;
					_gameHud.UpdateTimer((int)_secondsLeft);
					_hudState = HudState.WaitingForRoundEnd;

					// We have consumed that packet.
					return true;
				}

				return false;
			}
		}

		bool Network_OnRoundEnded(List<SimulationResult> simResults)
		{
			lock (_hudStateLocker)
			{
				if (_hudState == HudState.WaitingForRoundEnd)
				{
					if (ViewMgr.PeekLayer() is MessageBox)
					{
						// Pop MessageBox "Waiting for server to simulate the turn."
						ViewMgr.PopLayer();
					}

					_hudState = HudState.AnimatingSimulationResult;
					// TODO do some animations using simulation results and then set _hudState to WaitingForRoundStart.

					foreach (var simResult in simResults)
					{
						Scene.ImplementChange(simResult);
					}
					
					// TODO when animation is done that line should be moved to the end of animation.
					_hudState = HudState.WaitingForRoundStart;
					
					// We have consumed that packet.
					return true;
				}

				return false;
			}
		}

		void Network_OnGameEnded(/*game result here!*/)
		{
			// TODO show game result and statistics
			throw new NotImplementedException();
		}

		void Network_OnOtherPlayerLeft(string username, DateTime time)
		{
			_players.RemoveAll(player => player.Username.Equals(username));
			_gameHud.UpdatePlayerList(_players);

			// TODO print info (somewhere) about it!
		}

		void Network_OnDisconnected(string reason)
		{
			InvokeOnMainThread(obj =>
			{
				var menuState = new MenuState(Game);
				Client.ChangeState(menuState);
				menuState.OnDisconnected("Disconnection", "You were disconnected from the server.");
			});
		}

		#endregion
	}
}
