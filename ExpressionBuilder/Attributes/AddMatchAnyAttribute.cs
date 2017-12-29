using System;

namespace ExpressionBuilder.Attributes
{
    public class AddMatchAny : Attribute
    {
    }
}


/*
/// OLD NOTE

/// Irrelivant: [WAIT, MAYBE NOT => see last line]
/// Now checking if <see cref="ExpressionBuilder.Interfaces.IFilterStatement.Value"/> is an array
/// Might use this as a way to add the operation while building the winform [STAND BY]


using System;
using System.ComponentModel;

namespace ExpressionBuilder.Attributes
{
    public class AddMatchAnyAttribute : Attribute
    {
        [DefaultValue(false)]
        public bool AddMatchAny { get; private set; }

        /// <summary>
        /// Defines weather the <see cref="Common.Operation.MatchAny"/> attribute is allowed for the specified field or property.
        /// </summary>
        /// <param name="allow">Is MatchAny allowed.</param>
        public AddMatchAnyAttribute(bool allow = false)
        {
            AddMatchAny = allow;
        }
    }
}
*/
