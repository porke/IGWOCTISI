﻿namespace Client.Renderer
{
	using System.Linq;
	using Microsoft.Xna.Framework.Graphics;
	using Client.Model;
	using Microsoft.Xna.Framework.Content;
	using Microsoft.Xna.Framework;
	using System;
	
	public class MapVisual
	{
		#region Protected members

		protected SpriteBatch _spriteBatch;
		protected Effect _fxLinks;
		protected Tuple<BackgroundLayer, Texture2D>[] _layers;
		protected VertexBuffer LinksVB;

		#endregion

		public Map Map { get; protected set; }

		public MapVisual(GameClient client, Map map)
		{
			Map = map;
			var contentMgr = client.Content;

			var vertices = new VertexPositionColor[map.Links.Count * 2];
			var color = Color.LightGreen;

			for (var i = 0; i < map.Links.Count; ++i)
			{
				var link = Map.Links[i];
				var sourcePlanet = Map.Planets.First(x => x.Id == link.SourcePlanet);
				var targetPlanet = Map.Planets.First(x => x.Id == link.TargetPlanet);

				vertices[2 * i + 0] = new VertexPositionColor(new Vector3(sourcePlanet.X, sourcePlanet.Y, sourcePlanet.Z), color);
				vertices[2 * i + 1] = new VertexPositionColor(new Vector3(targetPlanet.X, targetPlanet.Y, targetPlanet.Z), color);
			}

			LinksVB = new VertexBuffer(client.GraphicsDevice, VertexPositionColor.VertexDeclaration, vertices.Length, BufferUsage.WriteOnly);
			LinksVB.SetData(vertices);

			_spriteBatch = new SpriteBatch(client.GraphicsDevice);
			_layers = Map.Background.Select(x => Tuple.Create(x, client.Content.Load<Texture2D>(x.Texture))).ToArray();

			_fxLinks = contentMgr.Load<Effect>("Effects\\Links");

			foreach (var planet in Map.Planets)
			{
				planet.Visual = new PlanetVisual(client, planet);
			}

			foreach (var planetarySystem in map.PlanetarySystems)
			{
				planetarySystem.Visual = new PlanetarySystemVisual(client, planetarySystem);
				planetarySystem.Visual.Color = Color.LightGray;
				client.Components.Add(planetarySystem.Visual.ParticleSystem);
			}
		}
		internal void Initialize()
		{
			foreach (var planet in Map.Planets)
			{
				planet.Visual.Initialize();
			}
		}
		public void Update(double delta, double time)
		{
			foreach (var planetarySystem in Map.PlanetarySystems)
			{
				planetarySystem.Visual.Update(Map, delta, time);
			}
		}
		public void DrawBackground(GraphicsDevice device, ICamera camera, double delta, double time)
		{
			var viewport = device.Viewport;
			_spriteBatch.Begin();

			foreach (var pair in _layers)
			{
				var layer = pair.Item1;
				var texture = pair.Item2;

				var worldCamera = new Vector2(Map.Camera.X, -Map.Camera.Y);
				var halfView = new Vector2(viewport.Width, viewport.Height) / 2.0f;
				var worldOrigin = layer.Origin + worldCamera*(layer.Speed - 1.0f);
				var screenOrigin = (worldOrigin + worldCamera) * new Vector2(1, -1) + halfView;
				var scale = layer.Size / new Vector2(texture.Width, texture.Height);
				_spriteBatch.Draw(texture, screenOrigin, null, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 1);
			}

			_spriteBatch.End();
		}
		public void DrawIndicators(GraphicsDevice device, ICamera camera, double delta, double time)
		{
			_fxLinks.Parameters["World"].SetValue(Matrix.Identity);
			_fxLinks.Parameters["View"].SetValue(camera.GetView());
			_fxLinks.Parameters["Projection"].SetValue(camera.Projection);

			// links
			_fxLinks.Parameters["Ambient"].SetValue(0.0f);
			foreach (var pass in _fxLinks.CurrentTechnique.Passes)
			{
				pass.Apply();
				device.SetVertexBuffer(LinksVB);
				device.DrawPrimitives(PrimitiveType.LineList, 0, LinksVB.VertexCount / 2);
			}
		}
	}
}
