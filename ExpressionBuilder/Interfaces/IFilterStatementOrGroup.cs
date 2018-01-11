namespace ExpressionBuilder.Interfaces
{
    using ExpressionBuilder.Builders;
    using ExpressionBuilder.Common;
    using System.Linq.Expressions;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    /// <summary>
    /// A Filter Statement Or Group.
    /// </summary>
    public abstract class IFilterStatementOrGroup : IXmlSerializable
    {
        /// <summary>
        /// Builds the <see cref="Expression"/>
        /// </summary>
        /// <param name="param"></param>
        /// <param name="connector"></param>
        /// <param name="builder"></param>
        /// <returns></returns>
        internal abstract Expression Build(ParameterExpression param, ref FilterStatementConnector connector, FilterBuilder builder);
        /// <summary>
        /// String version of the filter Statement or Group
        /// </summary>
        /// <param name="LastConnector"></param>
        /// <returns></returns>
		public abstract string ToString(ref FilterStatementConnector LastConnector);

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
        public abstract void ReadXml(XmlReader reader);

        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer">The System.Xml.XmlWriter stream to which the object is serialized.</param>
        public abstract void WriteXml(XmlWriter writer);
    }
}