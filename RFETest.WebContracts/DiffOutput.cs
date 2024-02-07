using System;
using System.Collections.Generic;
using System.Text;

namespace RFETest.WebContracts
{
    public class DiffOutput
    {
        public string Message { get; set; }

        public EDiffResultType Result { get; set; }

        public IEnumerable<Difference> Differences { get; set; }
    }
}
