using System;
using System.Collections.Generic;
using System.Text;

namespace RFETest.Core.Values
{
    /// <summary>
    /// Thrown when one or more values are missing for the given comparison ID
    /// </summary>
    public class ComparisonIDIncompleteException : Exception
    {
        public ComparisonIDIncompleteException(string message) 
            : base(message)
        {            
        }
    }
}
