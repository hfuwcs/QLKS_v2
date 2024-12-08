using System;

namespace QLKS.Models
{
    [Table("PHIEUDICHVU")]
    public class BookingService
    {
        [Column("MAPHIEUDV", true, true)]
        public int Id { get; set; }
        [Column("NGAYLAP")]
        public DateTime BookingDate { get; set; }
        [Column("TONGTIEN")]
        public decimal TotalPrice { get; set; }
        [Column("MAPHIEUDATPHONG")]
        public int BookingRoom { get; set; }
        [Column("MANV")]
        public int Employee { get; set; }

        public BookingService() { }
    }
}
