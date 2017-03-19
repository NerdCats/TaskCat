using TaskCat.Data.Entity;

namespace TaskCat.Model.Tag
{
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
