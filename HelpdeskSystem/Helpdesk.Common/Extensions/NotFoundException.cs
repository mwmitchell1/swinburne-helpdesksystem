using System;
using System.Collections.Generic;
using System.Text;

namespace Helpdesk.Common.Extensions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message)
        {

        }
    }
}
