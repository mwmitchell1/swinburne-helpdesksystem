using System;
using System.Collections.Generic;
using System.Text;

namespace Helpdesk.Common.Responses.Helpdesk
{
    public class DatabaseExportResponse : BaseResponse
    {
        public byte[] File { get; set; }
        public string Path { get; set; }
    }
}
