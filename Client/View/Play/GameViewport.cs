﻿namespace Client.View.Play
{
    using Client.State;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;
    using Model;
    using Nuclex.Input;
    using Nuclex.UserInterface.Input;

    class GameViewport : BaseView
    {
        private Scene _scene;

        #region IView members

        public override void Draw(double delta, double time)
        {
            var renderer = state.Client.Renderer;

            renderer.Draw(State.Scene, delta, time);
        }

        #endregion

        #region Nested class: GameInputReceiver

        private class GameInputReceiver : Client.Input.IInputReceiver
        {
            private Vector2 _currentMousePosition = Vector2.Zero;
            private GameViewport _receiverView;

            protected internal GameInputReceiver(GameViewport receiverView)
            {
                _receiverView = receiverView;
            }

            #region IInputReceiver members

            public bool OnCommand(Command command)
            {
                // Not supported            
                return true;
            }

            public bool OnKeyPressed(Keys key)
            {
                // TODO: implement OnKeyPressed
                return true;
            }

            public bool OnKeyReleased(Keys key)
            {
                // TODO: implement OnKeyReleased
                return true;
            }

            public bool OnMouseMoved(Vector2 position)
            {
                _currentMousePosition = position;
                return true;
            }

            public bool OnMousePressed(MouseButtons button)
            {
                // Handled in OnMouseReleased
                return true;
            }

            public bool OnMouseReleased(MouseButtons button)
            {
                if (button.HasFlag(MouseButtons.Left) || button.HasFlag(MouseButtons.Right))
                {
                    var planet = _receiverView._scene.PickPlanet(_currentMousePosition);
                    _receiverView.PlanetSelected(planet);
                }

                return true;
            }

            public bool OnMouseWheel(float ticks)
            {
                // Not supported
                return true;
            }

            public bool OnButtonPressed(Buttons button)
            {
                // Not supported
                return true;
            }

            public bool OnButtonReleased(Buttons button)
            {
                // Not supported
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

        public GameViewport(GameState state, Scene scene) : base(state)
        {
            IsLoaded = true;
            IsTransparent = false;
            InputReceiver = new GameInputReceiver(this);
            _scene = scene;
            State = state;
        }
    }
}
