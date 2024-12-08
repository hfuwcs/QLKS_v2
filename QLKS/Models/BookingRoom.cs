using System;

namespace QLKS.Models
{
    [Table("PHIEUDATPHONG")]
    public class BookingRoom
    {
        [Column("MAPHIEUDATPHONG", true, true)]
        public int Id { get; set; }
        [Column("NGAYLAP")]
        public DateTime BookingDate { get; set; }
        [Column("NGAYDEN")]
        public DateTime ArrivedDate { get; set; }
        [Column("NGAYTRADUKIEN")]
        public DateTime ExpectedDate { get; set; }
        [Column("MANV")]
        public int Employee { get; set; }
        [Column("MAKH")]
        public int Customer { get; set; }

        public BookingRoom() { }
    }
}
