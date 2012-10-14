﻿namespace Client.Input
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;
    using Nuclex.Input;
    using Nuclex.UserInterface.Input;

    public class NullInputReceiver : IInputReceiver
    {
        #region IInputReceiver members

        public virtual bool OnEnter()
        {
            return !InputPassThrough;
        }

        // commands
        public virtual bool OnCommand(Command command)
        {
            return !InputPassThrough;
        }

        // keyboard
        public virtual bool OnKeyPressed(Keys key)
        {
            return !InputPassThrough;
        }
        public virtual bool OnKeyReleased(Keys key)
        {
            return !InputPassThrough;
        }

        // mouse
        public virtual bool OnMouseMoved(Vector2 position)
        {
            return !InputPassThrough;
        }
        public virtual bool OnMousePressed(MouseButtons button)
        {
            return !InputPassThrough;
        }
        public virtual bool OnMouseReleased(MouseButtons button)
        {
            return !InputPassThrough;
        }
        public virtual bool OnMouseWheel(float ticks)
        {
            return !InputPassThrough;
        }

        // gamepad
        public virtual bool OnButtonPressed(Buttons button)
        {
            return !InputPassThrough;
        }
        public virtual bool OnButtonReleased(Buttons button)
        {
            return !InputPassThrough;
        }

        #endregion

        public bool InputPassThrough { get; protected set; }

        public NullInputReceiver(bool inputPassThrough)
        {
            InputPassThrough = inputPassThrough;
        }
    }
}
