using QLKS.Models;
using QLKS.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace QLKS.Forms
{
    public partial class FormRoom : Form
    {
        static DbContext db = new DbContext(DbContext.ConnectionType.ConfigurationManager, "DefaultConnection"); 
        Account account;

        public FormRoom(Account account)
        {
            InitializeComponent();
            this.account = account;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            FormRoomType frmRoomType = new FormRoomType();
            frmRoomType.ShowDialog();
        }
        IEnumerable<RoomViewModel> View = RoomViewModel.GetRooms(db);


        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtSearch.Text))
            {
                MessageBox.Show("Vui lòng nhập vào thông tin để tìm kiếm", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            RoomViewModel room = null;
            foreach (RoomViewModel view in View)
            {
                if (view.Number == txtSearch.Text)
                {
                    room = view;
                    break;
                }
            }
            if (room == null)
            {
                MessageBox.Show("Không tìm thấy phòng này", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            txtNumber.Text = room.Number;
            cboStatus.Text = room.Status;
            cboTypeId.Text = room.TypeId.ToString();
            txtTypeName.Text = room.Type;
            txtMaxPeople.Text = room.MaxPeople.ToString();
            txtPrice.Text = room.Price.ToString();
            cboRoomId.Text = room.Id.ToString();
        }
        IEnumerable<RoomType> types = db.GetTable<RoomType>();
        private void FormRoom_Load(object sender, EventArgs e)
        {
            if (account.Role != "Quản trị viên")
            {
                groupBox3.Visible = false;
                groupBox4.Visible = false;
            }
            LoadDataSource();
            List<string> list = new List<string>();
            foreach (RoomType type in types)
            {
                list.Add(type.Id.ToString());
            }
            cboTypeId.DataSource = list;
            LoadRoomId();
        }
        void LoadDataSource()
        {
            List<RoomViewModel> list = new List<RoomViewModel>();
            foreach (RoomViewModel view in View)
            {
                list.Add(view);
            }
            dtgvRoom.DataSource = list;
        }
        void LoadRoomId()
        {
            cboRoomId.Items.Clear();
            foreach (Room room in db.GetTable<Room>())
            {
                cboRoomId.Items.Add(room.Id);
            }
        }
        private void dtgvRoom_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dtgvRoom.Rows[e.RowIndex];
                txtNumber.Text = row.Cells["Number"].Value?.ToString();
                cboStatus.Text = row.Cells["Status"].Value?.ToString();
                cboTypeId.Text = row.Cells["TypeId"].Value?.ToString();
                txtTypeName.Text = row.Cells["Type"].Value?.ToString(); ;
                txtMaxPeople.Text = row.Cells["MaxPeople"].Value?.ToString(); ;
                txtPrice.Text = row.Cells["Price"].Value?.ToString();
                cboRoomId.Text = row.Cells["Id"].Value?.ToString();
                txtPrice.Text = decimal.Parse(txtPrice.Text, NumberStyles.Currency, CultureInfo.CurrentCulture).ToString();
            }
        }
        string ErrorMessage()
        {
            if (string.IsNullOrEmpty(txtNumber.Text))
                return "Vui lòng nhập vào số phòng";
            if (string.IsNullOrEmpty(cboTypeId.Text))
                return "Vui lòng nhập vào mã loại phòng";
            if (string.IsNullOrEmpty(txtTypeName.Text))
                return "Vui lòng nhập vào tên loại phòng";
            if (string.IsNullOrEmpty(txtPrice.Text))
                return "Vui lòng nhập vào giá phòng";
            if (string.IsNullOrEmpty(cboStatus.Text))
                return "Vui lòng nhập vào trạng thái phòng";
            if (string.IsNullOrEmpty(txtMaxPeople.Text))
                return "Vui lòng nhập vào số lượng người tối đa";
            return null;
        }
        private void btnAdd_Click(object sender, EventArgs e)
        {
            string error = ErrorMessage();
            if (cboStatus.SelectedIndex != 0)
            {
                MessageBox.Show("Trạng thái phòng khi thêm chỉ là phòng trống!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (error != null)
            {
                MessageBox.Show(error, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (db.GetTable<Room>(t => t.Name == txtNumber.Text).FirstOrDefault() != null)
            {
                MessageBox.Show("Phòng đã tồn tại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            Room room = new Room();
            room.Name = txtNumber.Text;
            room.Status = cboStatus.Text;
            room.RoomType = int.Parse(cboTypeId.Text);
            if (db.AddRow(room) == null)
            {
                MessageBox.Show("Thêm phòng không thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            };
            LoadDataSource();
            LoadRoomId();
            MessageBox.Show("Thêm phòng thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Helpers.ClearControl(tableLayoutPanel6);
        }

        private void cboTypeId_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboTypeId.SelectedIndex >= 0)
            {
                RoomType roomType = types.First(t => t.Id == int.Parse(cboTypeId.Text));
                txtMaxPeople.Text = roomType.MaxPeople.ToString();
                txtTypeName.Text = roomType.Name;
                txtPrice.Text = string.Format("{0:C0}", roomType.Price);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cboRoomId.Text))
            {
                MessageBox.Show("Vui lòng nhập vào mã phòng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string error = ErrorMessage();
            if (error != null)
            {
                MessageBox.Show(error, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Room room = new Room();
            room.Name = txtNumber.Text;
            room.Status = cboStatus.Text;
            room.RoomType = int.Parse(cboTypeId.Text);
            room.Id = int.Parse(cboRoomId.Text);
            if (!db.UpdateRow(room))
            {
                MessageBox.Show("Cập nhật phòng không thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            LoadDataSource();
            MessageBox.Show("Cập nhật phòng thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Helpers.ClearControl(tableLayoutPanel6);
        }

        private void cboRoomId_SelectedIndexChanged(object sender, EventArgs e)
        {
            RoomViewModel room = new RoomViewModel();
            foreach (RoomViewModel roomView in RoomViewModel.GetRooms(db))
            {
                if (roomView.Id == int.Parse(cboRoomId.Text))
                {
                    room = roomView; break;
                }
            }
            txtNumber.Text = room.Number;
            cboStatus.Text = room.Status;
            cboTypeId.Text = room.TypeId.ToString();
            txtTypeName.Text = room.Type;
            txtMaxPeople.Text = room.MaxPeople.ToString();
            txtPrice.Text = room.Price.ToString();
        }
        void ClearControl(Control control)
        {
            foreach (Control control1 in control.Controls)
            {
                if (control1 is TextBox)
                {
                    TextBox textBox = (TextBox)control1;
                    textBox.Clear();
                }
                else if (control1 is ComboBox)
                {
                    ComboBox comboBox = (ComboBox)control1;
                    comboBox.SelectedIndex = 0;
                }
                else if (control1 is DateTimePicker)
                {
                    DateTimePicker dateTimePicker = (DateTimePicker)control1;
                    dateTimePicker.Value = DateTime.Now;
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cboRoomId.Text))
            {
                MessageBox.Show("Vui lòng nhập vào mã phòng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string error = ErrorMessage();
            if (error != null)
            {
                MessageBox.Show(error, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (MessageBox.Show("Bạn có chắc muốn xóa phòng này?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;
            Func<Room, bool> predicate = p => p.Id == int.Parse(cboRoomId.Text);
            if (db.DeleteRows<Room>($"MAPHONG={cboRoomId.Text}") == 0)
            {
                MessageBox.Show("Xóa phòng không thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            LoadDataSource();
            LoadRoomId();
            MessageBox.Show("Xóa phòng thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Helpers.ClearControl(tableLayoutPanel6);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ClearControl(groupBox2);
            ClearControl(groupBox1);
            ClearControl(tableLayoutPanel6);
        }
    }
}
