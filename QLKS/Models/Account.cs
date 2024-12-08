using System;

namespace QLKS.Models
{
    [Table("TAIKHOAN")]
    public class Account
    {
        [Column("MANV", true)]
        public int Employee { get; set; }
        [Column("MATKHAU")]
        public string Password { get; set; }
        [Column("VAITRO")]
        public string Role { get; set; }
        [Column("NGAYTAO")]
        public DateTime CreatedAt { get; set; }
        [Column("NGAYDOIMK")]
        public DateTime PasswordUpdatedAt { get; set; }
        [Column("HOATDONG")]
        public bool IsActive { get; set; }

        public Account() { }
    }
}
