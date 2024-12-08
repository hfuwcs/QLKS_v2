using QLKS.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QLKS.ViewModels
{
    public class LoadingListRoom
    {
        BookingRoom bookingRoom;
        BookingRoomDetail bookingRoomDetail;
        Customer customer;
        Room room;

        public BookingRoom BookingRoom { get => bookingRoom; set => bookingRoom = value; }
        public Customer Customer { get => customer; set => customer = value; }
        public Room Room { get => room; set => room = value; }
        public BookingRoomDetail BookingRoomDetail { get => bookingRoomDetail; set => bookingRoomDetail = value; }
        public string Number { get; set; }
        public string Status { get; set; }
        public string CustomerName { get; set; }
        public string ArrivedDate { get; set; }
        public string ExpectedDate { get; set; }
        public int Id { get; set; }
        public int IdRoom { get; set; }


        public LoadingListRoom()
        {
        }
        public LoadingListRoom(Room room, DbContext db, DateTime start, DateTime end)
        {
            this.room = room;
            string status = CheckRoomStatus(db, room.Id, start, end);
            if (status == "Đã đặt")
            {
                List<BookingRoomDetail> bookings = db.GetTable<BookingRoomDetail>(t => t.Room == room.Id).ToList();
                foreach (BookingRoomDetail booking in bookings)
                {
                    BookingRoom booking1 = db.GetTable<BookingRoom>(p => p.Id == booking.BookingRoom).First();
                    if (!(booking1.ExpectedDate <= start.Date || booking1.ArrivedDate >= end.Date))
                    {
                        bookingRoomDetail = booking;
                        break;
                    }
                }
            }
            else if (status == "Đã nhận")
            {
                List<BookingRoomDetail> bookings = db.GetTable<BookingRoomDetail>(t => t.Room == room.Id).ToList();
                foreach (BookingRoomDetail booking in bookings)
                {
                    BookingRoom booking1 = db.GetTable<BookingRoom>(p => p.Id == booking.BookingRoom).First();
                    if (!(booking1.ExpectedDate <= start.Date || booking1.ArrivedDate >= end.Date))
                    {
                        ReceivingRoom receiving = db.GetTable<ReceivingRoom>(p => p.BookingRoom == booking1.Id).FirstOrDefault();
                        if (receiving == null)
                            continue;
                        bookingRoomDetail = booking;
                        break;
                    }
                }
            }
            if (bookingRoomDetail != null)
            {
                Id = bookingRoomDetail.BookingRoom;
                bookingRoom = db.GetTable<BookingRoom>(t => t.Id == Id).First();
                customer = db.GetTable<Customer>(t => t.Id == bookingRoom.Customer).First();
                CustomerName = customer.Name;
                ArrivedDate = bookingRoom.ArrivedDate.ToString("dd/MM/yyyy");
                ExpectedDate = bookingRoom.ExpectedDate.ToString("dd/MM/yyyy");
                IdRoom = room.Id;
            }
            Number = room.Name;
            Status = room.Status;

        }
        public static IEnumerable<LoadingListRoom> GetRooms(DbContext context, DateTime start, DateTime end)
        {
            foreach (Room room in context.GetTable<Room>())
            {
                yield return new LoadingListRoom(room, context, start, end);
            }
        }
        string CheckRoomStatus(DbContext db, int roomId, DateTime dayStart, DateTime dayEnd)
        {
            List<BookingRoom> bookings = db.GetTable<BookingRoom>(p => !(p.ExpectedDate <= dayStart.Date || p.ArrivedDate >= dayEnd.Date)).ToList();
            foreach (BookingRoom booking in bookings)
            {
                Invoice invoice = db.GetTable<Invoice>(p => p.BookingRoom == booking.Id).FirstOrDefault();
                if (invoice != null)
                {
                    foreach (BookingRoomDetail room in db.GetTable<BookingRoomDetail>(p => p.BookingRoom == booking.Id))
                    {
                        if (roomId == room.Room)
                        {
                            return "Phòng trống";
                        }
                    }
                }
                else
                {
                    ReceivingRoom receiving = db.GetTable<ReceivingRoom>(p => p.BookingRoom == booking.Id).FirstOrDefault();
                    if (receiving != null)
                    {
                        foreach (BookingRoomDetail room in db.GetTable<BookingRoomDetail>(p => p.BookingRoom == booking.Id))
                        {
                            if (roomId == room.Room)
                            {
                                return "Đã nhận";
                            }
                        }
                    }
                    else
                    {
                        foreach (BookingRoomDetail room in db.GetTable<BookingRoomDetail>(p => p.BookingRoom == booking.Id))
                        {
                            if (roomId == room.Room)
                            {
                                return "Đã đặt";
                            }
                        }
                    }
                }
            }
            return "Phòng trống";
        }

    }
}
