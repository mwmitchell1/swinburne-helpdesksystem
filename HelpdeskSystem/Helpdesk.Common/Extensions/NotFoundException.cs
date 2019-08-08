using System;
using System.Collections.Generic;
using System.Text;

namespace Helpdesk.Common.Extensions
{
    /// <summary>
    /// Used to indicate that certain information coud not be found in the database
    /// </summary>
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message)
        {

        }
    }
}
