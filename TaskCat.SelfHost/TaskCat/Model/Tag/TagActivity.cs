namespace TaskCat.Model.Tag
{
    using Data.Entity;

    public class TagActivity
    {
        public TagOperation Operation { get; set; }

        public DataTag OldValue { get; set; }
        public DataTag Value { get; set; }

        public TagActivity(TagOperation op, DataTag value, DataTag oldValue = null)
        {
            this.Operation = op;
            this.Value = value;
            this.OldValue = oldValue;
        }
    }
}
