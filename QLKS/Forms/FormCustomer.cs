using QLKS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace QLKS.Forms
{
    public partial class FormCustomer : Form
    {
        static DbContext db = new DbContext(DbContext.ConnectionType.ConfigurationManager, "DefaultConnection");
        public FormCustomer()
        {
            InitializeComponent();
        }

        private void FormCustomer_Load(object sender, EventArgs e)
        {
            cboTypeSearch.SelectedIndex = 0;
            cboCountry.DataSource = Helpers.Countries;
            cboGender.DataSource = new List<string> { "Nam", "Nữ" };
            dtgvCustomer.DataSource = db.GetTable<Customer>();
            LoadCustomerId();
        }
        void LoadCustomerId()
        {
            cboCustomerId.DataSource = null;
            cboCustomerId.Items.Clear();
            foreach (Customer customer in db.GetTable<Customer>())
            {
                cboCustomerId.Items.Add(customer.Id);
            }
        }
        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtSearch.Text))
            {
                MessageBox.Show("Vui lòng nhập vào thông tin để tìm kiếm", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            Customer customer = new Customer();
            if (cboTypeSearch.SelectedIndex == 0)
                customer = db.GetTable<Customer>(t => t.UniqueNumber.StartsWith(txtSearch.Text)).FirstOrDefault();
            else
                customer = db.GetTable<Customer>(t => t.Phone.StartsWith(txtSearch.Text)).FirstOrDefault();
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
            cboCustomerId.Text = customer.Id.ToString();
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
        private void btnAdd_Click(object sender, EventArgs e)
        {
            string error = ErrorMessage();
            if (error != null)
            {
                MessageBox.Show(error, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            Customer temp = db.GetTable<Customer>(t => t.UniqueNumber.Equals(txtCustomerIdShow.Text, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            if (temp != null)
            {
                MessageBox.Show("Khách hàng đã tồn tại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            Customer customer = new Customer();
            customer.Name = txtCustomerName.Text;
            customer.DoB = dtpDoB.Value;
            customer.Gender = cboGender.Text;
            customer.Country = cboCountry.Text;
            customer.Phone = txtPhoneNumber.Text;
            customer.UniqueNumber = txtCustomerIdShow.Text;
            var addedCustomer = db.AddRow(customer);
            if (addedCustomer.Id == 0)
            {
                MessageBox.Show("Thêm khách hàng không thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            dtgvCustomer.DataSource = db.GetTable<Customer>().ToList();
            LoadCustomerId();
            MessageBox.Show("Thêm khách hàng thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void dtgvCustomer_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dtgvCustomer.Rows[e.RowIndex];
                txtCustomerName.Text = row.Cells["NameCustomer"].Value?.ToString();
                txtPhoneNumber.Text = row.Cells["Phone"].Value?.ToString();
                DateTime date = DateTime.Parse(row.Cells["DoB"].Value?.ToString());
                dtpDoB.Value = date.Date;
                cboCountry.Text = row.Cells["Country"].Value?.ToString();
                cboGender.Text = row.Cells["Gender"].Value?.ToString();
                txtCustomerIdShow.Text = row.Cells["UniqueNumber"].Value?.ToString();
                cboCustomerId.Text = row.Cells["Id"].Value?.ToString();
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cboCustomerId.Text))
            {
                MessageBox.Show("Vui lòng nhập vào mã khách hàng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string error = ErrorMessage();
            if (error != null)
            {
                MessageBox.Show(error, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            Customer customer = new Customer();
            customer.Name = txtCustomerName.Text;
            customer.DoB = dtpDoB.Value;
            customer.Gender = cboGender.Text;
            customer.Country = cboCountry.Text;
            customer.Phone = txtPhoneNumber.Text;
            customer.UniqueNumber = txtCustomerIdShow.Text;
            customer.Id = int.Parse(cboCustomerId.Text);
            if (!db.UpdateRow(customer))
            {
                MessageBox.Show("Cập nhật khách hàng không thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            dtgvCustomer.DataSource = db.GetTable<Customer>();
            MessageBox.Show("Cập nhật thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cboCustomerId.Text))
            {
                MessageBox.Show("Vui lòng nhập vào mã khách hàng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string error = ErrorMessage();
            if (error != null)
            {
                MessageBox.Show(error, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (MessageBox.Show("Bạn có chắc muốn xóa khách hàng này?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;
            Customer customer = new Customer();
            customer.Name = txtCustomerName.Text;
            customer.DoB = dtpDoB.Value;
            customer.Gender = cboGender.Text;
            customer.Country = cboCountry.Text;
            customer.Phone = txtPhoneNumber.Text;
            customer.UniqueNumber = txtCustomerIdShow.Text;
            customer.Id = int.Parse(cboCustomerId.Text);
            if (db.DeleteRows<Customer>($"MAKH={customer.Id.ToString()}") == 0)
            {
                MessageBox.Show("Xóa khách hàng không thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            dtgvCustomer.DataSource = db.GetTable<Customer>();
            LoadCustomerId();
            MessageBox.Show("Xóa khách hàng thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
        private void btnClose_Click(object sender, EventArgs e)
        {
            ClearControl(grbCustomer);
        }

        private void cboCustomerId_SelectedIndexChanged(object sender, EventArgs e)
        {
            Customer customer = db.GetTable<Customer>(t => t.Id == int.Parse(cboCustomerId.Text)).First();
            txtCustomerName.Text = customer.Name;
            dtpDoB.Value = customer.DoB;
            cboGender.Text = customer.Gender;
            cboCountry.Text = customer.Country;
            txtPhoneNumber.Text = customer.Phone;
            txtCustomerIdShow.Text = customer.UniqueNumber;
        }
    }
}
