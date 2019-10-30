using System;
using System.Collections.Generic;
using System.Text;

namespace Helpdesk.Common.Responses.Helpdesk
{
    /// <summary>
    /// Used to indicate the result of exporting the database helpdesk table
    /// </summary>
    public class DatabaseExportResponse : BaseResponse
    {
        public byte[] File { get; set; }
        public string Path { get; set; }
    }
}
