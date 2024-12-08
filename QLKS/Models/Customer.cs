using System;

namespace QLKS.Models
{
    [Table("KHACHHANG")]
    public class Customer
    {
        [Column("MAKH", true, true)]
        public int Id { get; set; }
        [Column("TENKH")]
        public string Name { get; set; }
        [Column("NGSINH")]
        public DateTime DoB { get; set; } // Ngày sinh
        [Column("GIOITINH")]
        public string Gender { get; set; }
        [Column("SDT")]
        public string Phone { get; set; }
        [Column("MADD")]
        public string UniqueNumber { get; set; } // CCCD | Thị thực
        [Column("QUOCTICH")]
        public string Country { get; set; }

        public Customer() { }
    }
}
