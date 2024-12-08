namespace QLKS.Models
{
    [Table("PHONG")]
    public class Room
    {
        [Column("MAPHONG", true, true)]
        public int Id { get; set; }
        [Column("SOPHONG")]
        public string Name { get; set; } // Số phòng
        [Column("TINHTRANG")]
        public string Status { get; set; }
        [Column("MALOAIPHONG")]
        public int RoomType { get; set; }

        public Room() { }
    }
}
