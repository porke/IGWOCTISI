﻿namespace Client.View.Play
{
    using Client.State;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;
    using Model;
    using Nuclex.Input;

    class GameViewport : BaseView
    {
        #region IView members

        public override void Draw(double delta, double time)
        {
            var renderer = state.Client.Renderer;

            renderer.Draw(PlayState.Scene, delta, time);
        }

        #endregion

        #region Nested class: GameInputReceiver

        private class GameInputReceiver : Client.Input.NullInputReceiver
        {
            private Vector2 _currentMousePosition = Vector2.Zero;
            private GameViewport _receiverView;

            protected internal GameInputReceiver(GameViewport receiverView) : base(false)
            {
                _receiverView = receiverView;
            }

            #region IInputReceiver members

            public override bool OnMouseMoved(Vector2 position)
            {
                _currentMousePosition = position;
                return true;
            }

            public override bool OnMouseReleased(MouseButtons button)
            {
                if (button.HasFlag(MouseButtons.Left) || button.HasFlag(MouseButtons.Right))
                {
                    var planet = _receiverView.PlayState.Scene.PickPlanet(_currentMousePosition, _receiverView.PlayState.Client.Renderer);

                    if (planet != null)
                    {
                        _receiverView.PlanetSelected(planet);
                    }
                }

                return true;
            }

            public override bool OnKeyPressed(Keys key)
            {
                var camera = _receiverView.PlayState.Client.Renderer.GetCamera();

                switch (key)
                {
                    case Keys.Down:
                        camera.TranslationDirection = -Vector3.UnitY;
                        break;
                    case Keys.Up:
                        camera.TranslationDirection = Vector3.UnitY;
                        break;
                    case Keys.Left:
                        camera.TranslationDirection = -Vector3.UnitX;
                        break;
                    case Keys.Right:
                        camera.TranslationDirection = Vector3.UnitX;
                        break;
                }

                return true;
            }

            public override bool OnKeyReleased(Keys key)
            {
                switch (key)
                {
                    case Keys.Down:
                    case Keys.Up:
                    case Keys.Left:
                    case Keys.Right:
                        _receiverView.PlayState.Client.Renderer.GetCamera().TranslationDirection = Vector3.Zero;
                        break;
                }

                return true;
            }

            #endregion
        }        

        #endregion

        #region Event handlers

        private void PlanetSelected(Planet planetSelected)
        {
            state.HandleViewEvent("SelectPlanet", new SelectPlanetArgs(planetSelected));
        }

        #endregion

        public PlayState PlayState { get; protected set; }

        public GameViewport(GameState state) : base(state)
        {
            IsLoaded = true;
            IsTransparent = false;
            InputReceiver = new GameInputReceiver(this);
            PlayState = (PlayState)state;
        }
    }
}
