namespace QLKS.Models
{
    [Table("LOAIPHONG")]
    public class RoomType
    {
        [Column("MALOAIPHONG", true, true)]
        public int Id { get; set; }
        [Column("TENLOAIPHONG")]
        public string Name { get; set; }
        [Column("GIA")]
        public decimal Price { get; set; }
        [Column("SOLUONGNGUOITOIDA")]
        public int MaxPeople { get; set; }

        public RoomType() { }
    }
}
