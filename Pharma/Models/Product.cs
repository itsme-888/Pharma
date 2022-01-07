using Pharma.Attributes;

namespace Pharma.Models
{
    [Table(Name = nameof(Product), Desc = "Товар")]
    public class Product: BaseEntity
    {
        public string Name { get; set; }
    }
}
