namespace Convai.RestAPI.Result
{
#nullable enable
    public class OperationResult
    {
        public bool IsCompleted { get; private set; }
        public bool WasSuccess { get; private set; }
        public string? ErrorMessage { get; protected set; }

        protected void SetCompletion(bool success, string error = "")
        {
            IsCompleted = true;
            WasSuccess = success;
            ErrorMessage = success ? null : error;
        }


    }

    public class OperationResult<T>
    {
        public bool IsCompleted { get; private set; }
        public bool WasSuccess { get; private set; }
        public T? Result { get; private set; }
        public string? ErrorMessage { get; protected set; }

        protected void SetCompletion(bool success, T result, string error = "")
        {
            IsCompleted = true;
            WasSuccess = success;
            Result = result;
            ErrorMessage = success ? null : error;
        }
    }
}