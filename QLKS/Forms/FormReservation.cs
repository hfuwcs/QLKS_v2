using QLKS.Forms;
using QLKS.Models;
using QLKS.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace QLKS
{

    public partial class FormReservation : Form
    {
        string RoomNumberMenu = null;
        bool isCompleted = false;
        DateTime ArrivedDateLoad;
        DateTime ExpectedDateLoad;
        public class ListRoomBooked
        {

            public string RoomNumber { get; set; }
            public string ArrivedDate { get; set; }
            public string ExpectedDate { get; set; }
            public ListRoomBooked(string roomnumber, string arriveddate, string expecteddate)
            {
                RoomNumber = roomnumber;
                ArrivedDate = arriveddate;
                ExpectedDate = expecteddate;
            }
        }

        static DbContext db = new DbContext(DbContext.ConnectionType.ConfigurationManager, "DefaultConnection");
        public FormReservation()
        {
            InitializeComponent();
        }
        public FormReservation(string RoomNumber, DateTime Start, DateTime End)
        {
            InitializeComponent();
            this.RoomNumberMenu = RoomNumber;
            this.ArrivedDateLoad = Start;
            this.ExpectedDateLoad = End;
        }
        IEnumerable<ReservationRoomStatus> viewModel = ReservationRoomStatus.GetRooms(db);
        public void LoadingList()
        {
            if (RoomNumberMenu != null)
            {
                foreach (ListViewItem item in lsvRoomEmpty.Items)
                {
                    if (item.Text == RoomNumberMenu)
                    {
                        item.Selected = true;
                    }
                }
                cboRoomNumber.Text = RoomNumberMenu;
                dtpStart.Value = ArrivedDateLoad.Date;
                dtpEnd.Value = ExpectedDateLoad.Date;
            }
            DeleteListView(lsvRoomEmpty, lsvRoomBooked);
            List<string> roomBooked = new List<string>();
            List<string> roomEmpty = new List<string>();
            foreach (ReservationRoomStatus room in viewModel)
            {
                string status = CheckRoomStatus(room.Id, dtpStart.Value, dtpEnd.Value);
                if (status != "Phòng trống")
                {
                    roomBooked.Add(room.Number);
                }
                else if (status == "Phòng trống")
                    roomEmpty.Add(room.Number);
            }
            foreach (string room in roomBooked)
            {
                lsvRoomBooked.Items.Add(room);
            }
            cboRoomNumber.Items.Clear();
            foreach (string room in roomEmpty)
            {
                lsvRoomEmpty.Items.Add(room);
                cboRoomNumber.Items.Add(room);
            }
            if (RoomNumberMenu != null)
            {
                foreach (ListViewItem item in lsvRoomEmpty.Items)
                {
                    if (item.Text == RoomNumberMenu)
                    {
                        item.Selected = true;
                    }
                }
                cboRoomNumber.Text = RoomNumberMenu;
            }
            isCompleted = true;
        }
        //string CheckRoomStatus(int roomId, DateTime dayStart, DateTime dayEnd)
        //{
        //    List<BookingRoom> bookings = db.GetTable<BookingRoom>(p => p.ArrivedDate >= dayStart.Date || p.ExpectedDate <= dayEnd.Date).ToList();
        //    foreach (BookingRoom booking in bookings)
        //    {
        //        Invoice invoice = db.GetTable<Invoice>(p => p.BookingRoom == booking.Id).FirstOrDefault();
        //        if (invoice != null)
        //        {
        //            foreach (BookingRoomDetail room in db.GetTable<BookingRoomDetail>(p => p.BookingRoom == booking.Id))
        //            {
        //                if (roomId == room.Room)
        //                {
        //                    return "Phòng trống";
        //                }
        //            }
        //        }
        //        else
        //        {
        //            ReceivingRoom receiving = db.GetTable<ReceivingRoom>(p => p.BookingRoom == booking.Id).FirstOrDefault();
        //            if (receiving != null)
        //            {
        //                foreach (BookingRoomDetail room in db.GetTable<BookingRoomDetail>(p => p.BookingRoom == booking.Id))
        //                {
        //                    if (roomId == room.Room)
        //                    {
        //                        return "Đã nhận";
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                foreach (BookingRoomDetail room in db.GetTable<BookingRoomDetail>(p => p.BookingRoom == booking.Id))
        //                {
        //                    if (roomId == room.Room)
        //                    {
        //                        return "Đã đặt";
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    return "Phòng trống";
        //}
        string CheckRoomStatus(int roomId, DateTime dayStart, DateTime dayEnd)
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
        private void button6_Click(object sender, EventArgs e)
        {
            FormChangeRoom form = new FormChangeRoom();
            form.ShowDialog();
        }

        private void FormReservation_Load(object sender, EventArgs e)
        {
            dtpEnd.Value = DateTime.Today.AddDays(1);
            LoadingList();
            cboCountry.DataSource = Helpers.Countries;
            cboGender.DataSource = new List<string> { "Nam", "Nữ" };
        }

        private void lsvRoomEmpty_SelectedIndexChanged(object sender, EventArgs e)
        {

            ListView listView = (ListView)sender;
            if (listView.SelectedItems.Count > 0)
            {
                foreach (ReservationRoomStatus room in viewModel)
                {
                    if (room.Number == listView.SelectedItems[0].Text)
                    {
                        txtRoomTypeName.Text = room.NameRoomType;
                        txtPrice.Text = room.Price.ToString();
                        txtMaxPeople.Text = room.MaxPeople.ToString();
                    }
                }
            }
        }

        private void btnSearchCustomer_Click(object sender, EventArgs e)
        {
            Customer customer = db.GetTable<Customer>(t => t.UniqueNumber == txtCustomerId.Text).FirstOrDefault();
            if (customer == null)
            {
                MessageBox.Show("Không tìm thấy khách hàng này", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            txtCustomerIdShow.Text = customer.UniqueNumber.ToString();
            txtCustomerName.Text = customer.Name.ToString();
            dtpDoB.Text = customer.DoB.ToString();
            txtPhoneNumber.Text = customer.Phone.ToString();
            cboGender.Text = customer.Gender.ToString();
            cboCountry.Text = customer.Country.ToString();
        }

        List<ListRoomBooked> listRooms = new List<ListRoomBooked>();

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cboRoomNumber.Text))
            {
                MessageBox.Show("Vui lòng chọn số phòng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            foreach (DataGridViewRow row in dtgvListBookedRoom.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (cell.Value != null)
                    {
                        if (cell.Value.ToString() == cboRoomNumber.Text)
                        {
                            MessageBox.Show("Phòng đã được chọn");
                            return;
                        }
                    }

                }
            }

            //listRooms.Add(new ListRoomBooked(cboRoomNumber.Text,dtpArrivedDate.Text,dtpExpectedRoom.Text));
            dtgvListBookedRoom.Rows.Add(cboRoomNumber.Text, dtpStart.Value.Date.ToString("dd/MM/yyyy"), dtpEnd.Value.Date.ToString("dd/MM/yyyy"));
            //dtgvListBookedRoom.DataSource = null;
            //dtgvListBookedRoom.DataSource = listRooms;
            dtgvListBookedRoom.Refresh();
            foreach (ListViewItem item in lsvRoomEmpty.Items)
            {
                if (item.Text == cboRoomNumber.Text)
                {
                    lsvRoomEmpty.Items.Remove(item);
                    lsvRoomBooked.Items.Add(item);
                    lsvRoomBooked.Refresh();
                    lsvRoomEmpty.Refresh();
                    break;
                }
            }
        }
        void DeleteListView(ListView listView1, ListView listView2)
        {
            listView1.Items.Clear();
            listView2.Items.Clear();
        }

        private void dtpStart_ValueChanged(object sender, EventArgs e)
        {
            if (isCompleted)
            {
                if (dtpStart.Value > dtpEnd.Value)
                {
                    MessageBox.Show("Vui lòng nhập ngày bắt đầu không lớn hơn ngày kết thúc!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    dtpStart.Value = DateTime.Today;
                    dtpEnd.Value = DateTime.Today.AddDays(1);
                    return;
                }

                LoadingList();
            }
        }

        private void dtpEnd_ValueChanged(object sender, EventArgs e)
        {
            if (isCompleted)
            {
                if (dtpStart.Value > dtpEnd.Value)
                {
                    MessageBox.Show("Vui lòng nhập ngày kết thúc không nhỏ hơn ngày kết thúc!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    dtpStart.Value = DateTime.Today;
                    dtpEnd.Value = DateTime.Today.AddDays(1);
                    return;
                }
                LoadingList();
            }
        }
        private int selectedRowIndex = -1;

        private void dtgvListBookedRoom_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (e.RowIndex >= 0)
                {
                    selectedRowIndex = e.RowIndex;
                    dtgvListBookedRoom.ClearSelection();
                    dtgvListBookedRoom.Rows[e.RowIndex].Selected = true;
                    contextMenuStrip1.Show(dtgvListBookedRoom, dtgvListBookedRoom.PointToClient(MousePosition));
                }
            }
        }

        private void xóaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedRowIndex >= 0)
            {
                if (MessageBox.Show("Bạn có muốn xóa phòng này", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    DataGridViewRow selectedRow = dtgvListBookedRoom.Rows[selectedRowIndex];
                    dtgvListBookedRoom.Rows.RemoveAt(selectedRowIndex);
                    bool flag = true;
                    foreach (ListViewItem item in lsvRoomEmpty.Items)
                    {
                        if (item.Text == selectedRow.Cells[0].Value.ToString())
                        {
                            flag = false; break;
                        }
                    }
                    if (flag)
                        lsvRoomEmpty.Items.Add(selectedRow.Cells[0].Value.ToString());
                    foreach (ListViewItem item in lsvRoomBooked.Items)
                    {
                        if (item.Text == selectedRow.Cells[0].Value.ToString())
                        {
                            lsvRoomBooked.Items.Remove(item);
                        }
                    }
                }
            }
        }
        string ErrorMessage()
        {
            if (string.IsNullOrEmpty(txtCustomerName.Text))
                return "Vui lòng nhập vào tên khách hàng";
            if (string.IsNullOrEmpty(txtCustomerIdShow.Text))
                return "Vui lòng nhập vào mã định danh khách hàng";
            if (string.IsNullOrEmpty(txtPhoneNumber.Text))
                return "Vui lòng nhập vào số điện thoại khách hàng";
            return null;
        }
        private void btnBooking_Click(object sender, EventArgs e)
        {
            string error = ErrorMessage();
            if (error != null)
            {
                MessageBox.Show(error, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            int count = 0;
            List<string> roomNumber = new List<string>();
            foreach (DataGridViewRow row in dtgvListBookedRoom.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (cell.Value != null)
                        count++;
                }
                if (row.Cells[0].Value != null)
                    roomNumber.Add(row.Cells[0].Value.ToString());
            }
            if (count == 0)
            {
                MessageBox.Show("Vui lòng chọn phòng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            BookingRoom booking = new BookingRoom();
            booking.BookingDate = DateTime.Today.Date;
            booking.ArrivedDate = dtpStart.Value;
            booking.ExpectedDate = dtpEnd.Value;   
            FormLogin formLogin = new FormLogin();
            booking.Employee = FormLogin.account.Employee;
            Customer customer = db.GetTable<Customer>($"MADD={txtCustomerIdShow.Text}").FirstOrDefault();
            if (customer == null)
            {
                MessageBox.Show("Khách hàng không hợp lệ", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            booking.Customer = customer.Id;
            BookingRoom bookingRoom = db.AddRow<BookingRoom>(booking);
            if (bookingRoom == null)
            {
                MessageBox.Show("Thêm phiếu đặt phòng không thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            foreach (string room in roomNumber)
            {
                BookingRoomDetail roomDetail = new BookingRoomDetail();
                Room room1 = db.GetTable<Room>($"SOPHONG='{room}'").FirstOrDefault();
                if (room1 == null || CheckRoomStatus(room1.Id, dtpStart.Value.Date, dtpEnd.Value.Date) != "Phòng trống")
                {
                    MessageBox.Show($"Phòng {room} không hợp lệ", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                roomDetail.BookingRoom = bookingRoom.Id;
                room1.Status = "Đã đặt";
                roomDetail.Room = room1.Id;
                db.UpdateRow<Room>(room1);
                db.AddRow<BookingRoomDetail>(roomDetail);
            }
            if (label1.Text == "PHIẾU NHẬN PHÒNG")
            {
                ReceivingRoom receiving = new ReceivingRoom();
                List<BookingRoomDetail> roomDetails = db.GetTable<BookingRoomDetail>($"MAPHIEUDATPHONG={bookingRoom.Id}").ToList();
                {
                    foreach (BookingRoomDetail roomDetail in roomDetails)
                    {
                        foreach (Room room in db.GetTable<Room>())
                        {
                            if (room.Id == roomDetail.Room)
                            {
                                room.Status = "Đã nhận";
                                db.UpdateRow<Room>(room);
                            }
                        }
                    }
                }
                receiving.BookingRoom = bookingRoom.Id;
                receiving.ReceivingDate = dtpStart.Value.Date;
                receiving.Employee = FormLogin.account.Employee;
                db.AddRow<ReceivingRoom>(receiving);
                MessageBox.Show("Nhận phòng thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            MessageBox.Show("Đặt phòng thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Helpers.ClearControl(groupBox1);
            Helpers.ClearControl(groupBox3);
            Helpers.ClearControl(groupBox4);
        }
    }
}
