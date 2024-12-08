using QLKS.Models;
using System.Collections.Generic;
using System.Linq;

namespace QLKS.ViewModels
{
    public class ReservationRoomStatus
    {
        Room room;
        RoomType roomType;

        public Room Room { get => room; set => room = value; }
        public RoomType RoomType { get => roomType; set => roomType = value; }
        public int Id { get; set; }
        public string Number { get; set; }
        public string Status { get; set; }
        public int RoomTypeId { get; set; }
        public string NameRoomType { get; set; }
        public decimal Price { get; set; }
        public int MaxPeople { get; set; }

        public ReservationRoomStatus()
        {
        }
        public ReservationRoomStatus(Room room, DbContext db)
        {
            Id = room.Id;
            Number = room.Name;
            Status = room.Status;
            RoomTypeId = room.RoomType;
            roomType = db.GetTable<RoomType>(t => t.Id == RoomTypeId).First();
            NameRoomType = roomType.Name;
            Price = roomType.Price;
            MaxPeople = roomType.MaxPeople;
        }
        public static IEnumerable<ReservationRoomStatus> GetRooms(DbContext context)
        {
            foreach (Room room in context.GetTable<Room>())
            {
                yield return new ReservationRoomStatus(room, context);
            }
        }
    }
}
