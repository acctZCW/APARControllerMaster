using APARControllerMaster;
using System.Data;
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
            byte[] frame = APARProtocol.GenerateFrame(Encoding.ASCII.GetBytes(content).ToList(),1);
            Assert.That(frame[15],Is.EqualTo(0xCB));
        }

        [Test]
        public void ReadCSVDataTest()
        {
            string filepath = "C:\\Users\\zcw\\Desktop\\datatest.csv";
            DataTable dt = APARCommands.ReadDataFromCSV(filepath);
            var result = dt.Rows[1];
        }
    }
}