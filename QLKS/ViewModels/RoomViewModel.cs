using QLKS.Models;
using QLKS.Models.Extensions;
using System.Collections.Generic;


namespace QLKS.ViewModels
{
    public class RoomViewModel
    {
        Room room;
        RoomType type;

        //public Room Room => room;
        //public RoomType RoomType => type;

        public int Id { get; set; }
        public string Number { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        public int TypeId { get; set; }
        public string Price { get; set; }
        public int MaxPeople { get; set; }

        public RoomViewModel() { }

        public RoomViewModel(Room r, DbContext db)
        {
            room = r;
            type = r.GetRoomType(db);

            Id = r.Id;
            Number = r.Name;
            Status = r.Status;
            Type = type.Name;
            Price = string.Format("{0:C0}",type.Price);
            MaxPeople = type.MaxPeople;
            TypeId = type.Id;
        }

        public static IEnumerable<RoomViewModel> GetRooms(DbContext context)
        {
            foreach (Room room in context.GetTable<Room>())
            {
                yield return new RoomViewModel(room, context);
            }
        }
    }
}
