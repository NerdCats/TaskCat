namespace TaskCat.Lib.HRID.Tests.Lib.HRID
{
    using NUnit.Framework;
    using Db;
    using Moq;
    using System.Threading.Tasks;
    using Exceptions;
    using System;

    [TestFixture()]
    public class HRIDServiceTests
    {
        Mock<IDbContext> IDbContextMock;


        [SetUp]
        public void Setup()
        {          
            IDbContextMock = new Mock<IDbContext>();
        }

        [Test()]
        public void Test_NextId_Without_ExistingCount()
        {
            Mock<HRIDService> HRIDServiceMock = new Mock<HRIDService>(IDbContextMock.Object);
            HRIDServiceMock.Setup(x => x.GetExistingIdCount(It.IsAny<string>())).Returns(Task.FromResult((long)0));
            HRIDServiceMock.Setup(x => x.InsertNewHRID(It.IsAny<string>())).Returns(Task.FromResult(false));

            HRIDService service = HRIDServiceMock.Object;
            var id = service.NextId("JOB");
            Assert.IsNotNull(id);
            Assert.That(id.Length == 12);
        }

        [Test()]
        public void Test_NextId_With_ExistingCount()
        {
            Assert.Throws<AggregateException>(()=>{
                Mock<HRIDService> HRIDServiceMock = new Mock<HRIDService>(IDbContextMock.Object);
                HRIDServiceMock.Setup(x => x.GetExistingIdCount(It.IsAny<string>())).Returns(Task.FromResult((long)1));
                HRIDServiceMock.Setup(x => x.InsertNewHRID(It.IsAny<string>())).Returns(Task.FromResult(false));

                HRIDService service = HRIDServiceMock.Object;
                var id = service.NextId("JOB");
                Assert.IsNotNull(id);
                Assert.That(id.Length == 12);
            });      
        }
    }
}