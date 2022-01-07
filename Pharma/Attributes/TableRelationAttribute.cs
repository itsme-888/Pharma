using System;

namespace Pharma.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class TableRelationAttribute: Attribute
    {
        public string TableName { get; set; }
        public string Description { get; set; }
        public Type RelationToTable { get; set; }

        public TableRelationAttribute(string tableName, Type type, string desc)
        {
            this.TableName = tableName;
            this.RelationToTable = type;
            this.Description = desc;
        }
    }
}
