using System;

namespace QLKS.Models
{
    [Table("PHIEUNHANPHONG")]
    public class ReceivingRoom
    {
        [Column("MAPHIEUNHANPHONG", true, true)]
        public int Id { get; set; }
        [Column("MAPHIEUDATPHONG")]
        public int BookingRoom { get; set; }
        [Column("NGAYNHAN")]
        public DateTime ReceivingDate { get; set; }
        [Column("MANV")]
        public int Employee { get; set; }

        public ReceivingRoom() { }
    }
}
