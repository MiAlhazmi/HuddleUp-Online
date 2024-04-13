namespace Model
{
    public class Result<T>
    {
        public bool IsSuccess { get; private set; } // Public getter, private setter
        public T Data { get; private set; } // Public getter, private setter
        public string Message { get; private set; } // Public getter, private setter

        public Result(bool success, T data, string message)
        {
            this.IsSuccess = success;
            this.Data = data;
            this.Message = message;
        }
    } 
}