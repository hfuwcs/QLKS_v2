using System;

namespace QLKS.Models
{
    [Table("THANHTOAN")]
    public class Payment
    {
        [Column("MATHANHTOAN", true, true)]
        public int Id { get; set; }
        [Column("MAHD")]
        public int Invoice { get; set; }
        [Column("PTTHANHTOAN")]
        public string PaymentMethod { get; set; }
        [Column("TRANGTHAI")]
        public string Status { get; set; }
        [Column("HANTHANHTOAN")]
        public DateTime PaymentTerm { get; set; }
        [Column("NGAYTHANHTOAN")]
        public DateTime PaymentDate { get; set; }

        public Payment() { }
    }
}
