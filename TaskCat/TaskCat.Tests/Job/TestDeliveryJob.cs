namespace TaskCat.Tests.Job
{
    using NUnit.Framework;
    using Data.Model.Order;
    using Data.Model;
    using Data.Model.GeoJson;
    using System.Linq;
    using Lib.Job.Builders;
    using Data.Model.JobTasks;
    using Data.Model.Identity.Response;
    using Data.Model.Identity.Profile;
    [TestFixture]
    public class TestDeliveryJob
    {
        [Test]
        public void Test_DeliverJobBuilder_Creation()
        {
            DeliveryOrder order = new DeliveryOrder();
            order.From = new Location() { Address = "Test From Address", Point = new Point((new double[] { 1, 2 }).ToList()) };
            order.To = new Location() { Address = "Test To Address", Point = new Point((new double[] { 2, 1 }).ToList()) };

            var builder = new DeliveryJobBuilder(order);

            Assert.IsNotNull(builder);
            Assert.IsNotNull(builder.Job);
            Assert.AreEqual(order, builder.Job.Order);
            Assert.AreEqual("Anonymous", builder.Job.User);
            Assert.That(builder.Job.Assets != null && builder.Job.Assets.Count == 0);
            Assert.AreEqual("Anonymous", builder.Job.JobServedBy);
            Assert.NotNull(builder.Job.CreateTime);
            Assert.NotNull(builder.Job.ModifiedTime);
            Assert.AreEqual(JobState.ENQUEUED, builder.Job.State);
            Assert.False(builder.Job.IsAssetEventsHooked);
            Assert.Null(builder.Job.TerminalTask);
        }

        [Test]
        public void Test_DeliverJobBuilder_Creation_With_TaskInitiation()
        {
            string orderName = "Sample Delivery Order";

            DeliveryOrder order = new DeliveryOrder();
            order.Name = orderName;
            order.From = new Location() { Address = "Test From Address", Point = new Point((new double[] { 1, 2 }).ToList()) };
            order.To = new Location() { Address = "Test To Address", Point = new Point((new double[] { 2, 1 }).ToList()) };

            var builder = new DeliveryJobBuilder(order);
            builder.BuildTasks();

            Assert.IsNotNull(builder);
            Assert.IsNotNull(builder.Job);
            Assert.IsNotNull(builder.Job.Name);
            Assert.AreEqual(orderName, builder.Job.Name);
            Assert.AreEqual(order, builder.Job.Order);
            Assert.AreEqual("Anonymous", builder.Job.User);
            Assert.That(builder.Job.Assets != null && builder.Job.Assets.Count == 0);
            Assert.AreEqual("Anonymous", builder.Job.JobServedBy);
            Assert.NotNull(builder.Job.CreateTime);
            Assert.NotNull(builder.Job.ModifiedTime);
            Assert.IsNotNull(builder.Job.Tasks);
            Assert.AreEqual(3, builder.Job.Tasks.Count);
            Assert.That(builder.Job.Tasks.First().GetType() == typeof(FetchDeliveryManTask));
            Assert.That(builder.Job.Tasks[1].GetType() == typeof(PackagePickUpTask));
            Assert.That(builder.Job.Tasks.Last().GetType() == typeof(DeliveryTask));
            Assert.True(builder.Job.IsAssetEventsHooked);
            Assert.NotNull(builder.Job.TerminalTask);
            Assert.That(builder.Job.TerminalTask == builder.Job.Tasks.Last());
            Assert.AreEqual(JobState.ENQUEUED, builder.Job.State);
        }

        [Test]
        public void Test_DeliveryJob_State_Changes_To_InProgress_After_FirstTask_Passed_Or_Reached_InProgress()
        {
            string orderName = "Sample Delivery Order";

            DeliveryOrder order = new DeliveryOrder();
            order.Name = orderName;
            order.From = new Location() { Address = "Test From Address", Point = new Point((new double[] { 1, 2 }).ToList()) };
            order.To = new Location() { Address = "Test To Address", Point = new Point((new double[] { 2, 1 }).ToList()) };

            var builder = new DeliveryJobBuilder(order);
            builder.BuildTasks();

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
            order.From = new Location() { Address = "Test From Address", Point = new Point((new double[] { 1, 2 }).ToList()) };
            order.To = new Location() { Address = "Test To Address", Point = new Point((new double[] { 2, 1 }).ToList()) };

            var builder = new DeliveryJobBuilder(order);
            builder.BuildTasks();

            var SampleAssetModel = new AssetModel()
            {
                AverageRating = 0.0,
                Email = "someone@someone.com",
                EmailConfirmed = false,
                PhoneNumber = "+8801711111111",
                PhoneNumberConfirmed = true,
                Profile = new UserProfile()
                {
                    Address = "Some place in somewhere",
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
            order.From = new Location() { Address = "Test From Address", Point = new Point((new double[] { 1, 2 }).ToList()) };
            order.To = new Location() { Address = "Test To Address", Point = new Point((new double[] { 2, 1 }).ToList()) };

            var builder = new DeliveryJobBuilder(order);
            builder.BuildTasks();

            var SampleAssetModel = new AssetModel()
            {
                AverageRating = 0.0,
                Email = "someone@someone.com",
                EmailConfirmed = false,
                PhoneNumber = "+8801711111111",
                PhoneNumberConfirmed = true,
                Profile = new UserProfile()
                {
                    Address = "Some place in somewhere",
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

            Assert.That(builder.Job.Tasks.First().State == JobTaskState.COMPLETED);
            Assert.That(builder.Job.Tasks[1].State == JobTaskState.IN_PROGRESS);
        }


    }
}
