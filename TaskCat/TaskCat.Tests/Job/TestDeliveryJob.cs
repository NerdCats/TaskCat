namespace TaskCat.Tests.Job
{
    using Moq;
    using NUnit.Framework;
    using TaskCat.Lib.Job;
    using Data.Entity;
    using MongoDB.Driver;
    using Microsoft.AspNet.Identity;
    using Data.Entity.Identity;
    using TaskCat.Lib.Auth;
    using System.Threading.Tasks;
    using Data.Model.Order;
    using Data.Model.Geocoding;
    using Data.Model.GeoJson;
    using System.Linq;
    using Data.Model.Identity.Response;
    using Data.Model.Identity.Profile;
    using TaskCat.Lib.Job.Builders;
    using TaskCat.Lib.HRID;
    using Data.Lib.Payment;
    using Data.Model;
    using TaskCat.Model.Job;
    [TestFixture]
    public class TestDeliveryJob
    {
        IHRIDService hridService;
        string MockedHrid = "Job#123456";
        Mock<IPaymentMethod> paymentMethodMock;

        [SetUp]
        public void Setup()
        {
            Mock<IHRIDService> hridServiceMock = new Mock<IHRIDService>();
            hridServiceMock.Setup<string>(x => x.NextId(It.IsAny<string>())).Returns(MockedHrid);
            hridService = hridServiceMock.Object;
            paymentMethodMock = new Mock<IPaymentMethod>();
        }

        [Test]
        public async Task Test_Cancel_Delivery_Job_With_No_Task_In_Progress()
        {
            var searchJobId = "i1i2i3i4";
            string cancellationReason = "test cancellation reason";

            var replaceOneResult = new ReplaceOneResult.Acknowledged(1, 1, null);

            var createdJob = GetDummyJob();

            var jobManagerMock = new Mock<IJobManager>();
            jobManagerMock.Setup(x => x.UpdateJob(It.IsAny<Job>()))
                .ReturnsAsync(replaceOneResult);

            jobManagerMock.Setup(x => x.GetJob(searchJobId)).ReturnsAsync(createdJob);

            var userStoreMock = new Mock<IUserStore<User>>();

            var jobRepository = new JobRepository(jobManagerMock.Object,
                new AccountManager(userStoreMock.Object));

            var result = await jobRepository.CancelJob(new JobCancellationRequest()
            {
                JobId = searchJobId,
                Reason = cancellationReason
            });

            Assert.IsNotNull(result);
            Assert.AreEqual(JobState.CANCELLED, result.UpdatedValue.State);
            Assert.AreEqual(cancellationReason, result.UpdatedValue.CancellationReason);
            Assert.AreEqual(JobTaskState.CANCELLED, result.UpdatedValue.Tasks.First().State);
        }

        [Test]
        public async Task Test_Restore_Delivery_Job_With_No_Task_In_Progress()
        {
            var searchJobId = "i1i2i3i4";
            string cancellationReason = "test cancellation reason";
            var replaceOneResult = new ReplaceOneResult.Acknowledged(1, 1, null);

            var createdJob = GetDummyJob();
            createdJob.State = JobState.CANCELLED;
            createdJob.Tasks.Last().State = JobTaskState.CANCELLED;

            var jobManagerMock = new Mock<IJobManager>();
            jobManagerMock.Setup(x => x.UpdateJob(It.IsAny<Job>()))
                .ReturnsAsync(replaceOneResult);

            jobManagerMock.Setup(x => x.GetJob(searchJobId)).ReturnsAsync(createdJob);

            var userStoreMock = new Mock<IUserStore<User>>();

            var jobRepository = new JobRepository(jobManagerMock.Object,
                new AccountManager(userStoreMock.Object));

            var result = await jobRepository.RestoreJob(searchJobId);

            Assert.IsNotNull(result);
            Assert.AreEqual(JobState.ENQUEUED, result.UpdatedValue.State);
            result.UpdatedValue.Tasks.ForEach(x => Assert.AreEqual(JobTaskState.PENDING, x.State));
            Assert.AreEqual(null, result.UpdatedValue.CancellationReason);
        }

        private Job GetDummyJob()
        {
            DeliveryOrder order = new DeliveryOrder();
            order.From = new DefaultAddress("Test From Address", new Point((new double[] { 1, 2 }).ToList()));
            order.To = new DefaultAddress("Test To Address", new Point((new double[] { 2, 1 }).ToList()));

            UserModel userModel = new UserModel()
            {
                Email = "someone@somewhere.com",
                EmailConfirmed = false,
                IsUserAuthenticated = false,
                PhoneNumber = "+8801684512833",
                PhoneNumberConfirmed = true,
                Profile = new UserProfile()
                {
                    Address = new DefaultAddress("Somewhere User belong", new Point(2, 1)),
                    Age = 26,
                    FirstName = "Gabul",
                    LastName = "Habul",
                    Gender = Gender.MALE,
                    PicUri = null
                },
                Type = Data.Model.Identity.IdentityTypes.USER,
                UserId = "123456789",
                UserName = "GabulTheAwesome"
            };

            UserModel backendAdminModel = new UserModel()
            {
                Email = "someone@somewhere.com",
                EmailConfirmed = false,
                IsUserAuthenticated = false,
                PhoneNumber = "+8801684512833",
                PhoneNumberConfirmed = true,
                Profile = new UserProfile()
                {
                    Address = new DefaultAddress("Somewhere User belong", new Point(2, 1)),
                    Age = 26,
                    FirstName = "Gabul",
                    LastName = "Habul",
                    Gender = Gender.MALE,
                    PicUri = null
                },
                Type = Data.Model.Identity.IdentityTypes.USER,
                UserId = "123456789",
                UserName = "GabulTheAwesome"
            };

            var builder = new DeliveryJobBuilder(order, userModel, backendAdminModel, hridService, paymentMethodMock.Object);
            builder.BuildJob();

            return builder.Job;
        }
    }
}
