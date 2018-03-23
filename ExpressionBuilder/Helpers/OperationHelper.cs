using LambdaExpressionBuilder.Attributes;
using LambdaExpressionBuilder.Common;
using LambdaExpressionBuilder.Configuration;
using LambdaExpressionBuilder.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace LambdaExpressionBuilder.Helpers
{
    /// <summary>
    /// Useful methods regarding <seealso cref="Operation"></seealso>.
    /// </summary>
    public class OperationHelper : IOperationHelper
    {
        readonly Dictionary<TypeGroup, HashSet<Type>> TypeGroups;

        /// <summary>
        /// Instantiates a new OperationHelper.
        /// </summary>
        public OperationHelper()
        {
            TypeGroups = new Dictionary<TypeGroup, HashSet<Type>>
            {
                { TypeGroup.Text, new HashSet<Type> { typeof(string), typeof(char) } },
                { TypeGroup.Number, new HashSet<Type> { typeof(int), typeof(uint), typeof(byte), typeof(sbyte), typeof(short), typeof(ushort), typeof(long), typeof(ulong), typeof(Single), typeof(double), typeof(decimal) } },
                { TypeGroup.Boolean, new HashSet<Type> { typeof(bool) } },
                { TypeGroup.Date, new HashSet<Type> { typeof(DateTime) } },
                { TypeGroup.Nullable, new HashSet<Type> { typeof(Nullable<>) } }
            };
        }

        /// <summary>
        /// Retrieves a list of <see cref="Operation"></see> supported by a type.
        /// </summary>
        /// <param name="type">Type for which supported operations should be retrieved.</param>
        /// <returns></returns>
        public List<Operation> SupportedOperations(Type type)
        {
            var supportedOperations = ExtractSupportedOperationsFromAttribute(type);

            var underlyingNullableType = Nullable.GetUnderlyingType(type);
            if(underlyingNullableType != null)
            {
                var underlyingNullableTypeOperations = SupportedOperations(underlyingNullableType);
                supportedOperations.AddRange(underlyingNullableTypeOperations);
            }

            return supportedOperations;
        }

        private void GetCustomSupportedTypes()
        {
            var configSection = ConfigurationManager.GetSection(ExpressionBuilderConfig.SectionName) as ExpressionBuilderConfig;
            if (configSection == null)
            {
                return;
            }

            foreach (ExpressionBuilderConfig.SupportedTypeElement supportedType in configSection.SupportedTypes)
            {
                Type type = Type.GetType(supportedType.Type, false, true);
                if (type != null)
                {
                    TypeGroups[supportedType.TypeGroup].Add(type);
                }
            }
        }

        private List<Operation> ExtractSupportedOperationsFromAttribute(Type type)
        {
            var typeName = type.Name;
            if (type.IsArray)
            {
                typeName = type.GetElementType().Name;
            }

            GetCustomSupportedTypes();
            var typeGroup = TypeGroups.FirstOrDefault(i => i.Value.Any(v => v.Name == typeName)).Key;
            var fieldInfo = typeGroup.GetType().GetField(typeGroup.ToString());
            var attrs = fieldInfo.GetCustomAttributes(false);
            var attr = attrs.FirstOrDefault(a => a is SupportedOperationsAttribute) as SupportedOperationsAttribute;
            return (attr ).SupportedOperations;
        }

        /// <summary>
        /// Retrieves the exactly number of values acceptable by a specific operation.
        /// </summary>
        /// <param name="operation">See <see cref="Operation" /> for which the number of values acceptable should be verified.</param>
        /// <param name="matchType">See <see cref="Operation" /> for which <see cref="MatchType" /> are allowed.</param>
        /// <returns></returns>
        public int NumberOfValuesAcceptable(Operation operation, MatchType matchType)
        {
            var attr = FetchAttribute(operation);

            if (matchType == MatchType.Default)
                matchType = attr.DefaultMatchType;
            
            return attr.NumberOfValues != 0 && (matchType == attr.DefaultMatchType || attr.AllowOtherMatchType) ? -1 : attr.NumberOfValues;
        }

        /// <summary>
        /// Retrieves the <see cref="MatchType"/>'s acceptable by a specific operation.
        /// </summary>
        /// <param name="operation">See <see cref="Operation" /> for which the number of values acceptable should be verified.</param>
        /// <returns></returns>
        public List<MatchType> AllowedMatchTypes(Operation operation)
        {
            var attr = FetchAttribute(operation);
            var allowedTypes = new List<MatchType>();

            if (attr != null && attr.NumberOfValues != 0)
            {
                allowedTypes.Add(attr.DefaultMatchType);
                if (attr.AllowOtherMatchType == true)
                    if (attr.DefaultMatchType == MatchType.All)
                        allowedTypes.Add(MatchType.Any);
                    else
                        allowedTypes.Add(MatchType.All);
            }

            return allowedTypes;
        }

        /// <summary>
        /// Retrieves the exactly number of values acceptable by a specific operation.
        /// </summary>
        /// <param name="operation">See <see cref="Operation" /> for which the number of values acceptable should be verified.</param>
        /// <returns></returns>
        internal int NumberOfValuesAcceptable(Operation operation)
        {
            var attr = FetchAttribute(operation);

            return (attr as OperationSettingsAttribute).NumberOfValues;
        }

        /// <summary>
        /// Retrieves the <see cref="OperationSettingsAttribute"/> from an operation.
        /// </summary>
        /// <param name="operation">See <see cref="Operation" /> for which the number of values acceptable should be verified.</param>
        /// <returns></returns>
        private static OperationSettingsAttribute FetchAttribute(Operation operation)
        {
            var fieldInfo = operation.GetType().GetField(operation.ToString());
            var attrs = fieldInfo.GetCustomAttributes(false);
            return attrs.FirstOrDefault(a => a is OperationSettingsAttribute) as OperationSettingsAttribute;
        }
    }
}
