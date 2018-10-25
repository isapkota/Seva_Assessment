using System;
using System.Collections.Generic;
using System.Text;

namespace Seva.Assessment.DataService
{
    public class HandledException : Exception
    {
        public HandledException()
        { }

        
        public HandledException(string message)
            : base(message)
        { }

        
        public HandledException(string message, Exception inner)
            : base(message, inner)
        { }
    }
}
