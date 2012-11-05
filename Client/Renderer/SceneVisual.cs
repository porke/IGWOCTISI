﻿namespace Client.Renderer
{
    using System;
    using System.Collections.Generic;
    using Client.Common.AnimationSystem;
    using Client.Model;
    using Microsoft.Xna.Framework.Content;
    using Client.View.Play.Animations;

    public sealed class SceneVisual
    {
        public List<Spaceship> Spaceships { get; private set; }
        public SimpleCamera Camera { get; set; }

        private Scene Scene { get; set; }
        private AnimationManager AnimationManager;
        private List<Spaceship> _spaceshipsToAdd = new List<Spaceship>();

        public SceneVisual(Scene scene, ContentManager Content, AnimationManager AnimationManager)
        {
            Scene = scene;
            Spaceships = new List<Spaceship>();
            Spaceship.InstallManagers(Content, AnimationManager);

            // Install handlers
            scene.AnimDeploy += new Action<Planet, int, Action>(Animation_Deploy);
        }

        void Animation_Deploy(Planet targetPlanet, int newFleetsCount, Action onEndCallback)
        {
            for (int i = 0; i < newFleetsCount; ++i)
            {
                var ship = Spaceship.Acquire(targetPlanet.Owner.Color);
                ship.Position = Camera.Position;
                ship.AnimateDeploy(AnimationManager, targetPlanet, newFleetsCount, onEndCallback);

                break;
            }
        }

        public void AddSpaceship(Spaceship ship)
        {
            lock (_spaceshipsToAdd)
            {
                _spaceshipsToAdd.Add(ship);
            }
        }

        internal void Draw(double delta, double time)
        {
            // Update spaceship list
            lock (_spaceshipsToAdd)
            {
                // It is more efficient to use that type of loop than foreach/ForEach.
                for (int i = 0, n = _spaceshipsToAdd.Count; i < n; ++i)
                    Spaceships.Add(_spaceshipsToAdd[i]);

                _spaceshipsToAdd.Clear();
            }

            // TODO Draw planets and links (and particles?)

            // Draw spaceships
            foreach (var ship in Spaceships)
            {
                ship.Draw(Camera, delta, time);
            }
        }
    }
}
