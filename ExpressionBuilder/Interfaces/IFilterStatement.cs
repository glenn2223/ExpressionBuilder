using ExpressionBuilder.Common;
using System;
using System.Collections.Generic;

namespace ExpressionBuilder.Interfaces
{
	/// <summary>
	/// Defines how a property should be filtered.
	/// </summary>
	public abstract class IFilterStatement : IFilterStatementOrGroup
    {
		/// <summary>
		/// Establishes how this filter statement will connect to the next one. 
		/// </summary>
		public FilterStatementConnector Connector { get; set; }
        /// <summary>
        /// Property identifier conventionalized by for the Expression Builder.
        /// </summary>
        public string PropertyId { get; set; }
        /// <summary>
        /// Express the interaction between the property and the constant value defined in this filter statement.
        /// </summary>
        public Operation Operation { get; set; }
        /// <summary>
        /// Constant value that will interact with the property defined in this filter statement.
        /// </summary>
        public object Value { get; set; }
        /// <summary>
        /// Establish how the values list will be matched to the property.
        /// </summary>
        public FilterStatementMatchType MatchType { get; set; }

        /// <summary>
        /// Get the PropertyType accosiated to this member
        /// </summary>
        /// <returns></returns>
        internal abstract Type GetPropertyType();
        /// <summary>
        /// Validates the FilterStatement regarding the number of provided values and supported operations.
        /// </summary>
        public abstract void Validate();
        /// <summary>
        /// Checks if the Value is a list of objects
        /// </summary>
        /// <returns></returns>
        internal abstract bool ValueIsList();
    }
}