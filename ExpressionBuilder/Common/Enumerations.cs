using ExpressionBuilder.Attributes;
using System;

namespace ExpressionBuilder.Common
{
    /// <summary>
    /// Defines how the filter statements will be connected to each other.
    /// </summary>
    public enum FilterStatementConnector
    {
        /// <summary>
        /// Determines that both the current AND the next filter statement needs to be satisfied.
        /// </summary>
        And,
        /// <summary>
        /// Determines that the current OR the next filter statement needs to be satisfied.
        /// </summary>
        Or
    }

    /// <summary>
    /// Defines how the filter statements with IList values will be searched.
    /// </summary>
    public enum FilterStatementMatchType
    {
        /// <summary>
        /// Determines that there must be a match for ALL values.
        /// </summary>
        All,
        /// <summary>
        /// Determines that there must be a match for ANY value.
        /// </summary>
        Any
    }

    /// <summary>
    /// Defines the operations supported by the <seealso cref="Builders.FilterBuilder" />.
    /// </summary>
    public enum Operation
    {
        /// <summary>
        /// Targets an object in which the property's value is equal to the provided value.
        /// </summary>
        /// <remarks>Accepts one value.</remarks>
        [NumberOfValues(1, false, true)]
        EqualTo,

        /// <summary>
        /// Targets an object in which the property's value contains part of the provided value.
        /// </summary>
        /// <remarks>Accepts one value.</remarks>
        [NumberOfValues(1, true, true)]
        Contains,

        /// <summary>
        /// Targets an object in which the property's value starts with the provided value.
        /// </summary>
        /// <remarks>Accepts one value.</remarks>
        [NumberOfValues(1, false, true)]
        StartsWith,

        /// <summary>
        /// Targets an object in which the property's value ends with the provided value.
        /// </summary>
        /// <remarks>Accepts one value.</remarks>
        [NumberOfValues(1, false, true)]
        EndsWith,

        /// <summary>
        /// Targets an object in which the property's value is not equal to the provided value.
        /// </summary>
        /// <remarks>Accepts one value.</remarks>
        [NumberOfValues(1, true, false)]
        NotEqualTo,

        /// <summary>
        /// Targets an object in which the property's value is greater than the provided value.
        /// </summary>
        /// <remarks>Accepts one value.</remarks>
        [NumberOfValues(1, true, true)]
        GreaterThan,

        /// <summary>
        /// Targets an object in which the property's value is greater than or equal to the provided value.
        /// </summary>
        /// <remarks>Accepts one value.</remarks>
        [NumberOfValues(1, true, true)]
        GreaterThanOrEqualTo,

        /// <summary>
        /// Targets an object in which the property's value is less than the provided value.
        /// </summary>
        /// <remarks>Accepts one value.</remarks>
        [NumberOfValues(1, true, true)]
        LessThan,

        /// <summary>
        /// Targets an object in which the property's value is less than or equal to the provided value.
        /// </summary>
        /// <remarks>Accepts one value.</remarks>
        [NumberOfValues(1, true, true)]
        LessThanOrEqualTo,

        /// <summary>
        /// Targets an object in which the property's value is between the two provided values (greater than or equal to the first one and less then or equal to the second one).
        /// </summary>
        /// <remarks>Accepts two values.</remarks>
        [NumberOfValues(2)]
        Between,

        /// <summary>
        /// Targets an object in which the property's value is null.
        /// </summary>
        /// <remarks>Accepts no value at all.</remarks>
        [NumberOfValues(0)]
        IsNull,

        /// <summary>
        /// Targets an object in which the property's value is an empty string.
        /// </summary>
        /// <remarks>Accepts no value at all.</remarks>
        [NumberOfValues(0)]
        IsEmpty,

        /// <summary>
        /// Targets an object in which the property's value is null or an empty string.
        /// </summary>
        /// <remarks>Accepts no value at all.</remarks>
        [NumberOfValues(0)]
        IsNullOrWhiteSpace,

        /// <summary>
        /// Targets an object in which the property's value is not null.
        /// </summary>
        /// <remarks>Accepts no value at all.</remarks>
        [NumberOfValues(0)]
        IsNotNull,

        /// <summary>
        /// Targets an object in which the property's value is not an empty string.
        /// </summary>
        /// <remarks>Accepts no value at all.</remarks>
        [NumberOfValues(0)]
        IsNotEmpty,

        /// <summary>
        /// Targets an object in which the property's value is neither null nor an empty string.
        /// </summary>
        /// <remarks>Accepts no value at all.</remarks>
        [NumberOfValues(0)]
        IsNotNullNorWhiteSpace,

        /// <summary>
        /// Targets an object in which the property's value does not contain part of the provided value.
        /// </summary>
        /// <remarks>Accepts one value.</remarks>
        [NumberOfValues(1, true, false)]
        DoesNotContain
    }
    
    /// <summary>
    /// Groups types into simple groups and map the supported operations to each group.
    /// </summary>
    public enum TypeGroup
    {
        /// <summary>
        /// Default type group, only supports EqualTo and NotEqualTo.
        /// </summary>
        [SupportedOperations(Operation.EqualTo, Operation.NotEqualTo)]
        Default,

        /// <summary>
        /// Supports all text related operations.
        /// </summary>
        [SupportedOperations(Operation.Contains, Operation.DoesNotContain, Operation.EndsWith, Operation.EqualTo,
                             Operation.IsEmpty, Operation.IsNotEmpty, Operation.IsNotNull, Operation.IsNotNullNorWhiteSpace,
                             Operation.IsNull, Operation.IsNullOrWhiteSpace, Operation.NotEqualTo, Operation.StartsWith)]
        Text,

        /// <summary>
        /// Supports all numeric related operations.
        /// </summary>
        [SupportedOperations(Operation.Between, Operation.EqualTo, Operation.GreaterThan, Operation.GreaterThanOrEqualTo,
                             Operation.LessThan, Operation.LessThanOrEqualTo, Operation.NotEqualTo)]
        Number,

        /// <summary>
        /// Supports boolean related operations.
        /// </summary>
        [SupportedOperations(Operation.EqualTo, Operation.NotEqualTo)]
        Boolean,

        /// <summary>
        /// Supports all date related operations.
        /// </summary>
        [SupportedOperations(Operation.Between, Operation.EqualTo, Operation.GreaterThan, Operation.GreaterThanOrEqualTo,
                             Operation.LessThan, Operation.LessThanOrEqualTo, Operation.NotEqualTo)]
        Date,

        /// <summary>
        /// Supports nullable related operations.
        /// </summary>
        [SupportedOperations(Operation.IsNotNull, Operation.IsNull)]
        Nullable
    }
}
