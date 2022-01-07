using Pharma.Attributes;

namespace Pharma.Models
{
    [Table(Name = nameof(Pharmacy), Desc = "Аптеки")]
    public class Pharmacy: BaseEntity
    {
        public string Name { get; set; }
        public string Adress { get; set; }
        public string Phone { get; set; }
    }
}
