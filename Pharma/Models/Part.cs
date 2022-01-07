using Pharma.Attributes;

namespace Pharma.Models
{
    [Table(Name = nameof(Part), Desc = "Партия")]
    class Part: BaseEntity
    {
        [TableRelation(nameof(Product), typeof(Product), "Name")]
        public int ProductId { get; set; }

        [TableRelation(nameof(StoreHouse), typeof(StoreHouse), "Name")]
        public int StoreHouseId { get; set; }

        public int Count { get; set; }
    }
}
