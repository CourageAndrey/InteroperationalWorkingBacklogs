using System;
using System.Collections.Generic;

namespace InteroperationalWorkingBacklogs.Core
{
	public class Operation
	{
		public string Name
		{ get; private set; }

		public decimal TimeNorm
		{ get; private set; }

		public decimal CalculatedWorkerCount
		{ get; private set; }

		public int ActualWorkerCount
		{ get { return (int) Math.Ceiling(CalculatedWorkerCount); } }

		public List<WorkPlace> WorkPlaces
		{ get; private set; }

		public Operation(string name, decimal timeNorm, decimal calculatedWorkerCount)
		{
			Name = name;
			TimeNorm = timeNorm;
			CalculatedWorkerCount = calculatedWorkerCount;
			WorkPlaces = new List<WorkPlace>();
		}
	}
}
