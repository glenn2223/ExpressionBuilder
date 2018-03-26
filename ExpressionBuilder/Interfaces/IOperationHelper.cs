using LambdaExpressionBuilder.Common;
using System;
using System.Collections.Generic;

namespace LambdaExpressionBuilder.Interfaces
{
    /// <summary>
    /// Useful methods regarding <seealso cref="Operation"></seealso>.
    /// </summary>
    public interface IOperationHelper
    {
        /// <summary>
        /// Retrieves a list of <see cref="Operation"></see> supported by a type.
        /// </summary>
        /// <param name="type">Type for which supported operations should be retrieved.</param>
        /// <returns></returns>
        List<Operation> SupportedOperations(Type type);

        /// <summary>
        /// Retrieves the exactly number of values acceptable by a specific operation.
        /// </summary>
        /// <param name="operation">See <see cref="Operation" /> for which the number of values acceptable should be verified.</param>
        /// <param name="matchType">See <see cref="Operation" /> for which <see cref="MatchType" /> are allowed.</param>
        /// <returns></returns>
        int NumberOfValuesAcceptable(Operation operation, MatchType matchType);
        
        /// <summary>
        /// Retreives the <see cref="MatchType"/> for the <see cref="Operation"/> provided.
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="matchType"></param>
        /// <returns></returns>
        /// <remarks>If <paramref name="matchType"/> is not <see cref="MatchType.Default"/> then it is returned.</remarks>
        MatchType FetchMatchType(Operation operation, MatchType matchType);
    }
}
