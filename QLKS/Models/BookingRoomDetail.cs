using System;

namespace QLKS.Models
{
    [Table("CHITIETPHIEUDAT")]
    public class BookingRoomDetail
    {
        [Column("MAPHIEUDATPHONG", true)]
        public int BookingRoom { get; set; }
        [Column("MAPHONG", true)]
        public int Room { get; set; }
        [Column("NGAYTRAPHONG")]
        public DateTime CheckoutDate { get; set; }

        public BookingRoomDetail() { }
    }
}
