namespace TaskCat.Tests.Job
{
    using NUnit.Framework;
    using Data.Model.Order;
    using Data.Model;
    using Data.Model.GeoJson;
    using System.Linq;
    using Lib.Job.Builders;
    using Data.Model.JobTasks;

    [TestFixture]
    public class TestDeliveryJobBuilding
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
            Assert.AreEqual(JobState.IN_PROGRESS, builder.Job.State);
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

            //First the job would be in progress because the FetchDeliveryTask initiates that way
            Assert.AreEqual(JobState.IN_PROGRESS, builder.Job.State);
            //Moving it to Enqueued state
            builder.Job.State = JobState.ENQUEUED;
            //Moving first task state to pending
            builder.Job.Tasks.First().State = JobTaskState.PENDING;
            //Changing that back to IN PROGRESS
            builder.Job.Tasks.First().State = JobTaskState.IN_PROGRESS;
            Assert.AreEqual(JobState.IN_PROGRESS, builder.Job.State);
        }

    }
}
