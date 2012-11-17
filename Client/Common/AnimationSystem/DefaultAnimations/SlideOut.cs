﻿namespace Client.Common.AnimationSystem.DefaultAnimations
{
	using System;
	using Nuclex.UserInterface;
	using Nuclex.UserInterface.Controls;

	public class SlideOut : Animation<Control>
	{
		#region Protected members

		protected UniScalar _begin;
		protected UniScalar _end;

		#endregion

		#region Animation members

		public override void Begin()
		{
			base.Begin();

			_begin = Context.Bounds.Left;
			_end = Context.Bounds.Left;
			_end.Offset -= Context.GetAbsoluteBounds().Right;

			Context.Bounds.Left = _begin;
		}
		public override void Update(double delta)
		{
			base.Update(delta);

			UniScalar current = new UniScalar();
			NuclexExtensions.Lerp(ref _begin, ref _end, (float) Progress, out current);
			Context.Bounds.Left = current;
		}
		public override void End()
		{
			Context.Bounds.Left = _end;

			base.End();
		}

		#endregion

		public SlideOut(Control context, AnimationManager animationMgr, double duration = 0.5, Func<double, double> interpolator = null)
			: base(context, animationMgr, duration, interpolator ?? Interpolators.AccelerateDecelerate())
		{
		}
	}

	public static class SlideOutExtensions
	{
		public static SlideOut SlideOut<T>(this Animation<T> animation, double duration = 0.5, Func<double, double> interpolator = null) where T : Control
		{
			var after = new SlideOut(animation.Context, animation.AnimationMgr, duration, interpolator);
			animation.AddAfter(after);
			return after;
		}
	}
}
