using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Helpdesk.Common.Responses
{
    /// <summary>
    /// Used as a base for all responses so that they contain status codes and messages
    /// </summary>
    public class BaseResponse
    {
        public BaseResponse()
        {
            StatusMessages = new List<StatusMessage>();
        }

        public List<StatusMessage> StatusMessages { get; set; }

        public HttpStatusCode Status = HttpStatusCode.InternalServerError;
    }

    /// <summary>
    /// Status message that is used to indicate the results of processes
    /// </summary>
    public class StatusMessage
    {
        public HttpStatusCode MessageStatus { get; set; }

        public string Message { get; set; }

        public StatusMessage(HttpStatusCode statusCode, string message)
        {
            MessageStatus = statusCode;
            Message = message;
        }
    }
}
