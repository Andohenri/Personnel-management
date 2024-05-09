using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppManagement.utils
{
    class ErrorValidationException : Exception
    {
        public ErrorValidationException(string message) : base(message)
        {
        }
    }
}
