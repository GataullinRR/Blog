using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EFCore22Utilities.Extensions
{
    public static class CommonEx
    {
        public static IQueryable<T> AsAsyncQuerable<T>(this IEnumerable<T> sequence)
        {
            return new TestAsyncEnumerable<T>(sequence).AsQueryable();
        }
    }
}
