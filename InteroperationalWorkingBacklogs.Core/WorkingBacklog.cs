using System;

namespace InteroperationalWorkingBacklogs.Core
{
	public class WorkingBacklog
	{
		public Operation Operation1
		{ get; private set; }

		public Operation Operation2
		{ get; private set; }

		public decimal StartTime
		{ get; private set; }

		public decimal Longevity
		{ get; private set; }

		public decimal EndTime
		{ get { return StartTime + Longevity; } }

		public decimal Value
		{ get; private set; }

		public WorkingBacklog(Operation operation1, Operation operation2, decimal startTime, decimal longevity, decimal value)
		{
			Operation1 = operation1;
			Operation2 = operation2;
			StartTime = startTime;
			Longevity = longevity;
			Value = value;
		}

		public bool TryMerge(WorkingBacklog other, out WorkingBacklog result)
		{
			if (Operation1 != other.Operation1 ||
			    Operation2 != other.Operation2 ||
			    EndTime != other.StartTime)
			{
				throw new InvalidOperationException("Only neighboor backlogs within single operation can be merged.");
			}

			if (Value == other.Value)
			{
				result = new WorkingBacklog(
					Operation1,
					Operation2,
					StartTime,
					Longevity + other.Longevity,
					Value);
				return true;
			}
			else
			{
				result = null;
				return false;
			}
		}
	}
}
