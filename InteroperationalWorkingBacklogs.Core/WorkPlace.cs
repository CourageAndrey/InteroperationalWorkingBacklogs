using System;

namespace InteroperationalWorkingBacklogs.Core
{
	public class WorkPlace
	{
		public int Number
		{ get; private set; }

		public Operation Operation
		{ get; private set; }

		public decimal LoadPercent
		{ get; private set; }

		public decimal StartTime
		{ get; set; }

		public decimal EndTime
		{ get { return StartTime + LoadPercent; } }

		public WorkPlace(int number, Operation operation, decimal loadPercent, decimal startTime = 0)
		{
			Number = number;
			Operation = operation;
			LoadPercent = loadPercent;
			StartTime = startTime;
			if (LoadPercent == 1 && startTime > 0)
			{
				throw new ArgumentException("When Load Percent = 100%, Start Time has to be 0!", "startTime");
			}
		}
	}
}
