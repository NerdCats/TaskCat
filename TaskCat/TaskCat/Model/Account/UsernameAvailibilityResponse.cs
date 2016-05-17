namespace TaskCat.Model.Account
{
    public class UsernameAvailibilityResponse
    {
        public string SuggestedUsername { get; private set; }
        public bool IsAvailable { get; private set; }

        public UsernameAvailibilityResponse(string suggestedUsername, bool isAvailable)
        {
            this.SuggestedUsername = suggestedUsername;
            this.IsAvailable = isAvailable;
        }
    }
}