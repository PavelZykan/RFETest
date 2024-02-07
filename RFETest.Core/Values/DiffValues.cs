using System.ComponentModel.DataAnnotations;

namespace RFETest.Core.Values
{
    /// <summary>
    /// Holds left and right values
    /// </summary>
    public class DiffValues
    {
        public string Left { get; }

        public string Right { get; }

        public DiffValues(string left, string right)
        {
            Left = left;
            Right = right;
        }
    }
}