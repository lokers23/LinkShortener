using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkShortener.Domain.Enums
{
    public enum StatusCode
    {
        ServerError = 500,
        ValidationError = 400,
        NotFound = 404,
        Success = 200,
    }
}
