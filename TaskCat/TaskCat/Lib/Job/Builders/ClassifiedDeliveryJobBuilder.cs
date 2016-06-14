namespace TaskCat.Lib.Job.Builders
{
    using System;
    using Data.Model;
    using Data.Model.Identity.Response;
    using HRID;
    using Data.Lib.Payment;
    using Data.Model.Order;

    public class ClassifiedDeliveryJobBuilder : JobBuilder
    {
        private IPaymentMethod paymentMethod;
        private DeliveryOrder order;

        public ClassifiedDeliveryJobBuilder(string name) : base(name)
        {
        }

        public ClassifiedDeliveryJobBuilder(OrderModel order, UserModel userModel, IHRIDService hridService) : base(order, userModel, hridService)
        {
        }

        public ClassifiedDeliveryJobBuilder(OrderModel order, UserModel userModel, UserModel adminUserModel, IHRIDService hridService) : base(order, userModel, adminUserModel, hridService)
        {
        }

        public override void BuildJob()
        {
            throw new NotImplementedException();
        }
    }
}