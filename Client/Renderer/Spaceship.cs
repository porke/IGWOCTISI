﻿namespace Client.Renderer
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Client.Common.AnimationSystem;
    using Client.Model;
    using Common;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    
    public class Spaceship : IMovable
    {
        #region Pooling

        /// <summary>
        /// Dictionary key is the color value.
        /// </summary>
        private static Dictionary<int, ObjectPool<Spaceship>> pools = new Dictionary<int,ObjectPool<Spaceship>>();
        
        public static void SetupColorPools(ICollection<PlayerColor> colors, ContentManager Content, AnimationManager AnimationManager)
        {
            foreach (var color in colors)
            {
                var factory = new SpaceshipFactory(color);
                var pool = new ObjectPool<Spaceship>(100, factory);

                factory.Content = Content;
                factory.AnimationManager = AnimationManager;

                pools.Add(color.Value, pool);
            }
        }
        
        public static Spaceship Acquire(PlayerColor playerColor)
        {
            return pools[playerColor.Value].Get(spaceship => { spaceship.Visible = true; });
        }

        public static void Recycle(Spaceship obj)
        {
            obj.Visible = false;
            pools[obj.PlayerColor.Value].Put(obj);
        }

        #endregion

        #region Object creation

        private class SpaceshipFactory : ObjectPool<Spaceship>.IObjectFactory
        {
            public ContentManager Content
            {
                get { return _contentManager; }
                set { _contentManager = value; OnInstallContentManager(); }
            }

            public AnimationManager AnimationManager { get; set; }

            private ContentManager _contentManager;
            private static Model _model;
            private Texture2D _texture;
            private PlayerColor _color;
                       
            public SpaceshipFactory(PlayerColor color)
            {
                _color = color;
            }

            public Spaceship Fetch()
            {
                Debug.Assert(_contentManager != null, "ContentManager can't be null!", "SpaceshipFactory should have ContentManager already installed on Fetching new Spaceship.");
                return new Spaceship(_color, _texture, _model, AnimationManager);
            }

            private void OnInstallContentManager()
            {
                _model = Content.Load<Model>(@"Models\LittleSpaceship");
                //Content.BeginLoad<Texture2D>(@"Textures\Spaceships\" + _color.ToString(), OnTextureLoad, null);
            }

            public void OnTextureLoad(IAsyncResult ar)
            {
                _texture = _contentManager.EndLoad<Texture2D>(ar);
            }
        }

        #endregion
        
        #region Public Fields

        public Vector3 Position
        {
            get { return WorldTransform.Translation; }
            set
            {
                Matrix tmpTransform = this.WorldTransform;
                tmpTransform.Translation = value;
                this.WorldTransform = tmpTransform;
            }
        }
        public Matrix WorldTransform { get; set; }
        public bool Visible { get; set; }
        public PlayerColor PlayerColor { get; private set; }

        #endregion

        #region Private Fields

        private Texture2D Texture { get; set; }
        private Model Model { get; set; }
        private AnimationManager _animationManager;

        #endregion

        private Spaceship(PlayerColor playerColor, Texture2D texture, Model model, AnimationManager animationManager)
        {
            PlayerColor = playerColor;
            Texture = texture;
            Model = model;
        }

        public void Draw(SimpleCamera camera, double delta, double time)
        {
            foreach (ModelMesh mesh in Model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
				{
					effect.EnableDefaultLighting();
					camera.ApplyToEffect(effect, WorldTransform);

                    //effect.Texture = this.Texture;

				}
				mesh.Draw();
            }


        }
    }
}
