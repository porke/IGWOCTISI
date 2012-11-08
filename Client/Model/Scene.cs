﻿namespace Client.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Client.Renderer;
    using Microsoft.Xna.Framework;
    using System.Threading;

	public class Scene
	{		
		public int HoveredPlanet { get; set; }
		public PlanetLink HoveredLink { get; set; }
		public SceneVisual Visual { get; set; }

		public int SelectedPlanet { get; private set; }
		public Map Map { get; private set; }		

		private List<Player> _players;

		#region Events for SceneVisual

        /// <summary>
        /// Arguments: targetPlanet, newFleetsCount, onEndCallback.
        /// </summary>
		internal event Action<Planet, int, Action> AnimDeploy;

		#endregion

		public Scene(Map map)
		{
            Map = map;
			HoveredPlanet = SelectedPlanet = 0;
			HoveredLink = null;
		}
		public Planet PickPlanet(Vector2 clickPosition, IRenderer renderer)
		{
			foreach (var item in Map.Planets)
			{
				if (renderer.RaySphereIntersection(clickPosition, new Vector3(item.X, item.Y, item.Z), item.Radius))
				{
					return item;
				}
			}

			return null;
		}
		public PlanetLink PickLink(Vector2 clickPosition, IRenderer renderer)
		{
			var selectedLinks = Map.Links.Where(x => SelectedPlanet == x.SourcePlanet || SelectedPlanet == x.TargetPlanet);
			foreach (var link in selectedLinks)
			{
				var sourcePlanet = Map.GetPlanetById(link.SourcePlanet);
				var targetPlanet = Map.GetPlanetById(link.TargetPlanet);
				var sourcePos = new Vector3(sourcePlanet.X, sourcePlanet.Y, sourcePlanet.Z);
				var targetPos = new Vector3(targetPlanet.X, targetPlanet.Y, targetPlanet.Z);

                if (renderer.RayLinkIntersection(clickPosition, sourcePos, targetPos))
                {
                    return link;
                }
			}
			return null;
		}
		internal void AnimateChanges(IList<SimulationResult> simResults, Action endCallback)
		{
		    var deploys = simResults.Where(sr => sr.Type == SimulationResult.MoveType.Deploy).ToList();
            var moves = simResults.Where(sr => sr.Type == SimulationResult.MoveType.Move).ToList();
		    var attacks = simResults.Where(sr => sr.Type == SimulationResult.MoveType.Attack).ToList();

		    var deployAnimCounter = new CountdownEvent(deploys.Count);
		    var moveAnimCounter = new CountdownEvent(moves.Count);
		    var attackAnimCounter = new CountdownEvent(attacks.Count);

            foreach (var deployResult in deploys)
            {
                var targetPlanet = Map.GetPlanetById(deployResult.TargetId);
                int newFleetsCount = deployResult.FleetCount;

                AnimDeploy(targetPlanet, newFleetsCount, () =>
                {
                    //targetPlanet.NumFleetsPresent += newFleetsCount;
                    deployAnimCounter.Signal();
                });
            }

            foreach (var moveResult in moves)
            {
                moveAnimCounter.Signal();
            }

            foreach (var attackResult in attacks)
            {
                attackAnimCounter.Signal();
            }

		    ThreadPool.QueueUserWorkItem(new WaitCallback((obj) =>
		    {
		        deployAnimCounter.Wait();
		        moveAnimCounter.Wait();
		        attackAnimCounter.Wait();
		        endCallback.Invoke();
		    }));


		    // TODO: The animation actually makes debugging the move and deploy mechanics a little more difficult
		    // so until it is done, this function will not function properly

		    //var sourcePlanet = Map.GetPlanetById(simResult.SourceId);
		    //var targetPlanet = Map.GetPlanetById(simResult.TargetId);

		    //if (simResult.Type == SimulationResult.MoveType.Attack)
		    //{
		    //    sourcePlanet.NumFleetsPresent = simResult.SourceLeft;
		    //    targetPlanet.NumFleetsPresent = simResult.TargetLeft;

		    //    if (simResult.TargetOwnerChanged)
		    //    {
		    //        targetPlanet.Owner = _players.Find(player => player.Username.Equals(simResult.TargetOwner));
		    //    }
		    //}
		    //else if (simResult.Type == SimulationResult.MoveType.Move)
		    //{
		    //    sourcePlanet.NumFleetsPresent = simResult.SourceLeft;
		    //    targetPlanet.NumFleetsPresent = simResult.TargetLeft;
		    //}
		    //else if (simResult.Type == SimulationResult.MoveType.Deploy)
		    //{
		    //    int newFleetsCount = simResult.FleetCount;
		    //    AnimDeploy(targetPlanet, newFleetsCount, () =>
		    //    {
		    //        targetPlanet.NumFleetsPresent += newFleetsCount;
		    //    });
		    //}
		}

	    public void Initialize(NewRoundInfo roundInfo, List<Player> players)
	    {
            _players = players;

            // Assign planets to the players and vice versa.
            foreach (var data in Map.PlayerStartingData)
            {
                var planet = Map.GetPlanetById(data.PlanetId);
                var newPlanetState = roundInfo.FindPlanetState(planet.Id);
                string ownerName = null;

                // Planet may be not assigned to anyone.
                if (roundInfo.TryFindPlanetOwner(planet.Id, ref ownerName))
                {
                    var player = _players.Find(p => p.Username.Equals(ownerName));
                    player.Color = Map.GetColorById(data.ColorId);
                    player.TryAssignPlanet(planet);
                    planet.NumFleetsPresent = newPlanetState.Fleets;

                    // Translate the color from hex to enum
                    player.Color = Map.GetColorById(data.ColorId);
                }
            }
	    }

		public bool CanSelectPlanet(Planet planet, Player clientPlayer)
		{
			if (planet.Owner == null) return false;
			if (!planet.Owner.Username.Equals(clientPlayer.Username)) return false;

			return true;
		}

		public void SelectPlanet(Planet planet)
		{
			SelectedPlanet = planet.Id;
		}
	}
}
