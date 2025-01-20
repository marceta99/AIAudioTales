using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kumadio.Core.Common
{
    public record Error
    {
        public string Code { get; init; }
        public string Message { get; init; }

        /// <summary>
        /// Optional metadata for additional context (e.g. validation fields, stack traces).
        /// </summary>
        public IReadOnlyDictionary<string, object>? Metadata { get; init; }

        public Error(string code, string message,
           IReadOnlyDictionary<string, object>? metadata = null)
        {
            Code = code;
            Message = message;
            Metadata = metadata;
        }
    }
}
