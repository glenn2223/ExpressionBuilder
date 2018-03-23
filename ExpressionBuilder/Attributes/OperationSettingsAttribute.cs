using LambdaExpressionBuilder.Common;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LambdaExpressionBuilder.Attributes
{
    internal class OperationSettingsAttribute : Attribute
    {
        public MatchType DefaultMatchType { get; private set; }
        public bool AllowOtherMatchType { get; private set; }
        [Range(0, 2, ErrorMessage = "Operations may only have from none to two values.")]
        [DefaultValue(1)]
        public int NumberOfValues { get; private set; }

        /// <summary>
        /// Used to define default <see cref="MatchType"/>, if the alternate <see cref="MatchType"/> is allowed and the number of values supported by the operation when <see cref="MatchType"/> is not allowed.
        /// </summary>
        /// <param name="defaultMatchType">What default <see cref="MatchType"/> should be used on arrays/lists. When N/A, use <see cref="MatchType.All"/> to stop exceptions.</param>
        /// <param name="allowOtherMatchType">Is the other <see cref="MatchType"/> allowed?</param>
        /// <param name="numberOfValues">Number of values the operation demands. When not matching against an array/list.</param>
        /// <exception cref="ArgumentException">The <see cref="DefaultMatchType"/> can not be <see cref="MatchType.Default"/>.</exception>
        public OperationSettingsAttribute(MatchType defaultMatchType, bool allowOtherMatchType, int numberOfValues = 1)
        {
            if (defaultMatchType == MatchType.Default)
                throw new ArgumentException("The default \"MatchType\" can not be \"Default\".", "defaultMatchType");

            DefaultMatchType = defaultMatchType;
            AllowOtherMatchType = allowOtherMatchType;
            NumberOfValues = numberOfValues;
        }
    }
}
