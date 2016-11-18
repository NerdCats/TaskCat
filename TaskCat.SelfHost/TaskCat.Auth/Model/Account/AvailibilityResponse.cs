namespace TaskCat.Auth.Model.Account
{
    public class AvailibilityResponse
    {
        public string Property { get; set; }
        public string SuggestedValue { get; private set; }
        public bool IsAvailable { get; private set; }

        public AvailibilityResponse(string property, string suggestedValue, bool isAvailable)
        {
            this.Property = property;
            this.SuggestedValue = suggestedValue;
            this.IsAvailable = isAvailable;
        }
    }
}