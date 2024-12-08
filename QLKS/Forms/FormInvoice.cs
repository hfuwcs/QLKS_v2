using System.Linq;
using System.Windows.Forms;

namespace QLKS.Forms
{
    public partial class FormInvoice : Form
    {
        DbContext db = new DbContext(DbContext.ConnectionType.ConfigurationManager, "DefaultConnection");

        public FormInvoice()
        {
            InitializeComponent();
            dataView.AutoGenerateColumns = false;
            dataView.DataSource = ViewModels.InvoiceViewModel.GetInvoices(db).ToList();
        }
        private int selectedRowIndex = -1;
        int maHD = 0;

        private void dataView_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (e.RowIndex >= 0)
                {
                    selectedRowIndex = e.RowIndex;
                    dataView.ClearSelection();
                    dataView.Rows[e.RowIndex].Selected = true;
                    contextMenuStrip1.Show(dataView, dataView.PointToClient(MousePosition));
                    maHD = int.Parse(dataView.Rows[e.RowIndex].Cells[0].Value.ToString());
                }
            }
        }

        private void inHóaĐơnToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            if (selectedRowIndex >= 0)
            {
                if (MessageBox.Show("Bạn có muốn in hóa đơn này", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    FormPrint formPrint = new FormPrint(maHD);
                    formPrint.ShowDialog();
                }
            }
        }
    }
}
