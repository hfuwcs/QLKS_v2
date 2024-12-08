using QLKS.Models;
using System;
using System.Linq;
using System.Security.Principal;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace QLKS.Forms
{
    public partial class FormService : Form
    {
        Account account;

        public FormService(Account account)
        {
            InitializeComponent();
            this.account = account;
        }
        static DbContext db = new DbContext(DbContext.ConnectionType.ConfigurationManager, "DefaultConnection");
        void LoadDataSource()
        {
            foreach (Service service in db.GetTable<Service>().ToList())
            {
                dtgvService.Rows.Add(service.Id, service.Name, string.Format("{0:C0}", service.Price));
            }
        }
        void LoadRoomId()
        {
            cboId.DataSource = null;
            cboId.Items.Clear();
            foreach (Service service in db.GetTable<Service>())
            {
                cboId.Items.Add(service.Id);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtSearch.Text))
            {
                MessageBox.Show("Vui lòng nhập vào thông tin để tìm kiếm", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            Service service = db.GetTable<Service>(t => t.Name == txtSearch.Text).FirstOrDefault();
            if (service == null)
            {
                MessageBox.Show("Không tìm thấy dịch vụ này", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            txtName.Text = service.Name;
            txtPrice.Text = string.Format("{0:C0}", service.Price);
            cboId.Text = service.Id.ToString();
        }

        private void FormService_Load(object sender, EventArgs e)
        {
            if (account.Role != "Quản trị viên")
            {
                groupBox3.Visible = false;
            }
            LoadDataSource();
            LoadRoomId();
        }

        private void dtgvService_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dtgvService.Rows[e.RowIndex];
                Service s = db.GetTable<Service>(t => t.Id.ToString() == row.Cells["Id"].Value.ToString()).First();
                txtName.Text = s.Name;
                txtPrice.Text = s.Price.ToString();
                cboId.Text = s.Id.ToString();
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string error = ErrorMessage();
            if (error != null)
            {
                MessageBox.Show(error, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (db.GetTable<Service>(t => t.Name == txtName.Text).FirstOrDefault() != null)
            {
                MessageBox.Show("Dịch vụ đã tồn tại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            Service service = new Service();
            service.Name = txtName.Text;
            service.Price = decimal.Parse(txtPrice.Text);
            if (db.AddRow(service) == null)
            {
                MessageBox.Show("Thêm dịch vụ không thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            };
            LoadDataSource();
            LoadRoomId();
            MessageBox.Show("Thêm dịch vụ thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Helpers.ClearControl(tableLayoutPanel3);
        }
        string ErrorMessage()
        {
            if (string.IsNullOrEmpty(txtName.Text))
                return "Vui lòng nhập vào tên dịch vụ";
            if (string.IsNullOrEmpty(txtPrice.Text))
                return "Vui lòng nhập vào giá dịch vụ";
            return null;
        }
        private void cboId_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboId.SelectedIndex != -1)
            {
                Service s = db.GetTable<Service>(t => t.Id == int.Parse(cboId.Text)).First();
                txtName.Text = s.Name;
                txtPrice.Text = ((int)s.Price).ToString();
                cboId.Text = s.Id.ToString();
            }

        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cboId.Text))
            {
                MessageBox.Show("Vui lòng nhập vào mã dịch vụ", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string error = ErrorMessage();
            if (error != null)
            {
                MessageBox.Show(error, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Service service = new Service();
            service.Name = txtName.Text;
            service.Price = decimal.Parse(txtPrice.Text);
            service.Id = int.Parse(cboId.Text);
            if (!db.UpdateRow(service))
            {
                MessageBox.Show("Cập nhật dịch vụ không thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            LoadDataSource();
            MessageBox.Show("Cập nhật dịch vụ thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Helpers.ClearControl(tableLayoutPanel3);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cboId.Text))
            {
                MessageBox.Show("Vui lòng nhập vào mã dịch vụ", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string error = ErrorMessage();
            if (error != null)
            {
                MessageBox.Show(error, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (MessageBox.Show("Bạn có chắc muốn xóa dịch vụ này?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;
            Func<Service, bool> predicate = p => p.Id == int.Parse(cboId.Text);
            if (db.DeleteRows<Service>($"MADV={cboId.Text}") == 0)
            {
                MessageBox.Show("Xóa dịch vụ không thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            LoadDataSource();
            LoadRoomId();
            MessageBox.Show("Xóa dịch vụ thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Helpers.ClearControl(tableLayoutPanel3);
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
                    comboBox.SelectedIndex = -1;
                    comboBox.Text = "";
                }
                else if (control1 is DateTimePicker)
                {
                    DateTimePicker dateTimePicker = (DateTimePicker)control1;
                    dateTimePicker.Value = DateTime.Now;
                }
            }
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            ClearControl(tableLayoutPanel3);
            ClearControl(tableLayoutPanel4);
        }
    }
}
