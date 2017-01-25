namespace TaskCat.Tests.Job
{
    using NUnit.Framework;
    using Data.Model.Order;
    using Data.Model;
    using Data.Model.GeoJson;
    using System.Linq;
    using Data.Model.JobTasks;
    using Data.Model.Identity.Response;
    using Data.Model.Identity.Profile;
    using Data.Model.Geocoding;
    using Moq;
    using Data.Lib.Payment;
    using System.Collections.Generic;
    using Data.Model.JobTasks.Preference;
    using System;
    using Data.Lib.Constants;
    using System.ComponentModel.DataAnnotations;
    using TaskCat.Job.Builders;
    using Data.Model.Order.Delivery;
    using Common.HRID;

    [TestFixture(TestOf = typeof(DeliveryJobBuilder))]
    public class TestDeliveryJobBuilder
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
        public void Test_DeliverJobBuilder_Creation()
        {
            DeliveryOrder order = new DeliveryOrder();
            order.From = new DefaultAddress("Test From Address", new Point((new double[] { 1, 2 }).ToList()));
            order.To = new DefaultAddress("Test To Address", new Point((new double[] { 2, 1 }).ToList()));
            order.UserId = "SampleUserId";
            order.PaymentMethod = "SamplePaymentMethod";
            order.ReferenceInvoiceId = "SampleReferenceInvoiceId";

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

            Validator.ValidateObject(order, new ValidationContext(order), validateAllProperties: true);

            var builder = new DeliveryJobBuilder(order, userModel, backendAdminModel, hridService, paymentMethodMock.Object);

            Assert.IsNotNull(builder);
            Assert.IsNotNull(builder.Job);
            Assert.AreEqual(order, builder.Job.Order);
            Assert.AreEqual(MockedHrid, builder.Job.HRID);
            Assert.AreEqual(userModel, builder.Job.User);
            Assert.That(builder.Job.Assets != null && builder.Job.Assets.Count == 0);
            Assert.AreEqual(backendAdminModel, builder.Job.JobServedBy);
            Assert.NotNull(builder.Job.CreateTime);
            Assert.NotNull(builder.Job.ModifiedTime);
            Assert.AreEqual(JobState.ENQUEUED, builder.Job.State);
            Assert.False(builder.Job.IsJobTasksEventsHooked);
            Assert.Null(builder.Job.TerminalTask);
        }

        [Test]
        public void Test_DeliverJobBuilder_Creation_With_TaskInitiation()
        {
            string orderName = "Sample Delivery Order";

            DeliveryOrder order = new DeliveryOrder();
            order.Name = orderName;
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

            var jobTaskETAPreferences = new List<JobTaskETAPreference>();

            var testETA = DateTime.Now.AddDays(1);
            jobTaskETAPreferences.Add(new JobTaskETAPreference()
            {
                ETA = testETA,
                Type = "TestJobTaskType"
            });

            jobTaskETAPreferences.Add(new JobTaskETAPreference()
            {
                ETA = testETA,
                Type = JobTaskTypes.PACKAGE_PICKUP
            });

            order.JobTaskETAPreference = jobTaskETAPreferences;


            var builder = new DeliveryJobBuilder(order, userModel, backendAdminModel, hridService, paymentMethodMock.Object);
            builder.BuildJob();

            Assert.IsNotNull(builder);
            Assert.IsNotNull(builder.Job);
            Assert.IsNotNull(builder.Job.Name);
            Assert.AreEqual(MockedHrid, builder.Job.HRID);
            Assert.AreEqual(orderName, builder.Job.Name);
            Assert.AreEqual(order, builder.Job.Order);
            Assert.AreEqual(userModel, builder.Job.User);
            Assert.That(builder.Job.Assets != null && builder.Job.Assets.Count == 0);
            Assert.AreEqual(backendAdminModel, builder.Job.JobServedBy);
            Assert.NotNull(builder.Job.CreateTime);
            Assert.NotNull(builder.Job.ModifiedTime);
            Assert.IsNotNull(builder.Job.Tasks);
            Assert.AreEqual(3, builder.Job.Tasks.Count);
            Assert.That(builder.Job.Tasks.First().GetType() == typeof(FetchDeliveryManTask));
            Assert.That(builder.Job.Tasks[1].GetType() == typeof(PackagePickUpTask));
            Assert.That(builder.Job.Tasks.Last().GetType() == typeof(DeliveryTask));
            Assert.True(builder.Job.IsJobTasksEventsHooked);
            Assert.NotNull(builder.Job.TerminalTask);
            Assert.That(builder.Job.TerminalTask == builder.Job.Tasks.Last());
            Assert.AreEqual(JobState.ENQUEUED, builder.Job.State);
            Assert.AreEqual(testETA, builder.Job.Tasks[1].ETA);
        }

        [Test]
        public void Test_DeliveryJob_State_Changes_To_InProgress_After_FirstTask_Passed_Or_Reached_InProgress()
        {
            string orderName = "Sample Delivery Order";

            DeliveryOrder order = new DeliveryOrder();
            order.Name = orderName;
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



            var builder = new DeliveryJobBuilder(order, userModel, hridService, paymentMethodMock.Object);
            builder.BuildJob();

            //Changing that back to IN PROGRESS
            builder.Job.Tasks.First().State = JobTaskState.IN_PROGRESS;
            Assert.AreEqual(JobState.IN_PROGRESS, builder.Job.State);
        }

        [Test]
        public void Test_DeliveryJob_State_Progress_When_Asset_Assignment_Is_InProgress()
        {
            string orderName = "Sample Delivery Order";

            DeliveryOrder order = new DeliveryOrder();
            order.Name = orderName;
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

            var builder = new DeliveryJobBuilder(order, userModel, hridService, paymentMethodMock.Object);
            builder.BuildJob();

            var SampleAssetModel = new AssetModel()
            {
                AverageRating = 0.0,
                Email = "someone@someone.com",
                EmailConfirmed = false,
                PhoneNumber = "+8801711111111",
                PhoneNumberConfirmed = true,
                Profile = new UserProfile()
                {
                    Address = new DefaultAddress("Somewhere User belong", new Point(2, 1)),
                    Age = 20,
                    FirstName = "John",
                    LastName = "Doe",
                    Gender = Gender.MALE,
                    PicUri = null
                },
                Type = Data.Model.Identity.IdentityTypes.BIKE_MESSENGER,
                UserId = "12345678",
                UserName = "SampleUserName"
            };

            builder.Job.Tasks.First().State = JobTaskState.IN_PROGRESS;
            builder.Job.Tasks.First().UpdateTask();

            Assert.That(builder.Job.Tasks.First().State == JobTaskState.IN_PROGRESS);
            Assert.That(builder.Job.State == JobState.IN_PROGRESS);
        }

        [Test]
        public void Test_NextTask_State_Progress_After_Asset_Assignment_In_First_Task()
        {
            string orderName = "Sample Delivery Order";

            DeliveryOrder order = new DeliveryOrder();
            order.Name = orderName;
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

            var builder = new DeliveryJobBuilder(order, userModel, hridService, paymentMethodMock.Object);
            builder.BuildJob();

            var SampleAssetModel = new AssetModel()
            {
                AverageRating = 0.0,
                Email = "someone@someone.com",
                EmailConfirmed = false,
                PhoneNumber = "+8801711111111",
                PhoneNumberConfirmed = true,
                Profile = new UserProfile()
                {
                    Address = new DefaultAddress("Somewhere User belong", new Point(2, 1)),
                    Age = 20,
                    FirstName = "John",
                    LastName = "Doe",
                    Gender = Gender.MALE,
                    PicUri = null
                },
                Type = Data.Model.Identity.IdentityTypes.BIKE_MESSENGER,
                UserId = "12345678",
                UserName = "SampleUserName"
            };

            builder.Job.Tasks.First().State = JobTaskState.IN_PROGRESS;
            builder.Job.Tasks.First().Asset = SampleAssetModel;
            builder.Job.Tasks.First().UpdateTask();

            Assert.That(builder.Job.State == JobState.IN_PROGRESS);
            Assert.That(builder.Job.Tasks.First().State == JobTaskState.COMPLETED);
            Assert.That(builder.Job.Tasks[1].State == JobTaskState.IN_PROGRESS);
        }

        [Test]
        public void Test_NextTask_State_Progress_After_Asset_Assignment_In_First_Task_Without_MOVING_TO_IN_PROGRESS()
        {
            string orderName = "Sample Delivery Order";

            DeliveryOrder order = new DeliveryOrder();
            order.Name = orderName;
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

            var builder = new DeliveryJobBuilder(order, userModel, hridService, paymentMethodMock.Object);
            builder.BuildJob();

            var SampleAssetModel = new AssetModel()
            {
                AverageRating = 0.0,
                Email = "someone@someone.com",
                EmailConfirmed = false,
                PhoneNumber = "+8801711111111",
                PhoneNumberConfirmed = true,
                Profile = new UserProfile()
                {
                    Address = new DefaultAddress("Somewhere User belong", new Point(2, 1)),
                    Age = 20,
                    FirstName = "John",
                    LastName = "Doe",
                    Gender = Gender.MALE,
                    PicUri = null
                },
                Type = Data.Model.Identity.IdentityTypes.BIKE_MESSENGER,
                UserId = "12345678",
                UserName = "SampleUserName"
            };

            builder.Job.Tasks.First().Asset = SampleAssetModel;
            builder.Job.Tasks.First().State = JobTaskState.IN_PROGRESS;
            builder.Job.Tasks.First().UpdateTask();

            Assert.That(builder.Job.State == JobState.IN_PROGRESS);
            Assert.That(builder.Job.Tasks.First().State == JobTaskState.COMPLETED);
            Assert.That(builder.Job.Tasks[1].State == JobTaskState.IN_PROGRESS);
        }

        [Test]
        public void Test_DeliverJobBuilder_Creation_With_ClassifiedDelivery_TaskInitiation()
        {
            string orderName = "Classified  Delivery Order";

            DeliveryOrder order = new DeliveryOrder();
            order.Name = orderName;
            order.From = new DefaultAddress("Test From Address", new Point((new double[] { 1, 2 }).ToList()));
            order.To = new DefaultAddress("Test To Address", new Point((new double[] { 2, 1 }).ToList()));
            order.Type = OrderTypes.ClassifiedDelivery;

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

            Assert.IsNotNull(builder);
            Assert.IsNotNull(builder.Job);
            Assert.IsNotNull(builder.Job.Name);
            Assert.AreEqual(MockedHrid, builder.Job.HRID);
            Assert.AreEqual(orderName, builder.Job.Name);
            Assert.AreEqual(order, builder.Job.Order);
            Assert.AreEqual(userModel, builder.Job.User);
            Assert.That(builder.Job.Assets != null && builder.Job.Assets.Count == 0);
            Assert.AreEqual(backendAdminModel, builder.Job.JobServedBy);
            Assert.NotNull(builder.Job.CreateTime);
            Assert.NotNull(builder.Job.ModifiedTime);
            Assert.IsNotNull(builder.Job.Tasks);
            Assert.AreEqual(4, builder.Job.Tasks.Count);
            Assert.That(builder.Job.Tasks.First().GetType() == typeof(FetchDeliveryManTask));
            Assert.That(builder.Job.Tasks[1].GetType() == typeof(PackagePickUpTask));
            Assert.That(builder.Job.Tasks.Last().GetType() == typeof(SecureDeliveryTask));
            Assert.True(builder.Job.IsJobTasksEventsHooked);
            Assert.NotNull(builder.Job.TerminalTask);
            Assert.That(builder.Job.TerminalTask == builder.Job.Tasks.Last());
            Assert.AreEqual(JobState.ENQUEUED, builder.Job.State);
        }
    }
}
