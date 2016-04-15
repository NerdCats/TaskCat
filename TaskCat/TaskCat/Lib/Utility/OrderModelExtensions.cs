namespace TaskCat.Lib.Utility
{
    using Data.Model;
    using Data.Model.Identity.Response;
    public static class OrderModelExtensions
    {
        public static void GenerateDefaultName(this OrderModel order, UserModel userModel)
        {
            order.Name = string.Format("{0} Job for {1}", order.Type.ToString(), string.IsNullOrWhiteSpace(userModel.UserName) ? order.UserId : userModel.UserName);
        }
    }
}