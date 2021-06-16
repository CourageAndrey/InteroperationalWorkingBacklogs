using System;
using System.Collections.Generic;
using System.Linq;

namespace InteroperationalWorkingBacklogs.Core
{
	public static class Calculator
	{
		public static IList<WorkPlace> DefineWorkPlaces(this IEnumerable<Operation> operations)
		{
			var workPlaces = new List<WorkPlace>();

			int workplaceNumber = 1;
			foreach (var operation in operations)
			{
				operation.WorkPlaces.Clear();
				WorkPlace workPlace;

				int fullTimeWorkerCount = (int)Math.Floor(operation.CalculatedWorkerCount);
				for (int w = 0; w < fullTimeWorkerCount; w++)
				{
					workPlaces.Add(workPlace = new WorkPlace(workplaceNumber++, operation, 1));
					operation.WorkPlaces.Add(workPlace);
				}

				decimal delta = operation.CalculatedWorkerCount - fullTimeWorkerCount;
				if (delta > 0)
				{
					workPlaces.Add(workPlace = new WorkPlace(workplaceNumber++, operation, delta));
					operation.WorkPlaces.Add(workPlace);
				}
			}

			return workPlaces;
		}

		public static IList<WorkingBacklog> CalculateWorkingBacklogs(
			this IList<Operation> operations,
			bool redefineWorkplaces,
			Rounding rounding,
			decimal timePeriod = 1)
		{
			if (redefineWorkplaces)
			{
				operations.DefineWorkPlaces();
			}

			var workingBacklogs = new List<WorkingBacklog>();

			for (int i = 0; i < operations.Count - 1; i++)
			{
				workingBacklogs.AddRange(operations[i].CalculateWorkingBacklogs(operations[i + 1], rounding, timePeriod));
			}

			return workingBacklogs;
		}

		public static IEnumerable<WorkingBacklog> CalculateWorkingBacklogs(
			this Operation operation1,
			Operation operation2,
			Rounding rounding,
			decimal timePeriod = 1)
		{
			var intervals1 = OperationInterval.Split(operation1);
			var intervals2 = OperationInterval.Split(operation2);

			foreach (var interval in IntersectedInterval.Intersect(intervals1, intervals2))
			{
				decimal longevity = interval.Interval1.Longevity * timePeriod;
				decimal value = (longevity * interval.Interval1.WorkPlacesCount) / interval.Interval1.Operation.TimeNorm -
								(longevity * interval.Interval2.WorkPlacesCount) / interval.Interval2.Operation.TimeNorm;
				if (value != 0)
				{
					decimal startTime = interval.Interval1.Begin * timePeriod;
					yield return new WorkingBacklog(
						interval.Interval1.Operation,
						interval.Interval2.Operation,
						startTime,
						longevity,
						rounding.Round(value));
				}
			}
		}

		private class OperationInterval
		{
			public Operation Operation
			{ get; private set; }

			public decimal Begin
			{ get; private set; }

			public decimal Longevity
			{ get; private set; }

			public int WorkPlacesCount
			{ get; private set; }

			public decimal End
			{ get { return Begin + Longevity; } }

			private OperationInterval(Operation operation, decimal begin, decimal longevity, int workPlacesCount)
			{
				Operation = operation;
				Begin = begin;
				Longevity = longevity;
				WorkPlacesCount = workPlacesCount;
			}

			public OperationInterval Subtract(OperationInterval other, out IntersectedInterval intersection)
			{
				decimal restLongevity = Longevity - other.Longevity;
				intersection = new IntersectedInterval(
					new OperationInterval(
						Operation,
						Begin,
						other.Longevity,
						WorkPlacesCount), 
					other);
				return new OperationInterval(
					Operation,
					Begin + other.Longevity,
					restLongevity,
					WorkPlacesCount);
			}

			public static List<OperationInterval> Split(Operation operation)
			{
				var result = new List<OperationInterval>();

				var lastWP = operation.WorkPlaces.Last();
				if (lastWP.LoadPercent == 1)
				{
					result.Add(new OperationInterval(operation, 0, 1, operation.WorkPlaces.Count));
				}
				else if (lastWP.StartTime == 0)
				{
					result.Add(new OperationInterval(operation, 0, lastWP.LoadPercent, operation.WorkPlaces.Count));
					result.Add(new OperationInterval(operation, lastWP.LoadPercent, 1 - lastWP.LoadPercent, operation.WorkPlaces.Count - 1));
				}
				else
				{
					result.Add(new OperationInterval(operation, 0, lastWP.StartTime, operation.WorkPlaces.Count - 1));
					result.Add(new OperationInterval(operation, lastWP.StartTime, /*lastWP.StartTime + */lastWP.LoadPercent, operation.WorkPlaces.Count));
					if (lastWP.EndTime < 1)
					{
						result.Add(new OperationInterval(operation, lastWP.StartTime + lastWP.LoadPercent, 1 - (lastWP.StartTime + lastWP.LoadPercent), operation.WorkPlaces.Count - 1));
					}
				}

				return result;
			}
		}

		private class IntersectedInterval
		{
			public OperationInterval Interval1
			{ get; private set; }

			public OperationInterval Interval2
			{ get; private set; }

			internal IntersectedInterval(OperationInterval interval1, OperationInterval interval2)
			{
				Interval1 = interval1;
				Interval2 = interval2;
			}

			public static IEnumerable<IntersectedInterval> Intersect(List<OperationInterval> intervals1, List<OperationInterval> intervals2)
			{
				while (intervals1.Count > 0 && intervals2.Count > 0)
				{
					var interval1 = intervals1[0];
					var interval2 = intervals2[0];
					intervals1.RemoveAt(0);
					intervals2.RemoveAt(0);

					if (interval1.End > interval2.End)
					{
						IntersectedInterval intersection;
						intervals1.Insert(0, interval1.Subtract(interval2, out intersection));
						yield return intersection;
					}
					else if (interval1.End < interval2.End)
					{
						IntersectedInterval intersection;
						intervals2.Insert(0, interval2.Subtract(interval1, out intersection));
						intersection.SwapOperations();
						yield return intersection;
					}
					else // interval1.End == interval2.End
					{
						yield return new IntersectedInterval(interval1, interval2);
					}
				}

			}

			private void SwapOperations()
			{
				var temp = Interval1;
				Interval1 = Interval2;
				Interval2 = temp;
			}
		}
	}
}
