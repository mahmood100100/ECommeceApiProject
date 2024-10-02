using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Core.Entities
{
    public class ApiValidationResponse : ApiResponse
    {
        //public HttpStatusCode StatusCode { get; set; }
        //public string Message { get; set; }
        public IEnumerable<string> Errors { get; set; }
       // public bool IsSuccess { get; set; }

        public ApiValidationResponse(IEnumerable<string>? Errors = null , int? StatusCode = 400) : base(StatusCode)
        {
            this.Errors = Errors ?? new List<string>();
        }
    }
}
