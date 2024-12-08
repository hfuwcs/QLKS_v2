using QLKS.Models;
using System;
using System.Linq;
using System.Windows.Forms;

namespace QLKS.Forms
{
    public partial class FormStaff : Form
    {
        static DbContext db = new DbContext(DbContext.ConnectionType.ConfigurationManager, "DefaultConnection");
        Account account;

        public FormStaff(Account account)
        {
            InitializeComponent();
            this.account = account;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            FormAccount formAccount = new FormAccount(account);
            formAccount.ShowDialog();
        }

        private void FormStaff_Load(object sender, EventArgs e)
        {
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.DataSource = db.GetTable<Employee>();
            dataGridView1.SelectionChanged += dataGridView1_SelectionChanged;
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 1)
            {
                Employee emp = dataGridView1.SelectedRows[0].DataBoundItem as Employee;
                textBox2.Text = emp.Name;
                dateTimePicker1.Value = emp.DoB;
                textBox4.Text = emp.UniqueNumber;
                textBox3.Text = emp.Phone;
                textBox7.Text = emp.Position;
                textBox6.Text = emp.Address;
                comboBox3.Text = emp.Gender;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length != 0)
            {
                dataGridView1.DataSource = db.GetTable<Employee>($"TENNV LIKE N'%{textBox1.Text}%'");
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text.Length == 0)
            {
                dataGridView1.DataSource = db.GetTable<Employee>();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                return;
            }
            int years = DateTime.Now.Year - dateTimePicker1.Value.Year;
            if (textBox2.Text.Length == 0 || !(years >= 18 && years <= 100) || textBox4.Text.Length != 12 || textBox3.Text.Length != 10 || textBox6.Text.Length == 0 || textBox7.Text.Length == 0)
                return;
            Employee employee = db.GetTable<Employee>($"MANV = {dataGridView1.SelectedRows[0].Cells["Column1"].Value}").First();
            employee.Name = textBox2.Text;
            employee.DoB = dateTimePicker1.Value;
            employee.UniqueNumber = textBox4.Text;
            employee.Phone = textBox3.Text;
            employee.Gender = comboBox3.SelectedItem.ToString();
            employee.Address = textBox6.Text;
            employee.Position = textBox7.Text;
            if (db.UpdateRow(employee))
            {
                MessageBox.Show("Cập nhật thông tin nhân viên thành công!", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                dataGridView1.DataSource = db.GetTable<Employee>();
            }
            else
            {
                MessageBox.Show("Cập nhật thông tin nhân viên thất bại!", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            dataGridView1.ClearSelection();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            textBox6.Clear();
            textBox7.Clear();
            dateTimePicker1.Value = DateTime.Now;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int years = DateTime.Now.Year - dateTimePicker1.Value.Year;
            if (textBox2.Text.Length == 0 || !(years >= 18 && years <= 100) || textBox4.Text.Length != 12 || textBox3.Text.Length != 10 || textBox6.Text.Length == 0 || textBox7.Text.Length == 0)
                return;
            Employee employee = new Employee();
            employee.Name = textBox2.Text;
            employee.DoB = dateTimePicker1.Value;
            employee.UniqueNumber = textBox4.Text;
            employee.Phone = textBox3.Text;
            employee.Gender = comboBox3.SelectedItem.ToString();
            employee.Address = textBox6.Text;
            employee.Position = textBox7.Text;
            employee.DoW = DateTime.Now;
            if (db.AddRow(employee).Id != 0)
            {
                MessageBox.Show("Thêm nhân viên thành công!", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                dataGridView1.DataSource = db.GetTable<Employee>();
            }
            else
            {
                MessageBox.Show("Thêm nhân viên thất bại!", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                return;
            }
            Employee employee = db.GetTable<Employee>($"MANV = {dataGridView1.SelectedRows[0].Cells["Column1"].Value}").First();
            if (MessageBox.Show($"Xoá nhân viên {employee.Name} có mã là {employee.Id}", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (db.DeleteRow(employee))
                {
                    MessageBox.Show("Xoá thành công!", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    dataGridView1.DataSource = db.GetTable<Employee>();
                }
                else
                {
                    MessageBox.Show("Xoá thất bại!", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
}
