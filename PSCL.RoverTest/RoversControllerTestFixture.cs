using System.Reflection;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PSCL.RoverTest
{
    [TestClass]
    public class RoversControllerTestFixture
    {
        [TestMethod]
        public void TestOutput ()
        {
            // Arrange.
            var instructionStream = Assembly.GetExecutingAssembly ().GetManifestResourceStream ("PSCL.RoverTest.RoverInstructions.txt");
            var roversController = new PSCL.Rover.RoversController (instructionStream);

            // Act.
            var result = roversController.Execute ();

            // Assert.
            Assert.AreEqual ("1 3 N\r\n5 1 E\r\n", result);
        }
    }
}
