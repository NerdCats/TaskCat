namespace TaskCat.Data.Model
{
    public enum JobTaskState
    {
        PENDING = 1,
        IN_PROGRESS = 2,
        COMPLETED = 4,
        CANCELLED = 8,
        RETURNED = 16,
        FAILED = 32
    }
}
