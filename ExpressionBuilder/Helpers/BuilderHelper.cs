﻿using LambdaExpressionBuilder.Interfaces;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace LambdaExpressionBuilder.Helpers
{
	internal class BuilderHelper : IBuilderHelper
	{
        public readonly MethodInfo trimMethod = typeof(string).GetMethod("Trim", new Type[0]);
        public readonly MethodInfo toLowerMethod = typeof(string).GetMethod("ToLower", new Type[0]);

        public Expression GetMemberExpression(Expression param, string propertyName)
        {
        	if (propertyName.Contains("."))
        	{
        		int index = propertyName.IndexOf(".");
        		var subParam = Expression.PropertyOrField(param, propertyName.Substring(0, index));
        		return GetMemberExpression(subParam, propertyName.Substring(index + 1));
        	}
            
            return Expression.PropertyOrField(param, propertyName);
        }
	}
}
