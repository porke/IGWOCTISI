﻿namespace Client.Common.AnimationSystem
{
	using System;
	using Client.Common;
	using Microsoft.Xna.Framework;


	/// <summary>
	/// If you inherit from this interface, then remember that Scale equals zero! You should minimally change it to 1!
	/// </summary>
	public interface ITransformable
	{
		float X { get; set; }
		float Y { get; set; }
		float Z { get; set; }

		Matrix Rotation { get; set; }

		float ScaleX { get; set; }
		float ScaleY { get; set; }
		float ScaleZ { get; set; }
	}

	public static class ITransformableExtensions
	{
		public static Matrix CalculateWorldTransform(this ITransformable t)
		{
			return t.Rotation
				* Matrix.CreateScale(t.ScaleX, t.ScaleY, t.ScaleZ)
				* Matrix.CreateTranslation(t.X, t.Y, t.Z);
		}

		public static void SetPosition(this ITransformable t, Vector3 position)
		{
			t.X = position.X;
			t.Y = position.Y;
			t.Z = position.Z;
		}

		public static Vector3 GetPosition(this ITransformable t)
		{
			return new Vector3(t.X, t.Y, t.Z);
		}

		public static Matrix GetTranslationMatrix(this ITransformable t)
		{
			return Matrix.CreateTranslation(t.X, t.Y, t.Z);
		}

		public static void SetScale(this ITransformable t, Vector3 scale)
		{
			t.ScaleX = scale.X;
			t.ScaleY = scale.Y;
			t.ScaleZ = scale.Z;
		}

		public static void SetScale(this ITransformable t, float scale)
		{
			t.ScaleX = t.ScaleY = t.ScaleZ = scale;
		}

		public static Vector3 GetScale(this ITransformable t)
		{
			return new Vector3(t.ScaleX, t.ScaleY, t.ScaleZ);
		}

		public static Matrix GetScaleMatrix(this ITransformable t)
		{
			return Matrix.CreateScale(t.ScaleX, t.ScaleY, t.ScaleZ);
		}

		public static Vector3 GetLook(this ITransformable t)
		{
			return t.Rotation.Forward;
		}

		/// <summary>
		/// Sets Rotation matrix based on position, lookAt point and up vector.
		/// </summary>
		/// <param name="t"></param>
		/// <param name="lookAt">point to look at</param>
		/// <param name="up">up vector</param>
		public static void LookAt(this ITransformable t, Vector3 lookAt, Vector3 up)
		{
			t.Rotation = MathUtils.LookAt(t.GetPosition(), lookAt, up);
		}

		public static void LookAt(this ITransformable t, Vector3 lookAt)
		{
			var look = lookAt - t.GetPosition();
			var up = look.GetUpVector();

			LookAt(t, lookAt, up);
		}

		public static Vector3 GetUpVector(this ITransformable t, float roll = 0)
		{
			return t.GetLook().GetUpVector(roll);
		}
	}
}
