using System;

namespace Pharma.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TableAttribute: Attribute
    {
        public string Name { get; set; }
        public string Desc { get; set; }

        public TableAttribute()
        {

        }
    }
}
