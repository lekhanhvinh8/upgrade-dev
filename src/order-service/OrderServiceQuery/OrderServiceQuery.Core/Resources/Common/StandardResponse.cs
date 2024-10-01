namespace OrderServiceQuery.Core.Resources.Common
{
    public class StandardResponse<TData, TErrorData> {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public TData? Data { get; set; }
        public TErrorData? DataError { get; set; }   
        public string? ExceptionMessage { get; set; } // Never publish it to the client, It should be sent to the log server. It just standardlization by the boss
        public Paging? Paging { get; set; }
        public string? ClientRequestId { get; set; } 

        public StandardResponse(TData? data, bool success = true)
        {
            this.Success = success;
            this.Data = data;
        }
    }

    public class StandardResponse<TData> : StandardResponse<TData, object>
    {
        public StandardResponse(TData data, bool success = true) 
          : base(data, success)
        {
        }
    }

    public class StandardResponse : StandardResponse<object, object>
    {
        public StandardResponse(bool success = true) 
          : base(null, success)
        {
        }
    }

    public class Paging
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
    }
}