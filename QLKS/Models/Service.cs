namespace QLKS.Models
{
    [Table("DICHVU")]
    public class Service
    {
        [Column("MADV", true, true)]
        public int Id { get; set; }
        [Column("TENDV")]
        public string Name { get; set; }
        [Column("DONGIA")]
        public decimal Price { get; set; }

        public Service() { }
    }
}
