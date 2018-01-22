using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LambdaExpressionBuilder.Attributes
{
    internal class NumberOfValuesAttribute : Attribute
    {
        [Range(0, 2, ErrorMessage = "Operations may only have from none to two values.")]
        [DefaultValue(1)]
        public int NumberOfValues { get; private set; }
        [DefaultValue(false)]
        public bool AllowMatchAll { get; private set; }
        [DefaultValue(false)]
        public bool AllowMatchAny { get; private set; }

        /// <summary>
        /// Defines the number of values supported by the operation.
        /// </summary>
        /// <param name="numberOfValues">Number of values the operation demands.</param>
        /// <param name="allowMatchAll">Can the function allow match all functions on arrays of various sizes.</param>
        /// <param name="allowMatchAny">Can the function allow match any functions on arrays of various sizes.</param>
        public NumberOfValuesAttribute(int numberOfValues = 1, bool allowMatchAll = false, bool allowMatchAny = false)
        {
            NumberOfValues = numberOfValues;
            AllowMatchAll = allowMatchAll;
            AllowMatchAny = allowMatchAny;
        }
    }
}
