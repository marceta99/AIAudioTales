using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kumadio.Core.Common
{
    public static class DomainErrors
    {
        public static class Transaction
        {
            public static Error Failed(string message) => new Error("TRANSACTION_FAILED", message);
        }
        public static class Auth
        {

        }
    }
}
