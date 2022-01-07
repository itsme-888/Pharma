using Pharma.Attributes;

namespace Pharma.Models
{
    public abstract class BaseEntity
    {
        [PrimaryKey]
        public int Id { get; set; }
    }
}
