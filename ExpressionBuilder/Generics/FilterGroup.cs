using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using LambdaExpressionBuilder.Builders;
using LambdaExpressionBuilder.Common;
using LambdaExpressionBuilder.Interfaces;

namespace LambdaExpressionBuilder.Generics
{
    /// <summary>
    /// A group of FilterStatements
    /// </summary>
    [Serializable]
    public class FilterGroup : IFilterGroup
    {
        /// <summary>
        /// Create a new group of filters or groups
        /// </summary>
        public FilterGroup()
        {
            Group = new List<IFilterStatementOrGroup>();
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
            return builder.GetPartialExpression(param, ref connector, Group);
        }

        /// <summary>
        /// String representation of <see cref="FilterGroup" />.
        /// </summary>
        /// <returns></returns>
		public override string ToString()
        {
            var forgetMe = FilterStatementConnector.And;
            return ToString(ref forgetMe);
        }

        /// <summary>
        /// String representation of <see cref="FilterGroup" />.
        /// </summary>
        /// <returns></returns>
		public override string ToString(ref FilterStatementConnector LastConnector)
        {
            var sb = new StringBuilder("( ");

            for (var index = 0; index < Group.Count; index++)
            {
                sb.Append(Group[index].ToString(ref LastConnector));
                if (index < Group.Count - 1)
                    sb.Append($" {LastConnector.ToString().ToUpper()} ");
            }

            return sb.ToString() + " )";
        }

        /// <summary>
        ///  Generates an object from its XML representation.
        /// </summary>
        /// <param name="reader">The System.Xml.XmlReader stream from which the object is deserialized.</param>
        public override void ReadXml(XmlReader reader)
        {
            var serializer = new XmlSerializer(typeof(FilterGroup));
            var statement = (IFilterGroup)serializer.Deserialize(reader);
            Group = statement.Group;
        }

        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer">The System.Xml.XmlWriter stream to which the object is serialized.</param>
        public override void WriteXml(XmlWriter writer)
        {
            foreach (var statementFiltersOrGroups in Group)
            {
                var serializer = new XmlSerializer(statementFiltersOrGroups.GetType());
                serializer.Serialize(writer, statementFiltersOrGroups);
            }
        }
    }
}