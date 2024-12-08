using QLKS.Models;
using QLKS.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace QLKS.Forms
{
    public partial class FormRoomManagement : Form
    {

        int disBefore = 0;
        void CreateRoomUI(int index, LoadingListRoom loading, int indexType, string status)
        {
            Panel panelRoom = new Panel();
            if (status == "Đã đặt")
                panelRoom.BackColor = System.Drawing.Color.DimGray;
            else if (status == "Đã nhận")
                panelRoom.BackColor = System.Drawing.Color.LightSeaGreen;
            else
                panelRoom.BackColor = System.Drawing.Color.SeaGreen;
            Panel panelChild = new Panel();
            panelChild.BackColor = System.Drawing.Color.Gainsboro;
            Label dayStart = new Label();
            dayStart.AutoSize = true;
            dayStart.ForeColor = System.Drawing.Color.Black;
            dayStart.Location = new System.Drawing.Point(3, 7);
            dayStart.Name = "lbl_dayStart";
            dayStart.Size = new System.Drawing.Size(52, 25);
            dayStart.TabIndex = 0;
            if (status == "Đã đặt" || status == "Đã nhận")
                dayStart.Text = loading.ArrivedDate;
            else
                dayStart.Text = "Trống";
            Label dayEnd = new Label();
            dayEnd.AutoSize = true;
            dayEnd.ForeColor = System.Drawing.Color.Black;
            dayEnd.Location = new System.Drawing.Point(120, 7);
            dayEnd.Name = "lbl_dayEnd";
            dayEnd.Size = new System.Drawing.Size(52, 25);
            dayEnd.TabIndex = 0;
            if (status == "Đã đặt" || status == "Đã nhận")
                dayEnd.Text = loading.ExpectedDate;
            else
                dayEnd.Text = "Trống";
            panelChild.Controls.Add(dayStart);
            panelChild.Controls.Add(dayEnd);
            panelChild.Dock = System.Windows.Forms.DockStyle.Bottom;
            panelChild.Location = new System.Drawing.Point(0, 88);
            panelChild.Name = "panelChild";
            panelChild.Size = new System.Drawing.Size(271, 37);
            panelRoom.Controls.Add(panelChild);
            PictureBox pictureBoxStatus = new PictureBox();
            if (status == "Đã đặt" || status == "Đã nhận")
                pictureBoxStatus.Image = ((System.Drawing.Image)(Properties.Resources.user));
            else
                pictureBoxStatus.Image = ((System.Drawing.Image)(Properties.Resources.check));
            pictureBoxStatus.Location = new System.Drawing.Point(29, 53);
            pictureBoxStatus.Name = "pictureBox2";
            pictureBoxStatus.Size = new System.Drawing.Size(24, 24);
            pictureBoxStatus.TabStop = false;
            panelRoom.Controls.Add(pictureBoxStatus);
            Label roomStatus = new Label();
            roomStatus.AutoSize = true;
            roomStatus.Location = new System.Drawing.Point(110, 9);
            roomStatus.Name = "lblRoomStatus";
            roomStatus.Size = new System.Drawing.Size(120, 25);
            roomStatus.TabIndex = 0;
            if (status == "Đã đặt")
                roomStatus.Text = "Phòng đã đặt";
            else if (status == "Đã nhận")
                roomStatus.Text = "Phòng đã nhận";
            else
                roomStatus.Text = "Phòng trống";
            panelRoom.Controls.Add(roomStatus);
            Label nameCustomer = new Label();
            nameCustomer.AutoSize = true;
            nameCustomer.Location = new System.Drawing.Point(81, 53);
            nameCustomer.Name = "lblCustomer";
            nameCustomer.Size = new System.Drawing.Size(131, 25);
            nameCustomer.TabIndex = 0;
            if (status == "Đã đặt" || status == "Đã nhận")
                nameCustomer.Text = loading.CustomerName;
            else
                nameCustomer.Text = "Phòng trống";
            panelRoom.Controls.Add(nameCustomer);
            Label nameRoom = new Label();
            nameRoom.AutoSize = true;
            nameRoom.Location = new System.Drawing.Point(14, 9);
            nameRoom.Name = "lblRoomNumber";
            nameRoom.Size = new System.Drawing.Size(52, 25);
            nameRoom.TabIndex = 0;
            nameRoom.Text = loading.Number;
            panelRoom.Controls.Add(nameRoom);

            panelRoom.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            panelRoom.ForeColor = System.Drawing.Color.White;
            int dis = 0;
            if (index >= 4)
            {
                dis = index / 4;
            }
            if ((index / 4) > 0)
                panelRoom.Location = new System.Drawing.Point(33 + (index % 4) * 315, 120 + disBefore * 140 + indexType * 180 + dis * 140);
            else
                panelRoom.Location = new System.Drawing.Point(33 + index * 315, 120 + disBefore * 140 + indexType * 180 + dis * 140);
            panelRoom.Name = "panelRoom";
            panelRoom.Size = new System.Drawing.Size(220, 125);
            //ContextMenu
            ToolStripMenuItem item1 = new ToolStripMenuItem();
            item1.Image = ((System.Drawing.Image)(Properties.Resources.booking));
            item1.Name = "toolStripMenuItem1";
            item1.Size = new System.Drawing.Size(214, 26);
            item1.Text = "Đặt phòng";

            ToolStripMenuItem item2 = new ToolStripMenuItem();
            item2.Image = ((System.Drawing.Image)(Properties.Resources.receiving));
            item2.Name = "toolStripMenuItem2";
            item2.Size = new System.Drawing.Size(214, 26);
            item2.Text = "Nhận phòng";

            ToolStripMenuItem item3 = new ToolStripMenuItem();
            item3.Image = ((System.Drawing.Image)(Properties.Resources.service));
            item3.Name = "toolStripMenuItem3";
            item3.Size = new System.Drawing.Size(214, 26);
            item3.Text = "Thêm dịch vụ";

            ContextMenuStrip contextMenu = new ContextMenuStrip();
            contextMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            item1,
            item2,
            item3 });
            contextMenu.Name = "Function";
            contextMenu.Size = new System.Drawing.Size(215, 110);
            contextMenu.Text = "Chức năng";
            contextMenu.Items[0].Click += Item1_Click;
            contextMenu.Items[1].Click += Item2_Click;
            panelRoom.ContextMenuStrip = contextMenu;

            panelMain.Controls.Add(panelRoom);
        }
        private void Item1_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;
            ContextMenuStrip contextMenu = (ContextMenuStrip)menuItem.Owner;
            Panel panel = (Panel)contextMenu.SourceControl;
            foreach (Control control in panel.Controls)
            {
                if (control is Label)
                {
                    Label label = (Label)control;
                    if (label.Name == "lblRoomStatus")
                    {
                        if (label.Text == "Phòng đã đặt")
                        {
                            MessageBox.Show("Phòng đã được đặt", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                        else if (label.Text == "Phòng đã nhận")
                        {
                            MessageBox.Show("Phòng đang được thuê", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                    }
                    if (label.Name == "lblRoomNumber")
                    {
                        FormReservation reservation = new FormReservation(label.Text, dtpStart.Value, dtpEnd.Value);
                        reservation.StartPosition = FormStartPosition.CenterScreen;
                        reservation.ShowDialog();
                    }
                }
            }
        }
        private void Item2_Click(object sender, EventArgs e)
        {
            //if (dtpStart.Value.Date != DateTime.Now.Date)
            //{
            //    MessageBox.Show("Chỉ được nhận phòng trong ngày hôm nay", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    return;
            //}
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;
            ContextMenuStrip contextMenu = (ContextMenuStrip)menuItem.Owner;
            Panel panel = (Panel)contextMenu.SourceControl;
            string status = null;
            foreach (Control control in panel.Controls)
            {
                if (control is Label)
                {
                    Label label = (Label)control;
                    if (label.Name == "lblRoomStatus")
                    {
                        if (label.Text == "Phòng đã nhận")
                        {
                            MessageBox.Show("Phòng đang được thuê", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                        status = label.Text;
                    }
                    Room room = new Room();
                    if (label.Name == "lblRoomNumber")
                    {
                        room = db.GetTable<Room>($"SOPHONG='{label.Text}'").FirstOrDefault();
                        if (status == "Phòng trống")
                        {
                            FormReservation reservation = new FormReservation(label.Text, dtpStart.Value, dtpEnd.Value);
                            foreach (Control control1 in reservation.Controls)
                            {
                                if (control1 is TabControl tabControl)
                                {
                                    foreach (Control control2 in tabControl.TabPages[0].Controls)
                                    {
                                        if (control2.Name == "label1")
                                        {
                                            Label label1 = (Label)control2;
                                            label1.Text = "PHIẾU NHẬN PHÒNG";
                                        }
                                        if (control2.Name == "groupBox6")
                                        {
                                            foreach (Control control3 in control2.Controls)
                                            {
                                                if (control3.Name == "btnBooking")
                                                {
                                                    control3.Text = "Nhận phòng";
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            reservation.StartPosition = FormStartPosition.CenterScreen;
                            reservation.ShowDialog();
                        }
                        else if (status == "Phòng đã đặt")
                        {

                            List<BookingRoomDetail> bookings = db.GetTable<BookingRoomDetail>(t => t.Room == room.Id).ToList();
                            foreach (BookingRoomDetail booking in bookings)
                            {
                                BookingRoom booking1 = db.GetTable<BookingRoom>(p => p.Id == booking.BookingRoom).First();
                                if (!(booking1.ExpectedDate <= dtpStart.Value.Date || booking1.ArrivedDate >= dtpEnd.Value.Date))
                                {
                                    Customer customer = db.GetTable<Customer>($"MAKH={booking1.Customer}").FirstOrDefault();
                                    List<Room> rooms = new List<Room>();
                                    List<BookingRoomDetail> booking1s = db.GetTable<BookingRoomDetail>($"MAPHIEUDATPHONG={booking1.Id}").ToList();
                                    foreach (BookingRoomDetail detail in booking1s)
                                    {
                                        rooms.Add(db.GetTable<Room>($"MAPHONG={detail.Room}").FirstOrDefault());
                                    }
                                    string roomMessage = "Bạn có chắc muốn nhận phòng: ";
                                    foreach (Room room1 in rooms)
                                    {
                                        if (room1.Id == rooms[rooms.Count - 1].Id)
                                            roomMessage += room1.Name;
                                        else
                                            roomMessage += room1.Name + ", ";
                                    }
                                    roomMessage += $" của khách hàng {customer.Name}";
                                    if (MessageBox.Show(roomMessage, "Thôn báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                                    {
                                        ReceivingRoom receivingRoom = new ReceivingRoom();
                                        receivingRoom.ReceivingDate = DateTime.Now;
                                        receivingRoom.BookingRoom = booking1.Id;
                                        receivingRoom.Employee = 1;
                                        db.AddRow<ReceivingRoom>(receivingRoom);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        void LoadRoomType(int indexType, string roomTypeName)
        {
            Label roomType = new Label();
            roomType.AutoSize = true;
            roomType.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            roomType.Location = new System.Drawing.Point(30, 95 + disBefore * 140 + indexType * 178);
            roomType.Name = "lblRoom";
            roomType.Size = new System.Drawing.Size(109, 28);
            roomType.Text = roomTypeName;
            panelMain.Controls.Add(roomType);
        }
        DbContext db = new DbContext(DbContext.ConnectionType.ConfigurationManager, "DefaultConnection");
        void LoadListRoom()
        {
            List<IEnumerable<Room>> rooms = new List<IEnumerable<Room>>();
            IEnumerable<LoadingListRoom> listRoom = LoadingListRoom.GetRooms(db, dtpStart.Value.Date, dtpEnd.Value.Date);
            IEnumerable<RoomType> roomTypes = db.GetTable<RoomType>();
            List<RoomType> roomTypesCopy = new List<RoomType>();
            foreach (RoomType roomType in roomTypes)
            {
                rooms.Add(db.GetTable<Room>(t => t.RoomType == roomType.Id));
                roomTypesCopy.Add(roomType);
            }
            int indexType = 0;
            int countSum = 0;
            int countBefore = 0;
            foreach (IEnumerable<Room> lst in rooms)
            {
                int index = 0;
                foreach (Room room in lst)
                {
                    LoadingListRoom loading = new LoadingListRoom(room, db, dtpStart.Value.Date, dtpEnd.Value.Date);
                    string status = CheckRoomStatus(room.Id, dtpStart.Value.Date, dtpEnd.Value.Date);
                    if (status == "Đã đặt" || status == "Đã nhận")
                        loading = listRoom.Where(t => t.IdRoom == room.Id).FirstOrDefault();
                    CreateRoomUI(index, loading, indexType, status);
                    index++;
                    countSum++;
                }
                LoadRoomType(indexType, roomTypesCopy[indexType].Name);
                indexType++;
                countBefore = lst.Count() - 1;
                disBefore += (lst.Count() - 1) / 4;
            }
        }
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
        //async Task LoadListRoomAsync()
        //{
        //    List<IEnumerable<Room>> rooms = new List<IEnumerable<Room>>();
        //    IEnumerable<LoadingListRoom> listRoom = await Task.Run(() => LoadingListRoom.GetRooms(db, dtpStart.Value.Date, dtpEnd.Value.Date));
        //    IEnumerable<RoomType> roomTypes = await Task.Run(() => db.GetTable<RoomType>());
        //    List<RoomType> roomTypesCopy = new List<RoomType>();

        //    foreach (RoomType roomType in roomTypes)
        //    {
        //        rooms.Add(await Task.Run(() => db.GetTable<Room>(t => t.RoomType == roomType.Id)));
        //        roomTypesCopy.Add(roomType);
        //    }

        //    int indexType = 0;
        //    int countSum = 0;
        //    int countBefore = 0;
        //    foreach (IEnumerable<Room> lst in rooms)
        //    {
        //        int index = 0;
        //        foreach (Room room in lst)
        //        {
        //            LoadingListRoom loading = new LoadingListRoom(room, db, dtpStart.Value.Date, dtpEnd.Value.Date);
        //            if (room.Status == "Đã đặt" || room.Status == "Đã nhận")
        //                loading = listRoom.Where(t => t.IdRoom == room.Id).First();
        //            CreateRoomUI(index, loading, indexType, CheckRoomStatus(loading.IdRoom, dtpStart.Value, dtpEnd.Value));
        //            index++;
        //            countSum++;
        //        }
        //        LoadRoomType(indexType, roomTypesCopy[indexType].Name);
        //        indexType++;
        //        countBefore = lst.Count() - 1;
        //        disBefore += (lst.Count() - 1) / 4;
        //    }
        //}

        public FormRoomManagement()
        {
            InitializeComponent();
        }
        public FormRoomManagement(string RoomNumber)
        {
            InitializeComponent();
        }

        private void FormRoomManagement_Load(object sender, EventArgs e)
        {
            dtpEnd.Value = DateTime.Today.AddDays(1);
            disBefore = 0;
            DeletePanel(panelMain);
            LoadListRoom();
        }
        void DeletePanel(Panel panel)
        {
            foreach (Control control in panel.Controls)
            {
                control.Dispose();
            }
            panel.Controls.Clear();
        }
        private void dtpStart_ValueChanged(object sender, EventArgs e)
        {
            if (dtpStart.Value > dtpEnd.Value)
            {
                MessageBox.Show("Vui lòng nhập ngày bắt đầu không lớn hơn ngày kết thúc!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                dtpStart.Value = DateTime.Today;
                dtpEnd.Value = DateTime.Today.AddDays(1);
                return;
            }
            disBefore = 0;
            DeletePanel(panelMain);
            LoadListRoom();
        }

        private void dtpEnd_ValueChanged(object sender, EventArgs e)
        {
            if (dtpStart.Value > dtpEnd.Value)
            {
                MessageBox.Show("Vui lòng nhập ngày kết thúc không nhỏ hơn ngày kết thúc!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                dtpStart.Value = DateTime.Today;
                dtpEnd.Value = DateTime.Today.AddDays(1);
                return;
            }
            disBefore = 0;
            DeletePanel(panelMain);
            LoadListRoom();
        }
        //private async void dtpStart_ValueChanged(object sender, EventArgs e)
        //{
        //    this.Invalidate();
        //    await LoadListRoomAsync();  // Sử dụng async method
        //    this.Refresh();
        //}
    }
}
