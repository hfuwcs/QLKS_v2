using QLKS.Models;
using QLKS.Models.Extensions;
using QLKS.ViewModels;
using System;
using System.Linq;
using System.Windows.Forms;

namespace QLKS.Forms
{
    public partial class FormAccount : Form
    {
        static DbContext db = new DbContext(DbContext.ConnectionType.ConfigurationManager, "DefaultConnection");

        Account account;

        public FormAccount(Account account)
        {
            InitializeComponent();
            this.account = account;
        }

        private void FormAccount_Load(object sender, EventArgs e)
        {
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.SelectionChanged += DataGridView1_SelectionChanged;
            dataGridView1.DataSource = AccountViewModel.GetAccounts(db).Where(x => !x.UniqueNumber.Equals(this.account.GetEmployee(db).UniqueNumber)).ToList();
        }

        private void DataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
                return;
            AccountViewModel model = dataGridView1.SelectedRows[0].DataBoundItem as AccountViewModel;
            textBox11.Text = model.Employee.Id.ToString();
            textBox8.Text = model.Role;
            dateTimePicker2.Value = model.Account.CreatedAt;
            textBox2.Text = model.Name;
            textBox4.Text = model.UniqueNumber;
            comboBox3.Text = model.Employee.Gender;
            dateTimePicker1.Value = model.Employee.DoB;
            textBox3.Text = model.Phone;
            textBox6.Text = model.Employee.Address;
            textBox7.Text = model.Employee.Position;
            textBox11.Enabled = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
                return;
            AccountViewModel model = dataGridView1.SelectedRows[0].DataBoundItem as AccountViewModel;
            Account account = model.Account;
            account.Password = textBox5.Text.HashSHA256();
            account.PasswordUpdatedAt = DateTime.Now;
            account.Role = textBox8.Text;
            if (db.UpdateRow(account))
            {
                MessageBox.Show("Đổi mật khẩu thành công!", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                button5_Click(null, EventArgs.Empty);

            }
            else
            {
                MessageBox.Show("Đổi mật khẩu thất bại!", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
                return;
            AccountViewModel model = dataGridView1.SelectedRows[0].DataBoundItem as AccountViewModel;
            Account account = model.Account;
            if (MessageBox.Show($"Xoá tài khoản của nhân viên {model.Name} có mã là {model.Employee.Id}?", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (db.DeleteRow(account))
                {
                    MessageBox.Show("Xoá tài khoản thành công!", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    dataGridView1.DataSource = AccountViewModel.GetAccounts(db).Where(x => !x.UniqueNumber.Equals(this.account.GetEmployee(db).UniqueNumber)).ToList();
                }
                else
                {
                    MessageBox.Show("Xoá tài khoản thất bại!", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            dataGridView1.ClearSelection();
            textBox11.Clear();
            textBox8.Clear();
            dateTimePicker2.Value = DateTime.Now;
            dateTimePicker1.Value = DateTime.Now;
            textBox2.Clear();
            textBox4.Clear();
            textBox3.Clear();
            textBox6.Clear();
            textBox7.Clear();
            textBox11.Enabled = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Employee empl = db.GetTable<Employee>(x => x.Id.ToString() == textBox11.Text).FirstOrDefault();
            if (empl == null || empl.Id == 0)
            {
                return;
            }
            else
            {
                Account account = new Account()
                {
                    Employee = empl.Id,
                    CreatedAt = DateTime.Now,
                    IsActive = true,
                    PasswordUpdatedAt = DateTime.Now,
                    Role = textBox8.Text,
                    Password = textBox5.Text.HashSHA256()
                };
                if (db.AddRow(account).Employee != 0)
                {
                    MessageBox.Show("Thêm tài khoản thành công!", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    dataGridView1.DataSource = AccountViewModel.GetAccounts(db).Where(x => !x.UniqueNumber.Equals(this.account.GetEmployee(db).UniqueNumber)).ToList();
                    button5_Click(null, EventArgs.Empty);

                }
                else
                {
                    MessageBox.Show("Thêm tài khoản thất bại!", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text.Length == 0)
            {
                dataGridView1.DataSource = AccountViewModel.GetAccounts(db).Where(x => !x.UniqueNumber.Equals(this.account.GetEmployee(db).UniqueNumber)).ToList();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length != 0)
            {
                dataGridView1.DataSource = AccountViewModel.GetAccounts(db, whereCondition: $"NHANVIEN.MANV = TAIKHOAN.MANV AND TENNV LIKE N'%{textBox1.Text}%'", fromAddition: typeof(Employee)).Where(x => !x.UniqueNumber.Equals(this.account.GetEmployee(db).UniqueNumber)).ToList();
            }
        }
    }
}
