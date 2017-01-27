namespace TaskCat.Tests.Job
{
    using Moq;
    using NUnit.Framework;
    using Data.Entity;
    using MongoDB.Driver;
    using Microsoft.AspNet.Identity;
    using Data.Entity.Identity;
    using System.Threading.Tasks;
    using Data.Model.Order;
    using Data.Model.Geocoding;
    using Data.Model.GeoJson;
    using System.Linq;
    using Data.Model.Identity.Response;
    using Data.Model.Identity.Profile;
    using Data.Lib.Payment;
    using Data.Model;
    using Data.Model.Inventory;
    using System.Collections.Generic;
    using System;
    using Data.Model.Order.Delivery;
    using Data.Model.Identity;
    using TaskCat.Account.Core;
    using System.Reactive.Subjects;
    using Common.HRID;
    using TaskCat.Job;
    using TaskCat.Job.Updaters;
    using TaskCat.Job.Builders;
    using TaskCat.Job.Extensions;
    using Data.Lib.Constants;

    [TestFixture]
    public class TestDeliveryJob
    {
        IHRIDService hridService;
        string MockedHrid = "Job#123456";
        Mock<IPaymentMethod> paymentMethodMock;
        private Subject<JobActivity> activitySubject;
        private Subject<Job> jobIndexingSubject;

        [SetUp]
        public void Setup()
        {
            Mock<IHRIDService> hridServiceMock = new Mock<IHRIDService>();
            hridServiceMock.Setup<string>(x => x.NextId(It.IsAny<string>())).Returns(MockedHrid);
            hridService = hridServiceMock.Object;
            paymentMethodMock = new Mock<IPaymentMethod>();

            this.activitySubject = new Subject<JobActivity>();
            this.jobIndexingSubject = new Subject<Job>();
        }

        private JobRepository SetupMockJobRepositoryForUpdate()
        {
            var jobManagerMock = new Mock<IJobManager>();
            var replaceOneResult = new ReplaceOneResult.Acknowledged(1, 1, null);

            jobManagerMock.Setup(x => x.UpdateJob(It.IsAny<Job>()))
                .ReturnsAsync(null);

            var userStoreMock = new Mock<IUserStore<User>>();
            var jobRepository = new JobRepository(jobManagerMock.Object,
                new AccountManager(userStoreMock.Object), activitySubject, jobIndexingSubject);
            return jobRepository;
        }

        [Test]
        public async Task Test_Update_Delivery_Order_In_Initial_State()
        {
            string orderName = "Updated Name";
            string noteToDeliveryMan = "Updated Note to Delivery Man";
            JobRepository jobRepository = SetupMockJobRepositoryForUpdate();

            var job = GetDummyJob(OrderTypes.Delivery);
            var updatedOrder = GetDummyOrder(orderType: OrderTypes.Delivery);
            updatedOrder.Name = orderName;
            updatedOrder.NoteToDeliveryMan = noteToDeliveryMan;
            updatedOrder.OrderCart = GetDummyCart();
            updatedOrder.RequiredChangeFor = 1000;

            var result = await jobRepository.UpdateOrder(job, updatedOrder, JobUpdateMode.smart);

            var newOrder = job.Order as DeliveryOrder;

            Assert.IsNotNull(result);
            Assert.AreEqual(orderName, job.Name);
            Assert.AreEqual(noteToDeliveryMan, newOrder.NoteToDeliveryMan);
            Assert.AreEqual(updatedOrder.OrderCart, newOrder.OrderCart);
            Assert.AreEqual(updatedOrder.RequiredChangeFor, newOrder.RequiredChangeFor);
        }

        [Test]
        public async Task Test_Update_Delivery_Order_In_In_Progress_State()
        {
            string orderName = "Updated Name";
            string noteToDeliveryMan = "Updated Note to Delivery Man";
            JobRepository jobRepository = SetupMockJobRepositoryForUpdate();

            var job = GetDummyJob(OrderTypes.Delivery);
            job.State = JobState.IN_PROGRESS;
            job.Tasks.First().State = JobTaskState.COMPLETED;
            job.Tasks.First().Asset = GetDummyAssetModel();
            job.Tasks.First().UpdateTask();
            job.Tasks[1].State = JobTaskState.IN_PROGRESS;
            job.Tasks[1].UpdateTask();

            var updatedOrder = GetDummyOrder(orderType: OrderTypes.Delivery);
            updatedOrder.Name = orderName;
            updatedOrder.NoteToDeliveryMan = noteToDeliveryMan;
            updatedOrder.OrderCart = GetDummyCart();
            updatedOrder.RequiredChangeFor = 1000;

            var result = await jobRepository.UpdateOrder(job, updatedOrder, JobUpdateMode.smart);

            var newOrder = job.Order as DeliveryOrder;

            Assert.IsNotNull(result);
            Assert.AreEqual(orderName, job.Name);
            Assert.AreEqual(noteToDeliveryMan, newOrder.NoteToDeliveryMan);
            Assert.IsNotNull(newOrder.OrderCart);
            Assert.AreEqual(updatedOrder.RequiredChangeFor, newOrder.RequiredChangeFor);
            Assert.IsNotNull(job.Tasks.First().InitiationTime);
            Assert.IsNotNull(job.Tasks[1].InitiationTime);
        }

        [Test]
        public void Test_Update_Delivery_Order_In_Completed_State()
        {
            string orderName = "Updated Name";
            string noteToDeliveryMan = "Updated Note to Delivery Man";
            JobRepository jobRepository = SetupMockJobRepositoryForUpdate();

            var job = GetDummyJob(OrderTypes.Delivery);
            job.Tasks.ForEach(x => { x.State = JobTaskState.COMPLETED; });
            job.State = JobState.COMPLETED;

            var updatedOrder = GetDummyOrder(orderType: OrderTypes.Delivery);
            updatedOrder.Name = orderName;
            updatedOrder.NoteToDeliveryMan = noteToDeliveryMan;
            updatedOrder.OrderCart = GetDummyCart();
            updatedOrder.RequiredChangeFor = 1000;

            Assert.ThrowsAsync<NotSupportedException>(async () => {
                var result = await jobRepository.UpdateOrder(job, updatedOrder, JobUpdateMode.smart);
            });   
        }

        [Test]
        public async Task Test_Cancel_Delivery_Job_With_No_Task_In_Progress()
        {
            string cancellationReason = "test cancellation reason";

            var replaceOneResult = new ReplaceOneResult.Acknowledged(1, 1, null);

            var createdJob = GetDummyJob(OrderTypes.Delivery);

            var jobManagerMock = new Mock<IJobManager>();
            jobManagerMock.Setup(x => x.UpdateJob(It.IsAny<Job>()))
                .ReturnsAsync(null);

            var userStoreMock = new Mock<IUserStore<User>>();

            var jobRepository = new JobRepository(jobManagerMock.Object,
                new AccountManager(userStoreMock.Object), activitySubject, jobIndexingSubject);

            var result = await jobRepository.CancelJob(createdJob, cancellationReason);

            Assert.IsNotNull(result);
            Assert.AreEqual(JobState.CANCELLED, result.UpdatedValue.State);
            Assert.AreEqual(cancellationReason, result.UpdatedValue.CancellationReason);
            Assert.AreEqual(JobTaskState.CANCELLED, result.UpdatedValue.Tasks.First().State);
        }

        [Test]
        public async Task Test_Restore_Delivery_Job_With_No_Task_In_Progress()
        {
            var createdJob = GetDummyJob(OrderTypes.Delivery);
            createdJob.State = JobState.CANCELLED;
            createdJob.Tasks.Last().State = JobTaskState.CANCELLED;

            var jobManagerMock = new Mock<IJobManager>();
            jobManagerMock.Setup(x => x.UpdateJob(It.IsAny<Job>()))
                .ReturnsAsync(null);

            var userStoreMock = new Mock<IUserStore<User>>();

            var jobRepository = new JobRepository(jobManagerMock.Object,
                new AccountManager(userStoreMock.Object), activitySubject, jobIndexingSubject);

            var result = await jobRepository.RestoreJob(createdJob);

            Assert.IsNotNull(result);
            Assert.AreEqual(JobState.ENQUEUED, result.UpdatedValue.State);
            result.UpdatedValue.Tasks.ForEach(x => Assert.AreEqual(JobTaskState.PENDING, x.State));
            Assert.AreEqual(null, result.UpdatedValue.CancellationReason);
        }

        [Test]
        public void Test_Update_ClassifiedDeliveryJob_DeliveryTask_To_Returned_ReturnDeliveryTask_Added()
        {
            JobRepository jobRepository = SetupMockJobRepositoryForUpdate();
            var job = GetDummyJob(OrderTypes.ClassifiedDelivery);

            // Registering extensions
            DeliveryJobExtensions.RegisterExtensions();

            // Assigning asset and making sure the first task is done
            job.State = JobState.IN_PROGRESS;
            job.Tasks.First().State = JobTaskState.COMPLETED;
            job.Tasks.First().Asset = GetDummyAssetModel();
            job.Tasks.First().UpdateTask();

            // Making sure Pickup is done too
            job.Tasks[1].State = JobTaskState.COMPLETED;
            job.Tasks[1].UpdateTask();

            // Now we should have delivery in progress
            // Update delivery to get returned.
            job.Tasks[2].State = JobTaskState.RETURNED;
            // Making sure multiple updates don't hurt the system
            job.Tasks[2].State = JobTaskState.RETURNED;
            job.Tasks[2].UpdateTask();

            Assert.AreEqual(4, job.Tasks.Count);
            Assert.That(job.Tasks.Last().Type == JobTaskTypes.DELIVERY);
            Assert.That(job.Tasks.Last().Variant == "return");
            Assert.That(job.TerminalTask == job.Tasks.Last());
            Assert.That(job.AttemptCount == 1);
        }

        [Test]
        public void Test_Update_ClassifiedDeliveryJob_DeliveryTask_To_Failed_RetryTask_Added()
        {
            JobRepository jobRepository = SetupMockJobRepositoryForUpdate();
            var job = GetDummyJob(OrderTypes.ClassifiedDelivery);

            // Registering extensions
            DeliveryJobExtensions.RegisterExtensions();

            // Assigning asset and making sure the first task is done
            job.State = JobState.IN_PROGRESS;
            job.Tasks.First().State = JobTaskState.COMPLETED;
            job.Tasks.First().Asset = GetDummyAssetModel();
            job.Tasks.First().UpdateTask();

            // Making sure Pickup is done too
            job.Tasks[1].State = JobTaskState.COMPLETED;
            job.Tasks[1].UpdateTask();

            // Now we should have delivery in progress
            // Update delivery to get returned.
            job.Tasks[2].State = JobTaskState.FAILED;
            job.Tasks[2].UpdateTask();

            Assert.AreEqual(5, job.Tasks.Count);
            Assert.That(job.Tasks[3].Type == JobTaskTypes.DELIVERY);
            Assert.That(job.Tasks[3].Variant == "retry");
            Assert.That(job.TerminalTask == job.Tasks.Last());
            Assert.That(job.AttemptCount == 2);
            Assert.That(job.Tasks.Count(x => x.IsTerminatingTask) == 1);
        }

        [Test]
        public void Test_Update_ClassifiedDeliveryJob_RetryDeliveryTask_To_Failed_RetryTaskAdded()
        {
            JobRepository jobRepository = SetupMockJobRepositoryForUpdate();
            var job = GetDummyJob(OrderTypes.ClassifiedDelivery);

            // Registering extensions
            DeliveryJobExtensions.RegisterExtensions();

            // Assigning asset and making sure the first task is done
            job.State = JobState.IN_PROGRESS;
            job.Tasks.First().State = JobTaskState.COMPLETED;
            job.Tasks.First().Asset = GetDummyAssetModel();
            job.Tasks.First().UpdateTask();

            // Making sure Pickup is done too
            job.Tasks[1].State = JobTaskState.COMPLETED;
            job.Tasks[1].UpdateTask();

            // Now we should have delivery in progress
            // Update delivery to FAILED.
            job.Tasks[2].State = JobTaskState.FAILED;
            job.Tasks[2].UpdateTask();

            // Lets even return the return job
            job.Tasks[3].State = JobTaskState.FAILED;
            job.Tasks[3].UpdateTask();

            Assert.AreEqual(6, job.Tasks.Count);
            Assert.AreEqual(JobTaskTypes.DELIVERY, job.Tasks[3].Type);
            Assert.AreEqual("retry", job.Tasks[3].Variant);
            Assert.AreEqual(JobTaskState.FAILED, job.Tasks[3].State);
            Assert.AreEqual(JobTaskTypes.DELIVERY, job.Tasks[4].Type);
            Assert.AreEqual("retry", job.Tasks[4].Variant);
            Assert.That(job.TerminalTask == job.Tasks.Last());
            Assert.AreEqual(3, job.AttemptCount);
            Assert.AreEqual(1, job.Tasks.Count(x => x.IsTerminatingTask));
        }

        private AssetModel GetDummyAssetModel()
        {
            var assetModel = new AssetModel()
            {
                AverageRating = 3.2,
                Email = "someAsset @asset.com",
                EmailConfirmed = true,
                IsUserAuthenticated = true,
                PhoneNumber = "123456789",
                PhoneNumberConfirmed = true,
                Profile = new AssetProfile()
                {
                    Address = new DefaultAddress("Somewhere in the world", new Point(1,2)),
                    Age = 25,
                    DriversLicenseId = "123456",
                    FirstName = "Some",
                    LastName = "Asset",
                    Gender = Gender.MALE,
                    NationalId = "e123r123",
                    PicUri = "http://some-pic-uri.img"
                },
                Type = IdentityTypes.BIKE_MESSENGER,
                UserId = "123456789",
                UserName = "someasset"
            };

            return assetModel;
        }

        private Job GetDummyJob(string orderType)
        {
            DeliveryOrder order = GetDummyOrder(orderType);

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
                UserName = "GabulTheAwesome",  
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

        private DeliveryOrder GetDummyOrder(string orderType)
        {
            DeliveryOrder order = new DeliveryOrder();
            order.UserId = "abcdef123ijkl12";
            order.From = new DefaultAddress("Test From Address", new Point((new double[] { 1, 2 }).ToList()));
            order.To = new DefaultAddress("Test To Address", new Point((new double[] { 2, 1 }).ToList()));
            order.PaymentMethod = "SamplePaymentMethod";
            order.Type = orderType;
            order.ReferenceInvoiceId = "S1ML-1NV01C31";
            return order;
        }

        private OrderDetails GetDummyCart()
        {
            OrderDetails orderDetails = new OrderDetails();
            orderDetails.PackageList = new List<ItemDetails>();
            orderDetails.PackageList.Add(new ItemDetails()
            {
                Item = "Item 1",
                PicUrl = "http://sample-pic-source/pic.png",
                Price = 10,
                Quantity = 1,
                VAT = 15,
                Weight = 0.2M
            });

            orderDetails.PackageList.Add(new ItemDetails()
            {
                Item = "Item 2",
                PicUrl = "http://sample-pic-source/pic2.png",
                Price = 10,
                Quantity = 1,
                VAT = 15,
                Weight = 0.2M
            });

            orderDetails.ServiceCharge = 100;
            orderDetails.SubTotal = 23;
            orderDetails.TotalVATAmount = 3;
            orderDetails.TotalWeight = 0.4M;
            orderDetails.TotalToPay = 123;

            return orderDetails;
        }
    }
}
