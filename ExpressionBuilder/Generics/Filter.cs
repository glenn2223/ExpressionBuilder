using LambdaExpressionBuilder.Builders;
using LambdaExpressionBuilder.Common;
using LambdaExpressionBuilder.Interfaces;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.Schema;
using LambdaExpressionBuilder.Helpers;
using System.Linq;
using System.Text;

namespace LambdaExpressionBuilder.Generics
{
    /// <summary>
    /// Aggregates <see cref="FilterStatement{TPropertyType}" /> and build them into a LINQ expression.
    /// </summary>
    /// <typeparam name="TClass"></typeparam>
    [Serializable]
    public class Filter<TClass> : IFilter, IXmlSerializable where TClass : class
	{
		private List<IFilterStatementOrGroup> _statements;
		private List<IFilterStatementOrGroup> _currentGroup;
        private List<int> _nest;

        /// <summary>
        /// List of <see cref="IFilterStatement" /> groups that will be combined and built into a LINQ expression.
        /// </summary>
        public List<IFilterStatementOrGroup> Statements
        {
            get
            {
                return _statements;
            }
        }
		
        private List<IFilterStatementOrGroup> CurrentStatementGroup
        {
            get
            {
                return _currentGroup;
            }
        }

        /// <summary>
        /// Initiates a new group
        /// </summary>
        public IFilter OpenGroup
        {
            get
            {
                StartGroup();
                return this;
            }
        }

        /// <summary>
        /// Instantiates a new <see cref="Filter{TClass}" />
        /// </summary>
		public Filter()
		{
            _statements = new List<IFilterStatementOrGroup>();
            _currentGroup = _statements;
            _nest = new List<int>();
        }

        /// <summary>
        /// Adds a new <see cref="FilterStatement{TPropertyType}" /> to the <see cref="Filter{TClass}" />.
        /// (To be used by <see cref="Operation" /> that need no values)
        /// </summary>
        /// <param name="propertyId">Property identifier conventionalized by for the Expression Builder.</param>
        /// <param name="operation"></param>
        /// <param name="connector"></param>
        /// <returns></returns>
        public IFilterStatementConnection By(string propertyId, Operation operation, Connector connector = Connector.And)
        {
            return By(propertyId, operation, (string)null, connector);
        }

        /// <summary>
        /// Adds a new <see cref="FilterStatement{TPropertyType}" /> to the <see cref="Filter{TClass}" />.
        /// </summary>
        /// <typeparam name="TPropertyType"></typeparam>
        /// <param name="propertyId">Name of the property that will be filtered.</param>
        /// <param name="operation">Express the interaction between the property and the constant value.</param>
        /// <param name="value">Constant value that will interact with the property.</param>
        /// <param name="connector">Establishes how this filter statement will connect to the next one.</param>
        /// <param name="matchType">Establishes how the IList values will be matched to the property</param>
        /// <returns></returns>
		public IFilterStatementConnection By<TPropertyType>(string propertyId, Operation operation, TPropertyType value, Connector connector = Connector.And, MatchType matchType = MatchType.Default)
		{
			IFilterStatement statement = new FilterStatement<TPropertyType>(propertyId, operation, value, connector, matchType);
            CurrentStatementGroup.Add(statement);
			return new FilterStatementConnection<TClass>(this, statement);
        }

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
        /// <returns></returns>
        public IFilterStatementConnection By<TPropertyType>(string propertyId, Operation operation, IEnumerable<TPropertyType> values, Connector connector = Connector.And, MatchType matchType = MatchType.Default)
        {
            IFilterStatement statement = new FilterStatement<TPropertyType>(propertyId, operation, values, connector, matchType);
            CurrentStatementGroup.Add(statement);
            return new FilterStatementConnection<TClass>(this, statement);
        }

        /// <summary>
        /// Adds a new <see cref="FilterStatement{TPropertyType}" /> to the <see cref="Filter{TClass}" />. Backward compatible
        /// </summary>
        /// <typeparam name="TPropertyType"></typeparam>
        /// <param name="propertyId">Name of the property that will be filtered.</param>
        /// <param name="operation">Express the interaction between the property and the constant value.</param>
        /// <param name="value">Constant value that will interact with the property.</param>
        /// <param name="value2">Constant value that will interact with the property.</param>
        /// <param name="connector">Establishes how this filter statement will connect to the next one.</param>
        /// <param name="matchType">Establishes how the IList values will be matched to the property</param>
        /// <returns></returns>
		public IFilterStatementConnection By<TPropertyType>(string propertyId, Operation operation, TPropertyType value, TPropertyType value2, Connector connector = Connector.And, MatchType matchType = MatchType.Default)
        {
            if (value2 != null)
                return By(propertyId, operation, value, connector, matchType);

            return By(propertyId, operation, new[] { value, value2 }.AsEnumerable(), connector, matchType);
        }

        /// <summary>
        /// Starts a new group denoting that every subsequent filter statement should be grouped together (as if using a parenthesis).
        /// </summary>
        public void StartGroup()
        {
            var group = new FilterGroup();

            _nest.Add(_currentGroup.Count);

            _currentGroup.Add(group);
            _currentGroup = group.Group;
        }

        /// <summary>
        /// Ends the current group denoting that every subsequent filter statement should be added after the parenthesis group.
        /// </summary>
        public void EndGroup()
        {
            if (_nest.Count == 0)
                throw new ArgumentException("There is no group to end");

            List<IFilterStatementOrGroup> current = _statements;

            _nest.RemoveAt(_nest.Count - 1);

            foreach(var index in _nest)
            {
                current = ((IFilterGroup)current[index]).Group;
            }

            _currentGroup = current;
        }

        /// <summary>
        /// Removes all <see cref="FilterStatement{TPropertyType}" />'s and <see cref="FilterGroup"/>'s, leaving the <see cref="Filter{TClass}" /> empty.
        /// </summary>
        public void Clear()
        {
            _statements.Clear();
            _currentGroup = _statements;
        }

        /// <summary>
        /// Implicitly converts a <see cref="Filter{TClass}" /> into a <see cref="Func{TClass, TResult}" />.
        /// </summary>
        /// <param name="filter"></param>
        public static implicit operator Func<TClass, bool>(Filter<TClass> filter)
		{
			return FilterBuilder.GetExpression<TClass>(filter).Compile();
		}

        /// <summary>
        /// Implicitly converts a <see cref="Filter{TClass}" /> into a <see cref="System.Linq.Expressions.Expression{Func{TClass, TResult}}" />.
        /// </summary>
        /// <param name="filter"></param>
        public static implicit operator System.Linq.Expressions.Expression<Func<TClass, bool>>(Filter<TClass> filter)
        {
			return FilterBuilder.GetExpression<TClass>(filter);
        }

        /// <summary>
        /// String representation of <see cref="Filter{TClass}" />.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
		{
            var sB = new StringBuilder();
            var lastConnector = Connector.And;

            for (var index = 0; index < _statements.Count; index++)
            {
                sB.Append(_statements[index].ToString(ref lastConnector));
                if (index < _statements.Count - 1)
                    sB.Append($" {lastConnector.ToString().ToUpper()} ");
            }

			return sB.ToString();
		}

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        ///  Generates an object from its XML representation.
        /// </summary>
        /// <param name="reader">The System.Xml.XmlReader stream from which the object is deserialized.</param>
        public void ReadXml(XmlReader reader)
        {
            while (reader.Read())
            {
                if (reader.IsStartElement("FilterGroup"))
                {
                    StartGroup();
                }

                if (reader.Name.StartsWith("FilterStatementOf"))
                {
                    var type = reader.GetAttribute("Type");
                    var filterType = typeof(FilterStatement<>).MakeGenericType(Type.GetType(type));
                    var serializer = new XmlSerializer(filterType);
                    var statement = (IFilterStatement)serializer.Deserialize(reader);
                    CurrentStatementGroup.Add(statement);
                }

                if (reader.Name == "FilterGroup" && reader.NodeType == XmlNodeType.EndElement)
                {
                    EndGroup();
                }
            }
        }

        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer">The System.Xml.XmlWriter stream to which the object is serialized.</param>
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("Type", typeof(TClass).AssemblyQualifiedName);
            writer.WriteStartElement("Statements");

            foreach (var statementFiltersOrGroups in _statements)
            {
                var serializer = new XmlSerializer(statementFiltersOrGroups.GetType());
                serializer.Serialize(writer, statementFiltersOrGroups);
            }

            writer.WriteEndElement();
        }
    }
}
