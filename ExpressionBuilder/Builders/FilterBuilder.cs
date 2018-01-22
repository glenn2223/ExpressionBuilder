using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using LambdaExpressionBuilder.Common;
using LambdaExpressionBuilder.Interfaces;
using LambdaExpressionBuilder.Helpers;

namespace LambdaExpressionBuilder.Builders
{
	internal class FilterBuilder
	{
        readonly BuilderHelper helper;
        
        readonly MethodInfo stringContainsMethod = typeof(string).GetMethod("Contains");
        readonly MethodInfo startsWithMethod = typeof(string).GetMethod("StartsWith", new[] { typeof(string) });
        readonly MethodInfo endsWithMethod = typeof(string).GetMethod("EndsWith", new[] { typeof(string) });

        public readonly Dictionary<Operation, Func<Expression, Expression, Expression>> Expressions;

        internal FilterBuilder(BuilderHelper helper)
		{
            this.helper = helper;

            Expressions = new Dictionary<Operation, Func<Expression, Expression, Expression>>
            {
                { Operation.EqualTo, (member, constant) => Expression.Equal(member, constant) },
                { Operation.NotEqualTo, (member, constant) => Expression.NotEqual(member, constant) },
                { Operation.GreaterThan, (member, constant) => Expression.GreaterThan(member, constant) },
                { Operation.GreaterThanOrEqualTo, (member, constant) => Expression.GreaterThanOrEqual(member, constant) },
                { Operation.LessThan, (member, constant) => Expression.LessThan(member, constant) },
                { Operation.LessThanOrEqualTo, (member, constant) => Expression.LessThanOrEqual(member, constant) },
                { Operation.Contains, (member, constant) => Contains(member, constant) },
                { Operation.StartsWith, (member, constant) => Expression.Call(member, startsWithMethod, constant) },
                { Operation.EndsWith, (member, constant) => Expression.Call(member, endsWithMethod, constant) },
                { Operation.Between, (member, constant) => Between(member, constant) },
                { Operation.IsNull, (member, constant) => Expression.Equal(member, Expression.Constant(null)) },
                { Operation.IsNotNull, (member, constant) => Expression.NotEqual(member, Expression.Constant(null)) },
                { Operation.IsEmpty, (member, constant) => Expression.Equal(member, Expression.Constant(String.Empty)) },
                { Operation.IsNotEmpty, (member, constant) => Expression.NotEqual(member, Expression.Constant(String.Empty)) },
                { Operation.IsNullOrWhiteSpace, (member, constant) => IsNullOrWhiteSpace(member) },
                { Operation.IsNotNullNorWhiteSpace, (member, constant) => IsNotNullNorWhiteSpace(member) },
                { Operation.DoesNotContain, (member, constant) => Expression.Not(Contains(member, constant)) }
            };
        }
		
        /// <summary>
        /// Get the expression to use in a lambda method
        /// </summary>
        /// <typeparam name="T">The original class to reference</typeparam>
        /// <param name="filter">The <see cref="IFilter"/> used to build the expression</param>
        /// <returns></returns>
		public static Expression<Func<T, bool>> GetExpression<T>(IFilter filter) where T : class
        {
            var param = Expression.Parameter(typeof(T), "x");
            Expression expression = null;
            var connector = FilterStatementConnector.And;

            expression = new FilterBuilder(new BuilderHelper()).GetPartialExpression(param, ref connector, filter.Statements);
            
            expression = expression ?? Expression.Constant(true);

            return Expression.Lambda<Func<T, bool>>(expression, param);
        }

        internal Expression GetPartialExpression(ParameterExpression param, ref FilterStatementConnector connector, IEnumerable<IFilterStatementOrGroup> statementGroup)
        {
            Expression expression = null;
            foreach (var statementFilterOrGroup in statementGroup)
            {
                var statementGroupConnector = FilterStatementConnector.And;
                Expression partialExpr = statementFilterOrGroup.Build(param, ref statementGroupConnector, this);

                expression = expression == null ? partialExpr : CombineExpressions(expression, partialExpr, connector);
                connector = statementGroupConnector;
            }

            return expression;
        }

        internal Expression GetPartialExpression(ParameterExpression param, ref FilterStatementConnector connector, IFilterStatement statement)
        {
            Expression expr = null;
            if (IsList(statement))
                expr = ProcessListStatement(param, statement);
            else
                expr = GetExpression(param, statement);

            connector = statement.Connector;

            return expr;
        }

        private static bool IsList(IFilterStatement statement)
        {
            return statement.PropertyId.Contains("[") && statement.PropertyId.Contains("]");
        }

        internal static Expression CombineExpressions(Expression expr1, Expression expr2, FilterStatementConnector connector)
        {
            return connector == FilterStatementConnector.And ? Expression.AndAlso(expr1, expr2) : Expression.OrElse(expr1, expr2);
        }

        private Expression ProcessListStatement(ParameterExpression param, IFilterStatement statement)
        {
            var basePropertyName = statement.PropertyId.Substring(0, statement.PropertyId.IndexOf("["));
            var propertyName = statement.PropertyId.Replace(basePropertyName, "").Replace("[", "").Replace("]", "");

            var type = param.Type.GetProperty(basePropertyName).PropertyType.GetGenericArguments()[0];
            ParameterExpression listItemParam = Expression.Parameter(type, "i");
            var lambda = Expression.Lambda(GetExpression(listItemParam, statement, propertyName), listItemParam);
            var member = helper.GetMemberExpression(param, basePropertyName);
            var enumerableType = typeof(Enumerable);
            var anyInfo = enumerableType.GetMethods(BindingFlags.Static | BindingFlags.Public).First(m => m.Name == "Any" && m.GetParameters().Count() == 2);
            anyInfo = anyInfo.MakeGenericMethod(type);
            return Expression.Call(anyInfo, member, lambda);
        }
        
        private Expression GetExpression(ParameterExpression param, IFilterStatement statement, string propertyName = null)
        {
            Expression resultExpr = null;
            var memberName = propertyName ?? statement.PropertyId;
            Expression member = helper.GetMemberExpression(param, memberName);

            if (Nullable.GetUnderlyingType(member.Type) != null && statement.Value != null)
            {
                resultExpr = Expression.PropertyOrField(member, "HasValue");
                member = Expression.PropertyOrField(member, "Value");
            }

            var safeStringExpression = GetSafeStringExpression(member, statement);
            resultExpr = resultExpr != null ? Expression.AndAlso(resultExpr, safeStringExpression) : safeStringExpression;
            resultExpr = GetSafePropertyMember(param, memberName, resultExpr);

            if ((statement.Operation == Operation.IsNull || statement.Operation == Operation.IsNullOrWhiteSpace) && memberName.Contains("."))
            {
                resultExpr = Expression.OrElse(CheckIfParentIsNull(param, member, memberName), resultExpr);
            }

            return resultExpr;
        }

        private Expression GetSafeStringExpression(Expression member, IFilterStatement statement)
        {
            var operation = statement.Operation;

            if (member.Type != typeof(string))
            {
                return GetSafeExpression(member, statement);
            }

            Expression newMember = member;

            if (operation != Operation.IsNullOrWhiteSpace && operation != Operation.IsNotNullNorWhiteSpace)
            {
                var trimMemberCall = Expression.Call(member, helper.trimMethod);
                newMember = Expression.Call(trimMemberCall, helper.toLowerMethod);
            }

            Expression resultExpr = operation != Operation.IsNull ?
                                    GetSafeExpression(newMember, statement) :
                                    GetSafeExpression(member, statement);

            if (member.Type == typeof(string) && new[] { Operation.IsNull, Operation.IsNullOrWhiteSpace, Operation.IsNotNullNorWhiteSpace }.Contains(operation) == false)
            {
                Expression memberIsNotNull = Expression.NotEqual(member, Expression.Constant(null));
                resultExpr = Expression.AndAlso(memberIsNotNull, resultExpr);
            }

            return resultExpr;
        }
        
        private Expression GetSafeExpression(Expression member, IFilterStatement statement)
        {
            var operation = statement.Operation;
            var matchType = statement.MatchType;
            var constant = GetConstantExpression(member, statement);

            if (operation != Operation.Between && statement.ValueIsList())
            {
                if (operation == Operation.EqualTo && matchType == FilterStatementMatchType.Any)
                    return Expressions[Operation.Contains].Invoke(member, constant);
                else if (operation == Operation.NotEqualTo && matchType == FilterStatementMatchType.All)
                    return Expressions[Operation.DoesNotContain].Invoke(member, constant);
                else
                {
                    var runningExpression = null as Expression;
                    var myList = ValueList(statement);
                    var connector = matchType == FilterStatementMatchType.Any ? FilterStatementConnector.Or : FilterStatementConnector.And;

                    foreach (var item in myList)
                    {
                        Expression loopConstant = Expression.Constant(item);
                        var loopExpression = Expressions[operation].Invoke(member, loopConstant);

                        runningExpression = runningExpression == null ? loopExpression : CombineExpressions(runningExpression, loopExpression, connector);
                    }

                    return runningExpression ?? Expression.Constant(true);
                }
            }

            return Expressions[operation].Invoke(member, constant);
        }
    
        public Expression GetSafePropertyMember(ParameterExpression param, String memberName, Expression expr)
        {
            if (!memberName.Contains("."))
                return expr;

            string parentName = memberName.Substring(0, memberName.IndexOf("."));
            Expression parentMember = helper.GetMemberExpression(param, parentName);
            return Expression.AndAlso(Expression.NotEqual(parentMember, Expression.Constant(null)), expr);
        }

        private Expression CheckIfParentIsNull(Expression param, Expression member, string memberName)
        {
            string parentName = memberName.Substring(0, memberName.IndexOf("."));
            Expression parentMember = helper.GetMemberExpression(param, parentName);
            return Expression.Equal(parentMember, Expression.Constant(null));
        }

        private Expression GetConstantExpression(Expression member, IFilterStatement statement)
        {
            var value = statement.Value;

            if (value == null) return null;

            Expression constant = Expression.Constant(value);

            if (value is string)
            {
                var trimConstantCall = Expression.Call(constant, helper.trimMethod);
                constant = Expression.Call(trimConstantCall, helper.toLowerMethod);
            }
            else if (statement.ValueIsList())
            {
                var myList = ValueList(statement);
                constant = Expression.Constant(statement.Operation == Operation.Between ? new ArrayList(myList).ToArray(statement.GetPropertyType()) : myList);
            }

            return constant;
        }

        private static IList ValueList(IFilterStatement statement)
        {
            var type = statement.GetPropertyType();
            var myList = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(type), statement.Value);

            if (type == typeof(string))
                for (var itemIndex = 0; itemIndex < myList.Count; itemIndex++)
                    myList[itemIndex] = (myList[itemIndex] as string)?.Trim().ToLower();

            return myList;
        }

        #region Operations

        private Expression Contains(Expression member, Expression expression)
        {
            MethodCallExpression contains = null;
            ConstantExpression constant = expression as ConstantExpression;
            if (constant != null && constant.Value is IList && constant.Value.GetType().IsGenericType)
            {
                var type = constant.Value.GetType();
                var containsInfo = type.GetMethod("Contains", new[] { type.GetGenericArguments()[0] });
                contains = Expression.Call(constant, containsInfo, member);
            }

            return contains ?? Expression.Call(member, stringContainsMethod, expression); ;
        }

        private Expression Between(Expression member, Expression constantArray)
        {
            var left = Expressions[Operation.GreaterThanOrEqualTo].Invoke(member, Expression.ArrayIndex(constantArray, Expression.Constant(0)));
            var right = Expressions[Operation.LessThanOrEqualTo].Invoke(member, Expression.ArrayIndex(constantArray, Expression.Constant(1)));

            return CombineExpressions(left, right, FilterStatementConnector.And);
        }

        private Expression IsNullOrWhiteSpace(Expression member)
        {
            Expression exprNull = Expression.Constant(null);
            var trimMemberCall = Expression.Call(member, helper.trimMethod);
            Expression exprEmpty = Expression.Constant(string.Empty);
            return Expression.OrElse(
                                    Expression.Equal(member, exprNull),
                                    Expression.Equal(trimMemberCall, exprEmpty));
        }

        private Expression IsNotNullNorWhiteSpace(Expression member)
        {
            Expression exprNull = Expression.Constant(null);
            var trimMemberCall = Expression.Call(member, helper.trimMethod);
            Expression exprEmpty = Expression.Constant(string.Empty);
            return Expression.AndAlso(
                                    Expression.NotEqual(member, exprNull),
                                    Expression.NotEqual(trimMemberCall, exprEmpty));
        }

        #endregion Operations
    }
}