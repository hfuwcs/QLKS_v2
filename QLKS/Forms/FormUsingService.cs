using QLKS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace QLKS.Forms
{
    public partial class FormUsingService : Form
    {
        DbContext db = new DbContext(DbContext.ConnectionType.ConfigurationManager, "DefaultConnection");
        public FormUsingService()
        {
            InitializeComponent();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (cboTypeSearch.SelectedIndex == -1)
            {
                MessageBox.Show("Vui lòng chọn loại thông tin để tìm kiếm", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (string.IsNullOrEmpty(txtSearch.Text))
            {
                MessageBox.Show("Vui lòng nhập vào thông tin để tìm kiếm", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            Customer customer = new Customer();
            if (cboTypeSearch.SelectedIndex == 0)
                customer = db.GetTable<Customer>(t => t.Phone.StartsWith(txtSearch.Text)).FirstOrDefault();
            else
                customer = db.GetTable<Customer>(t => t.UniqueNumber.StartsWith(txtSearch.Text)).FirstOrDefault();
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
            List<BookingRoom> bookings = db.GetTable<BookingRoom>(s => s.Customer == customer.Id).ToList();
            for (int i = bookings.Count - 1; i >= 0; i--)
            {

                Invoice invoice = db.GetTable<Invoice>(v => v.BookingRoom == bookings[i].Id).FirstOrDefault();
                if (invoice != null)
                    bookings.Remove(bookings[i]);
            }
            if (bookings.Count == 0)
            {
                MessageBox.Show("Khách hàng này chưa đặt phòng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            dtgvBooking.Rows.Clear();
            foreach (BookingRoom booking in bookings)
            {
                dtgvBooking.Rows.Add(booking.Id, booking.ArrivedDate.ToString("dd/MM/yyyy"), booking.ExpectedDate.ToString("dd/MM/yyyy"));
            }
        }
        bool isCompleted = false;
        private void FormUsingService_Load(object sender, EventArgs e)
        {
            cboServiceName.DataSource = db.GetTable<Service>();
            cboServiceName.DisplayMember = "Name";
            cboServiceName.ValueMember = "Id";
            cboServiceName.SelectedIndex = -1;
            isCompleted = true;
        }

        private void cboServiceName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboServiceName.SelectedIndex != -1 && isCompleted)
            {
                Service service = db.GetTable<Service>(s => s.Name == cboServiceName.Text).FirstOrDefault();
                if (service != null)
                {
                    txtPrice.Text = ((int)service.Price).ToString();
                }
            }
        }

        private void btnAddService_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cboServiceName.Text))
            {
                MessageBox.Show("Vui lòng chọn dịch vụ!", "Thông báo!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (nmQuantity.Value <= 0)
            {
                MessageBox.Show("Vui lòng nhập số lượng lớn hơn 0!", "Thông báo!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            foreach (DataGridViewRow row in dtgvListService.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (cell.ColumnIndex == 0 && cell.Value != null)
                    {
                        if (cell.Value.ToString() == cboServiceName.Text)
                        {
                            row.Cells[1].Value = int.Parse(nmQuantity.Value.ToString()) + int.Parse(row.Cells[1].Value.ToString());
                            row.Cells[3].Value = int.Parse(row.Cells[1].Value.ToString()) * double.Parse(row.Cells[2].Value.ToString());
                            return;
                        }
                    }
                }
            }
            dtgvListService.Rows.Add(cboServiceName.Text, nmQuantity.Value, txtPrice.Text, $"{double.Parse(nmQuantity.Value.ToString()) * double.Parse(txtPrice.Text)}");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dtgvBooking.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn phiếu đặt phòng!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (dtgvListService.Rows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn dịch vụ!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            BookingService bookingService = new BookingService();
            bookingService.TotalPrice = 0;
            bookingService.Employee = FormLogin.account.Employee;
            bookingService.BookingDate = DateTime.Now;
            if (MessageBox.Show("Tiến hành lập phiếu dịch vụ?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (dtgvBooking.SelectedRows[0].Cells[0].Value != null)
                {
                    bookingService.BookingRoom = int.Parse(dtgvBooking.SelectedRows[0].Cells[0].Value.ToString());
                }
                bookingService = db.AddRow<BookingService>(bookingService);
                foreach (DataGridViewRow row in dtgvListService.Rows)
                {
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        if (cell.ColumnIndex == 0 && cell.Value != null)
                        {
                            Service service = db.GetTable<Service>(s => s.Name == cell.Value.ToString()).FirstOrDefault();
                            BookingServiceDetail detail = new BookingServiceDetail();
                            detail.Service = service.Id;
                            detail.Quantity = int.Parse(row.Cells[1].Value.ToString());
                            detail.BookingService = bookingService.Id;
                            detail = db.AddRow<BookingServiceDetail>(detail);
                            if (detail == null)
                            {
                                MessageBox.Show($"Thêm dịch vụ {service.Name} không thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return;
                            }
                            string result = Regex.Match(service.Price.ToString(), "^\\d+").Value;
                            bookingService.TotalPrice += int.Parse(result) * detail.Quantity;
                            db.UpdateRow<BookingService>(bookingService);
                        }
                    }
                }
                MessageBox.Show("Lập phiếu dịch vụ thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                dtgvListService.Rows.Clear();
                dtgvInvoiceService.Rows.Clear();
            }

        }
        public decimal TinhTienPhong(int maphieudat)
        {
            decimal tien = 0;
            BookingRoom booking = db.GetTable<BookingRoom>(x => x.Id == maphieudat).First();
            foreach (BookingRoomDetail detail in db.GetTable<BookingRoomDetail>(b => b.BookingRoom == maphieudat))
            {
                detail.CheckoutDate = DateTime.Now;
                db.UpdateRow<BookingRoomDetail>(detail);
                Room room = db.GetTable<Room>(r => r.Id == detail.Room).FirstOrDefault();
                RoomType type = db.GetTable<RoomType>(t => t.Id == room.RoomType).FirstOrDefault();                
                tien += (type.Price * ((detail.CheckoutDate - booking.ArrivedDate).Days + 1));
            }
            return tien;
        }
        public decimal TinhTienDichVu(int maphieudat)
        {
            decimal tien = 0;
            List<BookingService> services = db.GetTable<BookingService>(b => b.BookingRoom == maphieudat).ToList();
            foreach (BookingService service in services)
            {
                tien += service.TotalPrice;
            }
            return tien;
        }

        private void dtgvBooking_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                int maphieudat = int.Parse(dtgvBooking.Rows[e.RowIndex].Cells[0].Value.ToString());
                txtMoney.Text = string.Format("{0:C0}", (TinhTienPhong(maphieudat) + TinhTienDichVu(maphieudat)));
                dtgvInvoiceService.Rows.Clear();
                dtgvBooking.Rows[e.RowIndex].Selected = true;
                List<BookingService> bookingServices = db.GetTable<BookingService>(b => b.BookingRoom == int.Parse(dtgvBooking.Rows[e.RowIndex].Cells[0].Value.ToString())).ToList();
                foreach (BookingService service in bookingServices)
                {
                    dtgvInvoiceService.Rows.Add(service.Id, string.Format("{0:C0}", service.TotalPrice));
                }
                dtgvInvoiceRoom.Rows.Clear();
                List<BookingRoom> bookings = db.GetTable<BookingRoom>(b => b.Id == int.Parse(dtgvBooking.Rows[e.RowIndex].Cells[0].Value.ToString())).ToList();
                List<Room> rooms = new List<Room>();
                foreach (BookingRoom room in bookings)
                {
                    foreach (BookingRoomDetail detail in db.GetTable<BookingRoomDetail>(d => d.BookingRoom == room.Id))
                    {
                        rooms.Add(db.GetTable<Room>(r => r.Id == detail.Room).FirstOrDefault());
                    }
                }
                foreach (Room room1 in rooms)
                {
                    RoomType type = db.GetTable<RoomType>(t => t.Id == room1.RoomType).FirstOrDefault();
                    dtgvInvoiceRoom.Rows.Add(room1.Name, type.Name, string.Format("{0:C0}", type.Price));
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Helpers.ClearControl(tableLayoutPanel6);
            Helpers.ClearControl(tableLayoutPanel5);
            Helpers.ClearControl(tableLayoutPanel3);
            dtgvBooking.Rows.Clear();
            dtgvInvoiceRoom.Rows.Clear();
            dtgvInvoiceService.Rows.Clear();
            dtgvListService.Rows.Clear();
        }
        private int selectedRowIndex = -1;
        private void xóaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedRowIndex >= 0)
            {
                if (MessageBox.Show("Bạn có muốn xóa dịch vụ này", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    DataGridViewRow selectedRow = dtgvListService.Rows[selectedRowIndex];
                    dtgvListService.Rows.RemoveAt(selectedRowIndex);
                }
            }
        }

        private void dtgvListService_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (e.RowIndex >= 0)
                {
                    selectedRowIndex = e.RowIndex;
                    dtgvListService.ClearSelection();
                    dtgvListService.Rows[e.RowIndex].Selected = true;
                    contextMenuStrip1.Show(dtgvListService, dtgvListService.PointToClient(MousePosition));
                }
            }
        }

        private void btnPayment_Click(object sender, EventArgs e)
        {
            if (dtgvBooking.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn phiếu dịch vụ để thanh toán!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (MessageBox.Show("Tiến hành lập hóa đơn?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                int maphieudat = int.Parse(dtgvBooking.SelectedRows[0].Cells[0].Value.ToString());
                Invoice invoice = new Invoice();
                invoice.BookingRoom = maphieudat;
                invoice.InvoiceDate = DateTime.Now;
                invoice.RoomPrice = TinhTienPhong(maphieudat);
                invoice.ServicePrice = TinhTienDichVu(maphieudat);
                invoice.TotalPrice = invoice.RoomPrice + invoice.ServicePrice;
                invoice.Employee = FormLogin.account.Employee;
                db.AddRow<Invoice>(invoice);
                if (db == null)
                {
                    MessageBox.Show("Thêm hóa đơn không thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                MessageBox.Show("Thêm hóa đơn thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Helpers.ClearControl(tableLayoutPanel6);
                Helpers.ClearControl(tableLayoutPanel5);
                Helpers.ClearControl(tableLayoutPanel3);
                dtgvBooking.Rows.Clear();
                dtgvInvoiceRoom.Rows.Clear();
                dtgvInvoiceService.Rows.Clear();
                dtgvListService.Rows.Clear();
            }
        }
    }
}
