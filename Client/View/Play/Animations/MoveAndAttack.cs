﻿namespace Client.View.Play.Animations
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Threading;
	using Client.Common.AnimationSystem;
	using Client.Model;
	using Client.Renderer;
	using Microsoft.Xna.Framework;
	using System.ComponentModel;

	public static class MoveAndAttack
	{
		public static void AnimateMovesAndAttacks(this SceneVisual scene,
			IList<Tuple<Planet, Planet, SimulationResult, Action<SimulationResult>>> movesAndAttacks,
			AnimationManager animationManager, SimpleCamera camera)
		{
			var bw = new BackgroundWorker();
			bw.DoWork += new DoWorkEventHandler((sender, workArgs) =>
			{
				var waiter = new ManualResetEvent(true);
				foreach (var tpl in movesAndAttacks)
				{
					var sourcePlanet = tpl.Item1;
					var targetPlanet = tpl.Item2;
					var simResult = tpl.Item3;
					var callback = tpl.Item4;

					waiter.Reset();
					if (simResult.Type == SimulationResult.MoveType.Move)
					{
						AnimateMove(sourcePlanet, targetPlanet, simResult, scene, animationManager, camera, waiter);
					}
					else
					{
						Debug.Assert(simResult.Type == SimulationResult.MoveType.Attack);
						AnimateAttack(sourcePlanet, targetPlanet, simResult, scene, animationManager, camera, waiter);
					}
					waiter.WaitOne();
					callback.Invoke(simResult);
				}
			});
			bw.RunWorkerAsync();
		}

		private static void AnimateMove(Planet sourcePlanet, Planet targetPlanet, SimulationResult simResult,
			SceneVisual scene, AnimationManager animationManager, SimpleCamera camera, ManualResetEvent waiter)
		{
			var player = sourcePlanet.Owner;
			var sourcePosition = sourcePlanet.Visual.GetPosition();
			var targetPosition = targetPlanet.Visual.GetPosition();
			var direction = Vector3.Normalize(targetPosition - sourcePosition);
			sourcePosition += direction * sourcePlanet.Radius;
			targetPosition -= direction * targetPlanet.Radius;

			var ship = Spaceship.Acquire(SpaceshipModelType.LittleSpaceship, player.Color);

			const float shipSpeedFactor = 0.015f;
			float moveDuration = (targetPosition - sourcePosition).Length() * shipSpeedFactor;
			float fadeDuration = ship.Length * shipSpeedFactor;

			scene.AddSpaceship(ship);
			ship.SetPosition(sourcePosition);
			ship.LookAt(targetPosition, Vector3.Forward);
			ship.Animate(animationManager)
				.Compound(moveDuration, c =>
				{
					// Move
					c.MoveTo(targetPlanet.Visual.GetPosition(), moveDuration, Interpolators.AccelerateDecelerate());

					// Fade in and fade out
					c.InterpolateTo(1, fadeDuration, Interpolators.Accelerate(),
						(s) => 0,
						(s, o) => { s.Opacity = (float)o; }
					)
					.Wait(moveDuration - 2 * fadeDuration)
					.InterpolateTo(0, fadeDuration, Interpolators.Decelerate(1.4),
						(s) => 1,
						(s, o) => { s.Opacity = (float)o; }
					);
				})
				.AddCallback(s =>
				{
					Spaceship.Recycle(s);
					waiter.Set();
				});
		}

		private static void AnimateAttack(Planet sourcePlanet, Planet targetPlanet, SimulationResult simResult,
			SceneVisual scene, AnimationManager animationManager, SimpleCamera camera, ManualResetEvent waiter)
		{
			var sourcePosition = sourcePlanet.Visual.GetPosition();
			var targetPosition = targetPlanet.Visual.GetPosition();
			var ship = Spaceship.Acquire(SpaceshipModelType.LittleSpaceship, sourcePlanet.Owner.Color);
			
			scene.AddSpaceship(ship);
			ship.SetPosition(sourcePosition);
			ship.LookAt(targetPosition, Vector3.Forward);
			ship.Animate(animationManager)
				.MoveTo(targetPosition, 2, Interpolators.AccelerateDecelerate())
				.AddCallback(s =>
				{
					Spaceship.Recycle(s);
					waiter.Set();
				});
		}
	}
}
