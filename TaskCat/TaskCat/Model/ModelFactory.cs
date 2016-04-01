namespace TaskCat.Model
{
    using Data.Entity.Identity;
    using Data.Model.Identity.Registration;

    public static class ModelFactory
    {
        // FIXME: This is the model factory that actualy should return UserPublicModel, not RegistrationModel
        public static UserRegistrationModel ToModel(this User user)
        {
            return new UserRegistrationModel()
            {
                UserName = user.UserName,
                Address = user.Profile.Address,
                Age = user.Profile.Age,
                Type = user.Type

            };
        }
    }
}