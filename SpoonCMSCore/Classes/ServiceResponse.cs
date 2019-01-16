using System;
using System.Collections.Generic;
using System.Text;

namespace SpoonCMSCore.Classes
{
    public class ServiceResponse<T>
    {
        public string Message { get; set; }
        public bool Success { get; set; }
        public T Data { get; set; }

        public void SetToError(Exception ex)
        {
            this.Data = default(T);
            this.Message = ex.Message;
            this.Success = false;
        }
    }
}
