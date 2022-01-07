using Pharma.Models;
using Pharma.Attributes;

namespace Pharma.Models
{
    [Table(Name = nameof(StoreHouse), Desc = "Склады")]
    public class StoreHouse: BaseEntity
    {
        [TableRelation(nameof(Pharmacy), typeof(Pharmacy), "Name")]
        public int PharmacyId { get; set; }
        public string Name { get; set; }
    }
}
