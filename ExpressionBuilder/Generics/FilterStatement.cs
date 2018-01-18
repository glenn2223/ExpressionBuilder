using ExpressionBuilder.Builders;
using ExpressionBuilder.Common;
using ExpressionBuilder.Exceptions;
using ExpressionBuilder.Helpers;
using ExpressionBuilder.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Xml;

namespace ExpressionBuilder.Generics
{
    /// <summary>
	/// Defines how a property should be filtered.
	/// </summary>
    [Serializable]
    public class FilterStatement<TPropertyType> : IFilterStatement
    {
        /// <summary>
        /// Instantiates a new <see cref="FilterStatement{TPropertyType}" />.
        /// </summary>
        /// <param name="propertyId"></param>
        /// <param name="operation"></param>
        /// <param name="values"></param>
        /// <param name="matchType"></param>
        /// <param name="connector"></param>
		public FilterStatement(string propertyId, Operation operation, IEnumerable<TPropertyType> values, FilterStatementConnector connector = FilterStatementConnector.And, FilterStatementMatchType matchType = FilterStatementMatchType.All)
        {
            var constructedListType = typeof(List<>).MakeGenericType(values.FirstOrDefault()?.GetType() ?? typeof(TPropertyType));

            PropertyId = propertyId;
            Connector = connector;
            Operation = operation;
            Value = values != null ? Activator.CreateInstance(constructedListType, values) : null;
            MatchType = matchType;

            Validate();
        }

        /// <summary>
        /// Instantiates a new <see cref="FilterStatement{TPropertyType}" />.
        /// </summary>
        /// <param name="propertyId"></param>
        /// <param name="operation"></param>
        /// <param name="value"></param>
        /// <param name="matchType"></param>
        /// <param name="connector"></param>
        public FilterStatement(string propertyId, Operation operation, TPropertyType value, FilterStatementConnector connector = FilterStatementConnector.And, FilterStatementMatchType matchType = FilterStatementMatchType.All)
		{
            PropertyId = propertyId;
			Connector = connector;
			Operation = operation;
            MatchType = matchType;

            var type = value?.GetType() ?? typeof(TPropertyType);
            var underlyingType = type.IsArray ? type.GetElementType() : type != typeof(string) && typeof(IEnumerable).IsAssignableFrom(type) ? type.GenericTypeArguments[0] : type;

            if (type != typeof(string) && typeof(IEnumerable).IsAssignableFrom(type))
            {
                var constructedListType = typeof(List<>).MakeGenericType(underlyingType);
                Value = value != null ? Activator.CreateInstance(constructedListType, value) : null;
            }
            else
			{
				Value = value;
			}

            Validate();
        }

        /// <summary>
        /// Instantiates a new <see cref="FilterStatement{TPropertyType}" />.
        /// </summary>
        public FilterStatement() { }

        /// <summary>
        /// Validates the FilterStatement regarding the number of provided values and supported operations.
        /// </summary>
        public override void Validate()
        {
            var helper = new OperationHelper();            
            ValidateNumberOfValues(helper);
            ValidateSupportedOperations(helper);
        }

        private void ValidateNumberOfValues(OperationHelper helper)
        {
            var numberOfValues = helper.NumberOfValuesAcceptable(Operation, MatchType);

            if (numberOfValues != -1 && CountValues() != numberOfValues)
            {
                throw new WrongNumberOfValuesException(Operation);
            }
        }

        private int CountValues()
        {
            IEnumerable<string> valueList;

            if (Value == null)
                return 0;

            else if (ValueIsList(out valueList))
                return valueList.Count();

            return 1;
        }

        internal override bool ValueIsList()
        {
            return Value?.GetType() == typeof(List<>).MakeGenericType(GetPropertyType());
        }

        private bool ValueIsList(out IEnumerable<string> ListToString)
        {
            ListToString = null;
            if (ValueIsList())
            {
                List<string> stringList = new List<string>();
                foreach (var x in Value as IEnumerable)
                    stringList.Add(x?.ToString() ?? "NULL");
                ListToString = stringList;
                return true;
            }
            else
                return false;
        }

        internal override Type GetPropertyType()
        {
            var t = Value?.GetType() ?? typeof(TPropertyType);
            return t.IsArray ? t.GetElementType() : t != typeof(string) && typeof(IEnumerable).IsAssignableFrom(t) ? t.GenericTypeArguments[0] : t;
        }

        private void ValidateSupportedOperations(OperationHelper helper)
        {
            List<Operation> supportedOperations = null;
            if (GetPropertyType() == typeof(object))
            {
                //TODO: Issue regarding the TPropertyType that comes from the UI always as 'Object'
                //supportedOperations = helper.GetSupportedOperations(Value.GetType());
                System.Diagnostics.Debug.WriteLine("WARN: Not able to check if the operation is supported or not.");
                return;
            }
            
            supportedOperations = helper.SupportedOperations(GetPropertyType());

            if (!supportedOperations.Contains(Operation))
            {
                throw new UnsupportedOperationException(Operation, GetPropertyType().Name);
            }
        }

        /// <summary>
        /// Builds the <see cref="Expression"/>
        /// </summary>
        /// <param name="param"></param>
        /// <param name="connector"></param>
        /// <param name="builder"></param>
        /// <returns></returns>
        internal override Expression Build(ParameterExpression param, ref FilterStatementConnector connector, FilterBuilder builder)
        {
            return builder.GetPartialExpression(param, ref connector, this);
        }

        /// <summary>
        /// String representation of <see cref="FilterStatement{TPropertyType}" />.
        /// </summary>
        /// <returns></returns>
		public override string ToString()
        {
            var value = Value;
            IEnumerable<string> valueList;
            if (ValueIsList(out valueList))
            {
                if (Operation == Operation.Between)
                    value = string.Join(" AND ", valueList);
                else
                    value = MatchType + " ( \"" + string.Join("\", \"", valueList) + "\" )";
            }

            var operationHelper = new OperationHelper();

            switch (operationHelper.NumberOfValuesAcceptable(Operation, MatchType))
            {
                case 0:
                    return string.Format("{0} {1}", PropertyId, Operation);
                default:
                    return string.Format("{0} {1} {2}", PropertyId, Operation, value);
            }
        }

        /// <summary>
        /// String representation of <see cref="FilterStatement{TPropertyType}" />.
        /// </summary>
        /// <returns></returns>
		public override string ToString(ref FilterStatementConnector LastConnector)
        {
            LastConnector = Connector;

            return ToString();
        }

        /// <summary>
        ///  Generates an object from its XML representation.
        /// </summary>
        /// <param name="reader">The System.Xml.XmlReader stream from which the object is deserialized.</param>
        public override void ReadXml(XmlReader reader)
        {
            var type = GetPropertyType();

            reader.Read();
            PropertyId = reader.ReadElementContentAsString();
            Operation = (Operation)Enum.Parse(typeof(Operation), reader.ReadElementContentAsString());
            if (reader.IsStartElement("Values"))
            {
                reader.Read();

                var valueList = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(type));

                while (reader.NodeType != XmlNodeType.EndElement && reader.Name != "Values")
                    valueList.Add(Convert.ChangeType(reader.ReadElementContentAsString(), type));

                Value = valueList;

                reader.ReadEndElement();
            }
            else
            {
                if (type.IsEnum)
                {
                    Value = Enum.Parse(type, reader.ReadElementContentAsString());
                }
                else
                {
                    if (reader.IsEmptyElement && reader.GetAttribute("NULLED") == true.ToString())
                    {
                        Value = null;
                        reader.Read();
                    }
                    else
                        Value = Convert.ChangeType(reader.ReadElementContentAsString(), type);
                }
            }
            Connector = (FilterStatementConnector)Enum.Parse(typeof(FilterStatementConnector), reader.ReadElementContentAsString());
            MatchType = (FilterStatementMatchType)Enum.Parse(typeof(FilterStatementMatchType), reader.ReadElementContentAsString());
        }

        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer">The System.Xml.XmlWriter stream to which the object is serialized.</param>
        public override void WriteXml(XmlWriter writer)
        {
            IEnumerable<string> valueList;
            
            writer.WriteAttributeString("Type", (Value?.GetType() ?? typeof(TPropertyType)).AssemblyQualifiedName);
            writer.WriteElementString("PropertyId", PropertyId);
            writer.WriteElementString("Operation", Operation.ToString("d"));
            if (Value != null)
            {
                if (ValueIsList(out valueList))
                {
                    writer.WriteStartElement("Values");
                    valueList.ToList().ForEach(x => writer.WriteElementString("Value", x));
                    writer.WriteEndElement();

                }
                else
                    writer.WriteElementString("Value", Value.ToString());
            }
            else
            {
                writer.WriteStartElement("Value");
                writer.WriteAttributeString("NULLED", true.ToString());
                writer.WriteEndElement();
            }
            writer.WriteElementString("Connector", Connector.ToString("d"));
            writer.WriteElementString("MatchType", MatchType.ToString("d"));
        }
    }
}
