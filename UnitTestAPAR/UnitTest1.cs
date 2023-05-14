using APARControllerMaster;
using System.Text;

namespace UnitTestAPAR
{
    [TestFixture]
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void GenerateFrameTest()
        {
            string content = "HelloWorld";
            byte[] frame = APARProtocol.GenerateFrame(Encoding.ASCII.GetBytes(content),1);
            Assert.That(frame[15],Is.EqualTo(0xCB));
        }
    }
}