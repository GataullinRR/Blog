using System;
using Xunit;

namespace BlogTests
{
    /// <summary>
    /// Used by CustomOrderer
    /// </summary>
    public class OrderAttribute : Attribute
    {
        public int I { get; }

        public OrderAttribute(int i)
        {
            I = i;
        }
    }
}
