using System.Linq.Expressions;

namespace LambdaExpressionBuilder.Interfaces
{
    internal interface IBuilderHelper
    {
        Expression GetMemberExpression(Expression param, string propertyName);
    }
}