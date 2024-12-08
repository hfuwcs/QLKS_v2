using System.Linq;

namespace QLKS.Models.Extensions
{
    public static class RoomExts
    {
        public static RoomType GetRoomType(this Room room, DbContext context)
        {
            RoomType roomType = context.GetTable<RoomType>(x => x.Id == room.RoomType).FirstOrDefault();
            return roomType;
        }
    }
}
