using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using QLKS.Models;
namespace QLKS.Forms
{
    public partial class BackUpAndRestore : Form
    {
        static DbContext db = new DbContext(DbContext.ConnectionType.ConfigurationManager, "DefaultConnection");
        Account account;
        public BackUpAndRestore(Account account)
        {
            InitializeComponent();
            this.account = account;
        }

        private void BackUpAndRestore_Load(object sender, EventArgs e)
        {
            rdFull.Checked = true;
            cbWithReplace.Checked = true;
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void btnPath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if(fbd.ShowDialog() == DialogResult.OK)
            {
                txtPath.Text = fbd.SelectedPath;
            }
        }

        private void btnBackUp_Click(object sender, EventArgs e)
        {
            string date = DateTime.Now.ToString("ddMMyyyy_HHmmss");
            string dbname = db.Connection.Database;
            if (String.IsNullOrEmpty(txtPath.Text))
            {
                MessageBox.Show("Vui lòng chọn đường dẫn lưu file back up", "Thông báo!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            //if (!txtPassword.Text.HashSHA256().Equals(account.Password))
            //{
            //    MessageBox.Show("Sai mật khẩu, không thể sao lưu!", "Thông báo!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    return;
            //}
            string path = txtPath.Text;
            if (MessageBox.Show("Bạn có chắc chắn muốn sao lưu dữ liệu không?", "Thông báo!", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                return;
            }
            if (rdFull.Checked == true) {
                try
                {
                    string sqlBackup = $"BACKUP DATABASE {dbname} TO DISK = '{path}\\{dbname}_{date}_full.bak' WITH INIT";
                    int Afected = db.ExecuteNonQuery(sqlBackup);
                    if (Afected != 0)
                    {
                        MessageBox.Show("Sao lưu dữ liệu thành công", "Thông báo!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Sao lưu dữ liệu thất bại", "Thông báo!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Thông báo!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (rdDiff.Checked == true)
            {
                try
                {
                    string sqlBackup = $"BACKUP DATABASE {dbname} TO DISK = '{path}\\{dbname}_{date}_diff.bak' WITH DIFFERENTIAL";
                    int Afected = db.ExecuteNonQuery(sqlBackup);
                    if (Afected != 0)
                    {
                        MessageBox.Show("Sao lưu dữ liệu thành công", "Thông báo!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Sao lưu dữ liệu thất bại", "Thông báo!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Thông báo!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (rdLog.Checked == true)
            {
                try
                {
                    string sqlBackup = $"BACKUP LOG {dbname} TO DISK = '{path}\\{dbname}_{date}_log.trn'";
                    int Afected = db.ExecuteNonQuery(sqlBackup);
                    if (Afected != 0)
                    {
                        MessageBox.Show("Sao lưu dữ liệu thành công", "Thông báo!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Sao lưu dữ liệu thất bại", "Thông báo!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Thông báo!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        List<string> listFilePath = new List<string>();
        private void btnPathRestore_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            ofd.Filter = "Backup files (*.bak;*.trn)|*.bak;*.trn";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                txtPathRestore.Text = Path.GetDirectoryName(ofd.FileName);
                listFilePath = ofd.FileNames.ToList();
                dgrFile.DataSource = ofd.FileNames.Select(x => new { FileName = Path.GetFileName(x) }).ToList();
            }
        }

        private void btnRestore_Click(object sender, EventArgs e)
        {
            string dbname = db.Connection.Database;
            string date = DateTime.Now.ToString("ddMMyyyy_HHmmss");

            if (String.IsNullOrEmpty(txtPathRestore.Text))
            {
                MessageBox.Show("Vui lòng chọn file dữ liệu cần phục hồi", "Thông báo!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!txtPasswordRe.Text.HashSHA256().Equals(account.Password))
            {
                MessageBox.Show("Sai mật khẩu, không thể phục hồi dữ liệu!", "Thông báo!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string path = txtPathRestore.Text;
            if (MessageBox.Show("Bạn có chắc chắn muốn phục hồi dữ liệu không?", "Thông báo!", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                return;
            }
            string OptionReplace = "WITH REPLACE";
            string OptionNoRecovery = "WITH NORECOVERY";
            string setSingleUser = $"ALTER DATABASE {dbname} SET SINGLE_USER WITH ROLLBACK IMMEDIATE";
            string setMultiUser = $"ALTER DATABASE {dbname} SET MULTI_USER";

            if (cbWithReplace.Checked == true)
            {
                if (db.ExecuteNonQuery(setSingleUser) != 0)
                {
                    int Afected = db.ExecuteNonQuery($"use master RESTORE DATABASE {dbname} FROM DISK = '{listFilePath[0]}' {OptionReplace}");
                    if (Afected!= 0)
                    {
                        db.ExecuteNonQuery(setMultiUser);
                        MessageBox.Show("Phục hồi dữ liệu thành công", "Thông báo!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Phục hồi dữ liệu thất bại", "Thông báo!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                db.ExecuteNonQuery(setMultiUser);
                return;
            }
            else
            {
                db.ExecuteNonQuery(setMultiUser);
            }

            if (cbFullRecovery.Checked == true)
            {
                if (db.ExecuteNonQuery(setSingleUser) != 0)
                {
                    //Backup logtail trước khi restore lại database
                    string backupTaillog = $"use master BACKUP LOG {dbname} TO DISK = '{path}\\{dbname}_{date}_logtail.trn' {OptionNoRecovery}";
                    string backupTaillogPath = $"{path}\\{dbname}_{date}_logtail.trn";
                    int Afected = db.ExecuteNonQuery(backupTaillog);
                    if (Afected != 0)
                    {
                        foreach(var filePath in listFilePath)
                        {
                            try
                            {
                                int ck =  db.ExecuteNonQuery($"USE master RESTORE DATABASE {dbname} FROM DISK = '{filePath}' WITH NORECOVERY");
                                if(ck==0)
                                {
                                    MessageBox.Show("Phục hồi dữ liệu thất bại", "Thông báo!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                                MessageBox.Show("Phục hồi dữ liệu thất bại", "Thông báo!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        db.ExecuteNonQuery($"RESTORE LOG {dbname} FROM DISK = '{backupTaillogPath}' WITH RECOVERY");
                        
                        MessageBox.Show("Phục hồi dữ liệu thành công", "Thông báo!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Phục hồi dữ liệu thất bại", "Thông báo!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                db.ExecuteNonQuery(setMultiUser);
                return;
            }
            else
            {
                db.ExecuteNonQuery(setMultiUser);
            }

        }

        private void dgrFile_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void cbWithReplace_CheckedChanged(object sender, EventArgs e)
        {
            if(cbWithReplace.Checked == true)
            {
                cbFullRecovery.Enabled = false;
                cbFullRe.Checked = true;
                cbFullRe.Enabled = false;
                cbDiffRe.Checked = false;
                cbDiffRe.Enabled = false;
                cbLogRe.Checked = false;
                cbLogRe.Enabled = false;
            }
            else
            {
                cbFullRecovery.Enabled = true;
                cbFullRe.Enabled = true;
                cbDiffRe.Enabled = true;
                cbLogRe.Enabled = true;
            }
        }

        private void cbFullRecovery_CheckedChanged(object sender, EventArgs e)
        {
            if(cbFullRecovery.Checked == true)
            {
                cbWithReplace.Enabled = false;
                cbFullRe.Checked = true;
                cbFullRe.Enabled = false;
                cbDiffRe.Checked = true;
                cbDiffRe.Enabled = false;
                cbLogRe.Checked = true;
                cbLogRe.Enabled = false;
            }
            else
            {
                cbWithReplace.Enabled = true;
                cbFullRe.Checked = true;
                cbFullRe.Enabled = true;
                cbDiffRe.Checked = false;
                cbDiffRe.Enabled = true;
                cbLogRe.Checked = false;
                cbLogRe.Enabled = true;
            }
        }
    }
}
