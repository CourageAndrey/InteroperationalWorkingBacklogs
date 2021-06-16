using System;
using System.Collections.Generic;

namespace InteroperationalWorkingBacklogs.Core
{
	public class Rounding
	{
		private readonly Func<decimal, decimal> _round;

		protected Rounding(Func<decimal, decimal> round)
		{
			_round = round;
		} 

		public virtual decimal Round(decimal value)
		{
			return _round(value);
		}

		public static readonly Rounding None = new Rounding(value => value);

		public static readonly Rounding MathCeilFloor = new Rounding(value => value > 0
			? Math.Ceiling(value)
			: Math.Floor(value));

		public static readonly Rounding MathRound = new Rounding(value => Math.Round(value));

		public static readonly ICollection<Rounding> All = new[]
		{
			None,
			MathCeilFloor,
			MathRound,
		};
	}
}
