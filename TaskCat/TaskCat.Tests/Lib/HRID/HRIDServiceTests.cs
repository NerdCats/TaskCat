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
            HRIDServiceMock.Setup(x => x.GetExistingIdCount(It.IsAny<string>())).Returns((long)0);
            HRIDServiceMock.Setup(x => x.InsertNewHRID(It.IsAny<string>())).Verifiable();

            HRIDService service = HRIDServiceMock.Object;
            var id = service.NextId("JOB");
            Assert.IsNotNull(id);
            Assert.That(id.Length == 12);
        }

        [Test()]
        public void Test_NextId_With_ExistingCount()
        {
            Assert.Throws<ServerErrorException>(()=>{
                Mock<HRIDService> HRIDServiceMock = new Mock<HRIDService>(IDbContextMock.Object);
                HRIDServiceMock.Setup(x => x.GetExistingIdCount(It.IsAny<string>())).Returns((long)1);
                HRIDServiceMock.Setup(x => x.InsertNewHRID(It.IsAny<string>())).Verifiable();

                HRIDService service = HRIDServiceMock.Object;
                var id = service.NextId("JOB");
                Assert.IsNotNull(id);
                Assert.That(id.Length == 12);
            });      
        }

        [Test()]
        public void Test_NextId_Multiple_Request()
        {
            Mock<HRIDService> HRIDServiceMock = new Mock<HRIDService>(IDbContextMock.Object);
            HRIDServiceMock.Setup(x => x.GetExistingIdCount(It.IsAny<string>())).Returns((long)0);
            HRIDServiceMock.Setup(x => x.InsertNewHRID(It.IsAny<string>())).Verifiable();

            HRIDService service = HRIDServiceMock.Object;

            var id1Task = Task.Run(() => { return service.NextId("JOB"); });
            var id2Task = Task.Run(() => { return service.NextId("JOB"); });
            var id3Task = Task.Run(() => { return service.NextId("JOB"); });
            var id4Task = Task.Run(() =>
            {
                return service.NextId("JOB");
            });

            Task.WaitAll(id1Task, id2Task, id3Task, id4Task);

            Assert.IsNotNull(id1Task.Result);
            Assert.IsNotNull(id2Task.Result);
            Assert.IsNotNull(id3Task.Result);
            Assert.IsNotNull(id4Task.Result);
            Assert.That(id1Task.Result.Length == 12);
            Assert.That(id2Task.Result.Length == 12);
            Assert.That(id3Task.Result.Length == 12);
            Assert.That(id4Task.Result.Length == 12);

        }
    }
}