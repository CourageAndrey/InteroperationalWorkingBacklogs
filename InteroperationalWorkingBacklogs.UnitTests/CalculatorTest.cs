using System.Linq;

using InteroperationalWorkingBacklogs.Core;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InteroperationalWorkingBacklogs.UnitTests
{
	[TestClass]
	public class CalculatorTest
	{
		[TestMethod]
		public void DefineWorkingPlaces()
		{
			// arrange
			Operation operationA, operationB, operationC;
			var operations = new[]
			{
				operationA = new Operation("A", 0, 3),
				operationB = new Operation("B", 0, 1.2m),
				operationC = new Operation("C", 0, 0.8m),
			};

			// act
			var workPlaces = operations.DefineWorkPlaces();

			// assert
			Assert.AreEqual(6, workPlaces.Count);

			Assert.AreEqual(1, workPlaces[0].Number);
			Assert.AreEqual(2, workPlaces[1].Number);
			Assert.AreEqual(3, workPlaces[2].Number);
			Assert.AreEqual(4, workPlaces[3].Number);
			Assert.AreEqual(5, workPlaces[4].Number);
			Assert.AreEqual(6, workPlaces[5].Number);

			Assert.AreSame(operationA, workPlaces[0].Operation);
			Assert.AreSame(operationA, workPlaces[1].Operation);
			Assert.AreSame(operationA, workPlaces[2].Operation);
			Assert.AreSame(operationB, workPlaces[3].Operation);
			Assert.AreSame(operationB, workPlaces[4].Operation);
			Assert.AreSame(operationC, workPlaces[5].Operation);

			Assert.IsTrue(workPlaces.Where(wp => wp.Operation == operationA).All(wp => wp.LoadPercent == 1));

			Assert.AreEqual(1m, workPlaces[3].LoadPercent);
			Assert.AreEqual(0.2m, workPlaces[4].LoadPercent);

			Assert.AreEqual(0.8m, workPlaces[5].LoadPercent);
		}

		[TestMethod]
		public void CalculateWorkingBacklogs()
		{
			// arange
			Operation operation1, operation2, operation3, operation4;
			var operations = new[]
			{
				operation1 = new Operation("Токарная", 1.9m, 1.19m),
				operation2 = new Operation("Сверлильная", 1.1m, 0.69m),
				operation3 = new Operation("Фрезерная", 2.1m, 1.31m),
				operation4 = new Operation("Шлифовальная", 1.3m, 0.81m),
			};

			var workPlaces = operations.DefineWorkPlaces();
			workPlaces[4].StartTime = 0.69m;
			workPlaces[5].StartTime = 0.19m;

			// act
			var workingBacklogs = operations.CalculateWorkingBacklogs(false, Rounding.MathCeilFloor, 240);

			// assert
			Assert.AreEqual(8, workingBacklogs.Count);

			Assert.IsTrue(workingBacklogs[0].Operation1 == operation1 && workingBacklogs[0].Operation2 == operation2);
			Assert.IsTrue(workingBacklogs[1].Operation1 == operation1 && workingBacklogs[1].Operation2 == operation2);
			Assert.IsTrue(workingBacklogs[2].Operation1 == operation1 && workingBacklogs[2].Operation2 == operation2);

			Assert.IsTrue(workingBacklogs[3].Operation1 == operation2 && workingBacklogs[3].Operation2 == operation3);
			Assert.IsTrue(workingBacklogs[4].Operation1 == operation2 && workingBacklogs[4].Operation2 == operation3);

			Assert.IsTrue(workingBacklogs[5].Operation1 == operation3 && workingBacklogs[5].Operation2 == operation4);
			Assert.IsTrue(workingBacklogs[6].Operation1 == operation3 && workingBacklogs[6].Operation2 == operation4);

			Assert.AreEqual(0m, workingBacklogs[0].StartTime);
			Assert.AreEqual(45.6m, workingBacklogs[0].Longevity);
			Assert.AreEqual(7m, workingBacklogs[0].Value);

			Assert.AreEqual(45.6m, workingBacklogs[1].StartTime);
			Assert.AreEqual(120m, workingBacklogs[1].Longevity);
			Assert.AreEqual(-46m, workingBacklogs[1].Value);

			Assert.AreEqual(165.6m, workingBacklogs[2].StartTime);
			Assert.AreEqual(74.4m, workingBacklogs[2].Longevity);
			Assert.AreEqual(40m, workingBacklogs[2].Value);

			Assert.AreEqual(0m, workingBacklogs[3].StartTime);
			Assert.AreEqual(165.6m, workingBacklogs[3].Longevity);
			Assert.AreEqual(72m, workingBacklogs[3].Value);

			Assert.AreEqual(165.6m, workingBacklogs[4].StartTime);
			Assert.AreEqual(74.4m, workingBacklogs[4].Longevity);
			Assert.AreEqual(-71m, workingBacklogs[4].Value);

			Assert.AreEqual(0m, workingBacklogs[5].StartTime);
			Assert.AreEqual(45.6m, workingBacklogs[5].Longevity);
			Assert.AreEqual(22m, workingBacklogs[5].Value);

			Assert.AreEqual(45.6m, workingBacklogs[6].StartTime);
			Assert.AreEqual(120m, workingBacklogs[6].Longevity);
			Assert.AreEqual(-36m, workingBacklogs[6].Value);

			Assert.AreEqual(165.6m, workingBacklogs[7].StartTime);
			Assert.AreEqual(74.4m, workingBacklogs[7].Longevity);
			Assert.AreEqual(14m, workingBacklogs[7].Value);
		}
	}
}
