namespace TaskCat.Data.Model.Tag
{
    using Entity;

    public class TagIndexOperation
    {
        public string Operation { get; set; }
        public DataTag Value { get; set; }

        public TagIndexOperation(string operation, DataTag value)
        {
            this.Operation = operation;
            this.Value = value;
        }
    }
}
