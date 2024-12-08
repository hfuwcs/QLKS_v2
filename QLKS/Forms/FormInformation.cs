using QLKS.Models;
using QLKS.Models.Extensions;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace QLKS.Forms
{
    public partial class FormInformation : Form
    {
        static DbContext db = new DbContext(DbContext.ConnectionType.ConfigurationManager, "DefaultConnection");
        Account account;

        public FormInformation(Account account)
        {
            InitializeComponent();
            this.account = account;
        }

        private void FormInformation_Load(object sender, EventArgs e)
        {
            Employee employee = account.GetEmployee(db);
            textBox8.Text = textBox4.Text = employee.UniqueNumber;
            textBox1.Text = account.Role;
            comboBox3.DataSource = new List<string>() { "Nam", "Nữ" };
            comboBox3.SelectedItem = employee.Gender;
            textBox3.Text = employee.Phone;
            textBox7.Text = employee.Position;
            dateTimePicker1.Value = employee.DoB;
            textBox6.Text = employee.Address;
            label3.Text = employee.Name;
        }

        private void btnChange_Click(object sender, EventArgs e)
        {
            if (textBox5.Text.Length == 0 || textBox2.Text.Length == 0 || textBox9.Text.Length == 0)
            {
                MessageBox.Show("Mật khẩu không được để trống!");
                return;
            }
            if (textBox5.Text.HashSHA256().Equals(account.Password))
            {
                if (textBox2.Text.Equals(textBox9.Text))
                {
                    account.Password = textBox9.Text.HashSHA256();
                    account.PasswordUpdatedAt = DateTime.Now;
                    if (db.UpdateRow(account))
                    {
                        MessageBox.Show("Đổi mật khẩu thành công!", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        textBox2.Clear();
                        textBox5.Clear();
                        textBox9.Clear();
                    }
                    else
                    {
                        MessageBox.Show("Lỗi kết nối!", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Xác nhận mật khẩu không trùng khớp với mật khẩu mới!", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
