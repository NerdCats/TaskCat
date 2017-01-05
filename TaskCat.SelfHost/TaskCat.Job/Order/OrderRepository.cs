namespace TaskCat.Job.Order
{
    using Job;
    using Job.Builders;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Data.Entity;
    using Data.Model;
    using Data.Model.Order;
    using Data.Model.Identity.Response;
    using Common.HRID;
    using Process;
    using Data.Lib.Payment;
    using Data.Model.Identity;
    using Data.Model.Identity.Profile;
    using Data.Entity.Identity;
    using Account.Core;
    using Vendor;
    using Payment.Core;

    public class OrderRepository : IOrderRepository
    {
        IJobManager manager;
        SupportedOrderStore supportedOrderStore;
        AccountManager accountManager;
        IHRIDService hridService;
        IOrderCalculationService orderCalculationService;
        IServiceChargeCalculationService serviceChargeCalculationService;
        IOrderProcessor orderProcessor;
        IPaymentService paymentService;
        private IVendorService vendorService;
        private IObserver<Job> jobSearchIndexService;

        public OrderRepository(
            IJobManager manager,
            SupportedOrderStore supportedOrderStore,
            AccountManager accountManager,
            IHRIDService hridService,
            IPaymentManager paymentManager,
            IVendorService vendorService,
            IObserver<Job> jobSearchIndexSubject)
        {
            this.manager = manager;
            this.supportedOrderStore = supportedOrderStore;
            this.accountManager = accountManager;
            this.hridService = hridService;
            this.vendorService = vendorService;
            this.jobSearchIndexService = jobSearchIndexSubject;

            orderCalculationService = new DefaultOrderCalculationService();
            serviceChargeCalculationService = new DefaultDeliveryServiceChargeCalculationService();
            paymentService = new PaymentService(paymentManager);
        }

        public async Task<Job> PostOrder(OrderModel model, string adminUserId)
        {
            return await PostOrderToJob(model, adminUserId);
        }

        public async Task<Job> PostOrder(OrderModel model)
        {
            return await PostOrderToJob(model);
        }

        private async Task<Job> PostOrderToJob(OrderModel model, string adminUserId = null)
        {
            UserModel userModel = new UserModel(await accountManager.FindByIdAsync(model.UserId));
            UserModel adminUserModel = null;

            Vendor vendor = default(Vendor);
            if (!string.IsNullOrWhiteSpace(model.VendorId))
            {
                vendor = await vendorService.Get(model.VendorId);
            }

            if (!string.IsNullOrWhiteSpace(adminUserId))
            {
                adminUserModel = new UserModel(await accountManager.FindByIdAsync(adminUserId));
            }

            JobShop jobShop = new JobShop();
            JobBuilder builder;

            switch (model.Type)
            {
                case OrderTypes.ClassifiedDelivery:
                    {
                        ClassifiedDeliveryOrder classifiedDeliveryOrderModel = model as ClassifiedDeliveryOrder;
                        if (!string.IsNullOrWhiteSpace(classifiedDeliveryOrderModel.BuyerInfo?.UserRef))
                        {
                            var user = await accountManager.FindByIdAsync(classifiedDeliveryOrderModel.BuyerInfo.UserRef);
                            classifiedDeliveryOrderModel.BuyerInfo.PhoneNumber = user.PhoneNumber;
                            classifiedDeliveryOrderModel.BuyerInfo.Address = user.Profile.Address;
                            classifiedDeliveryOrderModel.BuyerInfo.Name = GetNameFromProfile(user);
                        }
                        if (!string.IsNullOrWhiteSpace(classifiedDeliveryOrderModel.SellerInfo?.UserRef))
                        {
                            var user = await accountManager.FindByIdAsync(classifiedDeliveryOrderModel.SellerInfo.UserRef);
                            classifiedDeliveryOrderModel.SellerInfo.PhoneNumber = user.PhoneNumber;
                            classifiedDeliveryOrderModel.SellerInfo.Address = user.Profile.Address;
                            classifiedDeliveryOrderModel.SellerInfo.Name = GetNameFromProfile(user);
                        }
                        builder = GetDeliveryJobBuilder(userModel, adminUserModel, classifiedDeliveryOrderModel, vendor);
                        break;
                    }
                case OrderTypes.Delivery:
                    {
                        DeliveryOrder deliveryOrderModel = model as DeliveryOrder;
                        builder = GetDeliveryJobBuilder(userModel, adminUserModel, deliveryOrderModel, vendor);
                        break;
                    }
                default:
                    throw new InvalidOperationException("Invalid/Not supported Order Type Provided");
            }
            var result = await ConstructAndRegister(jobShop, builder);
            this.jobSearchIndexService.OnNext(result);
            return result;
        }

        private string GetNameFromProfile(User user)
        {
            switch (user.Type)
            {
                case IdentityTypes.USER:
                    return (user.Profile as UserProfile).FullName;
                case IdentityTypes.ENTERPRISE:
                    return (user.Profile as EnterpriseUserProfile).CompanyName;
                default:
                    return (user.Profile as AssetProfile).FullName;
            }
        }

        private JobBuilder GetDeliveryJobBuilder(UserModel userModel, UserModel adminUserModel, DeliveryOrder deliveryOrderModel, Vendor vendor)
        {
            JobBuilder builder;
            orderProcessor = new DeliveryOrderProcessor(
                      orderCalculationService,
                      serviceChargeCalculationService);
            var paymentMethod = paymentService.GetPaymentMethodByKey(deliveryOrderModel.PaymentMethod);
            orderProcessor.ProcessOrder(deliveryOrderModel);

            // Resolve appropriate profit sharing strategy here

            builder = adminUserModel == null ?
                new DeliveryJobBuilder(deliveryOrderModel, userModel, hridService, paymentMethod, vendor)
                : new DeliveryJobBuilder(deliveryOrderModel, userModel, adminUserModel, hridService, paymentMethod, vendor);
            return builder;
        }

        private async Task<Job> ConstructAndRegister(JobShop jobShop, JobBuilder builder)
        {
            var createdJob = jobShop.Construct(builder);
            var result = await manager.RegisterJob(createdJob);
            return result;
        }

        public async Task<SupportedOrder> PostSupportedOrder(SupportedOrder supportedOrder)
        {
            await supportedOrderStore.Post(supportedOrder);
            return supportedOrder;
        }

        public async Task<List<SupportedOrder>> GetAllSupportedOrder()
        {
            return await supportedOrderStore.GettAll();
        }

        public async Task<SupportedOrder> GetSupportedOrder(string id)
        {
            return await supportedOrderStore.Get(id);
        }

        public async Task<SupportedOrder> UpdateSupportedOrder(SupportedOrder order)
        {
            return await supportedOrderStore.Replace(order);
        }

        public async Task<SupportedOrder> DeleteSupportedOrder(string id)
        {
            return await supportedOrderStore.Delete(id);
        }
    }
}