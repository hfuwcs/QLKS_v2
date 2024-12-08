using QLKS.Forms;
using QLKS.Models;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace QLKS
{
    public partial class FormMainMenu : Form
    {
        Account account;

        private Button currentButton;
        private Random random;
        private int tempIndex;
        private Form activeForm;

        public bool Logout = false;

        public FormMainMenu(Account account)
        {
            InitializeComponent();
            random = new Random();
            this.Text = string.Empty;
            this.MaximizedBounds = Screen.FromHandle(this.Handle).WorkingArea;
            this.account = account;
            if (account.Role == "Tiếp tân")
            {
                button1.Visible = false;
                btnBackup.Visible = false;
            }
        }
        private Color SelectThemeColor()
        {
            //int index = random.Next(ThemeColor.ColorList.Count);
            ////while (tempIndex == index)
            ////{
            ////    index = random.Next(ThemeColor.ColorList.Count);
            ////}
            //tempIndex = index;
            //string color = ThemeColor.ColorList[tempIndex];
            //return ColorTranslator.FromHtml(color);
            return Color.SeaGreen;

        }
        private void ActivateButton(object btnSender)
        {
            if (btnSender != null)
            {
                if (currentButton != (Button)btnSender)
                {
                    {
                        DisableButton();
                        Color color = SelectThemeColor();
                        currentButton = (Button)btnSender;
                        currentButton.BackColor = color;
                        currentButton.ForeColor = Color.White;
                        currentButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                        //panel_TitleBar.BackColor = color;
                        //panel_Logo.BackColor = ThemeColor.ChangeColorBrightness(color, -0.3);
                    }
                }
            }
        }
        private void DisableButton()
        {
            foreach (Control previousBtn in panel_SideBar.Controls)
            {
                if (previousBtn is Button)
                {
                    previousBtn.BackColor = Color.FromArgb(51, 51, 76);
                    previousBtn.ForeColor = Color.Gainsboro;
                    previousBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                }
            }
        }

        private void OpenChildForm(Form childForm, object btnSender)
        {
            if (activeForm != null)
            {

                activeForm.Close();
            }
            ActivateButton(btnSender);
            activeForm = childForm;
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;
            this.panelMain.Controls.Add(childForm);
            this.panelMain.Tag = childForm;
            childForm.BringToFront();
            childForm.Show();
            //lbl_Title.Text = childForm.Text;
            btn_CloseChildForm.Visible = true;
            btn_CloseChildForm.Image = ((System.Drawing.Image)(Properties.Resources.exit));
        }
        public void OpenListRoom(Form childForm)
        {
            if (activeForm != null)
            {
                activeForm.Close();
            }
            activeForm = childForm;
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;
            this.panelMain.Controls.Add(childForm);
            this.panelMain.Tag = childForm;
            childForm.BringToFront();
            childForm.Show();
            //lbl_Title.Text = childForm.Text;
            btn_CloseChildForm.Visible = true;
        }
        private void btn_Task1_Click(object sender, EventArgs e)
        {
            OpenChildForm(new Forms.FormCustomer(), sender);
        }

        public void btn_CloseChildForm_Click(object sender, EventArgs e)
        {
            if (activeForm != null)
            {
                activeForm.Close();
                OpenListRoom(new FormRoomManagement());
            }
            Reset();
        }
        private void Reset()
        {
            DisableButton();
            lbl_Title.Text = "QUẢN LÝ KHÁCH SẠN";
            panel_TitleBar.BackColor = Color.FromArgb(0, 150, 136);
            panel_Logo.BackColor = Color.FromArgb(39, 39, 58);
            currentButton = null;
            btn_CloseChildForm.Text = "";
            btn_CloseChildForm.Image = ((System.Drawing.Image)(Properties.Resources.refresh));
            //btn_CloseChildForm.Visible = false;
        }

        private void btn_Task3_Click(object sender, EventArgs e)
        {
            OpenChildForm(new FormRoom(account), sender);
        }

        private void btn_Task1_Click_1(object sender, EventArgs e)
        {
            OpenChildForm(new FormReservation(), sender);
        }

        private void btn_Task4_Click(object sender, EventArgs e)
        {
            OpenChildForm(new FormService(account), sender);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenChildForm(new FormStaff(account), sender);
        }

        private void btn_Task5_Click(object sender, EventArgs e)
        {
            OpenChildForm(new FormUsingService(), sender);
        }

        private void FormMainMenu_Load(object sender, EventArgs e)
        {
            OpenListRoom(new FormRoomManagement());
        }

        private void btn_Task6_Click(object sender, EventArgs e)
        {
            OpenChildForm(new FormInformation(account), sender);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc muốn đăng xuất?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Logout = true;
                Close();
            }
        }

        private void btn_Task7_Click(object sender, EventArgs e)
        {
            OpenChildForm(new FormInvoice(), sender);
        }

        private void btnBackup_Click(object sender, EventArgs e)
        {
            OpenChildForm(new BackUpAndRestore(account), sender);
        }
    }
}
