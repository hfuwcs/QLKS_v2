using QLKS.Models;
using System.Collections.Generic;
using System.Linq;

namespace QLKS.ViewModels
{
    public class BookingRoomViewModel
    {
        BookingRoom booking;
        BookingRoomDetail detail;
        Room room;
        Employee employee;
        Customer customer;

        public BookingRoom BookingRoom => booking;
        public BookingRoomDetail BookingRoomDetail => detail;
        public Room Room => room;
        public Employee Employee => employee;
        public Customer Customer => customer;

        public int Id { get; set; }
        public string BookingDate { get; set; }
        public string ArrivedDate { get; set; }
        public string ExpectedDate { get; set; }
        public string CheckoutDate { get; set; }
        public string EmployeeUnique { get; set; } // Tên nhân viên
        public string CustomerUnique { get; set; } // CCCD khách hàng
        public string RoomNumber { get; set; }

        public BookingRoomViewModel() { }

        public BookingRoomViewModel(BookingRoom booking, DbContext db, string dateFormat = "")
        {
            this.booking = booking;
            employee = db.GetTable<Employee>(x => x.Id == booking.Employee).First();
            customer = db.GetTable<Customer>(x => x.Id == booking.Customer).First();
            detail = db.GetTable<BookingRoomDetail>(x => x.BookingRoom == booking.Id).First();
            room = db.GetTable<Room>(x => x.Id == detail.Room).First();

            Id = booking.Id;
            BookingDate = booking.BookingDate.ToString(dateFormat);
            ArrivedDate = booking.ArrivedDate.ToString(dateFormat);
            if (booking.ExpectedDate.Year != 1)
                ExpectedDate = booking.ExpectedDate.ToString(dateFormat);
            EmployeeUnique = employee.UniqueNumber;
            CustomerUnique = customer.UniqueNumber;
            RoomNumber = room.Name;
            if (detail.CheckoutDate.Year != 1)
                CheckoutDate = detail.CheckoutDate.ToString(dateFormat);
        }

        public static IEnumerable<BookingRoomViewModel> GetBookingRooms(DbContext db, string dateFormat = "")
        {
            foreach (BookingRoom b in db.GetTable<BookingRoom>())
            {
                yield return new BookingRoomViewModel(b, db, dateFormat);
            }
        }
    }
}
