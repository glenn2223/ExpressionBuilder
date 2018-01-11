using System.Collections.Generic;
using ExpressionBuilder.Common;
using ExpressionBuilder.Generics;

namespace ExpressionBuilder.Interfaces
{
	/// <summary>
	/// Defines a filter from which a expression will be built.
	/// </summary>
	public interface IFilter
	{
        /// <summary>
        /// List of statements groups that compose this filter.
        /// </summary>
        List<IFilterStatementOrGroup> Statements { get; }
        /// <summary>
        /// Starts a new group of statements (similar behavior as a parenthesis at the expression).
        /// </summary>
        IFilter OpenGroup { get; }
        /// <summary>
        /// Add a statement, that doesn't need value, to this filter.
        /// </summary>
        /// <param name="propertyId">Property identifier conventionalized by for the Expression Builder.</param>
        /// <param name="operation">Express the interaction between the property and the constant value.</param>
        /// <param name="connector">Establishes how this filter statement will connect to the next one.</param>
        /// <returns>A FilterStatementConnection object that defines how this statement will be connected to the next one.</returns>
        IFilterStatementConnection By(string propertyId, Operation operation, FilterStatementConnector connector = FilterStatementConnector.And);
        /// <summary>
        /// Adds a new <see cref="FilterStatement{TPropertyType}" /> to the <see cref="Filter{TClass}" />.
        /// </summary>
        /// <typeparam name="TPropertyType"></typeparam>
        /// <param name="propertyId">Name of the property that will be filtered.</param>
        /// <param name="operation">Express the interaction between the property and the constant value.</param>
        /// <param name="value">Constant value that will interact with the property.</param>
        /// <param name="connector">Establishes how this filter statement will connect to the next one.</param>
        /// <param name="matchType">Establishes how the IList values will be matched to the property</param>
        /// <returns>A FilterStatementConnection object that defines how this statement will be connected to the next one.</returns>
        IFilterStatementConnection By<TPropertyType>(string propertyId, Operation operation, TPropertyType value, FilterStatementConnector connector = FilterStatementConnector.And, FilterStatementMatchType matchType = FilterStatementMatchType.All);
        /// <summary>
        /// Adds a new <see cref="FilterStatement{TPropertyType}" /> to the <see cref="Filter{TClass}" />.
        /// </summary>
        /// <typeparam name="TPropertyType"></typeparam>
        /// <param name="propertyId">Name of the property that will be filtered.</param>
        /// <param name="operation">Express the interaction between the property and the constant value.</param>
        /// <param name="values">Constant enumeration of values that will interact with the property.</param>
        /// <param name="connector">Establishes how this filter statement will connect to the next one.</param>
        /// <param name="matchType">Establishes how the IList values will be matched to the property</param>
        /// <returns>A FilterStatementConnection object that defines how this statement will be connected to the next one.</returns>
        IFilterStatementConnection By<TPropertyType>(string propertyId, Operation operation, IEnumerable<TPropertyType> values, FilterStatementConnector connector = FilterStatementConnector.And, FilterStatementMatchType matchType = FilterStatementMatchType.All);
        /// <summary>
        /// Starts a new group denoting that every subsequent filter statement should be grouped together (as if using a parenthesis).
        /// </summary>
        void StartGroup();
        /// <summary>
        /// Ends the current group denoting that every subsequent filter statement should be added after the parenthesis group.
        /// </summary>
        void EndGroup();
        /// <summary>
        /// Removes all statements from this filter.
        /// </summary>
        void Clear();
    }
}