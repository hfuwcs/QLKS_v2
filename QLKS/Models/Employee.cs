using System;

namespace QLKS.Models
{
    [Table("NHANVIEN")]
    public class Employee
    {
        [Column("MANV", true, true)]
        public int Id { get; set; }
        [Column("TENNV")]
        public string Name { get; set; }
        [Column("NGSINH")]
        public DateTime DoB { get; set; } // Ngày sinh
        [Column("SDT")]
        public string Phone { get; set; }
        [Column("CCCD")]
        public string UniqueNumber { get; set; } // CCCD
        [Column("DIACHI")]
        public string Address { get; set; }
        [Column("GIOITINH")]
        public string Gender { get; set; }
        [Column("CHUCVU")]
        public string Position { get; set; } // Chức vụ
        [Column("NGAYVAOLAM")]
        public DateTime DoW { get; set; } // Ngày vào làm

        public Employee() { }
    }
}
