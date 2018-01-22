namespace LambdaExpressionBuilder.Interfaces
{
    using System.Collections.Generic;

    /// <summary>
    /// A group of items (Either: Filters or Groups).
    /// </summary>
    public abstract class IFilterGroup : IFilterStatementOrGroup
    {
        /// <summary>
        /// A group of Statements or Groups
        /// </summary>
        public List<IFilterStatementOrGroup> Group { get; set; }
    }
}