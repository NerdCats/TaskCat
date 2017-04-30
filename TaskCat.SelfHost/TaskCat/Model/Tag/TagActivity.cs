namespace TaskCat.Model.Tag
{
    using Data.Entity;

    public class TagActivity
    {
        public TagOperation Operation { get; set; }
        public DataTag Value { get; set; }

        public TagActivity(TagOperation op, DataTag value)
        {
            this.Operation = op;
            this.Value = value;
        }
    }
}
