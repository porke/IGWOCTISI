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

        private static Dictionary<PlayerColor, ObjectPool<Spaceship>> pools;

        static Spaceship()
        {
            pools = new Dictionary<PlayerColor, ObjectPool<Spaceship>>();
            foreach (var color in Enum.GetValues(typeof(PlayerColor)).Cast<PlayerColor>())
            {
                var colorPool = new ObjectPool<Spaceship>(100, new SpaceshipFactory(color));
                pools.Add(color, colorPool);
            }
        }

        public static void InstallManagers(ContentManager Content, AnimationManager AnimationManager)
        {
            foreach (var factory in pools.Values.Select(pool => pool.Factory))
            {
                (factory as SpaceshipFactory).Content = Content;
                (factory as SpaceshipFactory).AnimationManager = AnimationManager;
            }
        }
        
        public static Spaceship Acquire(PlayerColor playerColor)
        {
            return pools[playerColor].Get(spaceship => { spaceship.Visible = true; });
        }

        public static void Put(Spaceship obj)
        {
            pools[obj.PlayerColor].Put(obj);
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
                    effect.World = WorldTransform;
                    //effect.Texture = this.Texture;
                }
                mesh.Draw();
            }
        }
    }
}
