using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace QLKS.Forms
{
    public partial class FormPrint : Form
    {
        DbContext db = new DbContext(DbContext.ConnectionType.ConfigurationManager, "DefaultConnection");

        public FormPrint(int invoiceId)
        {
            InitializeComponent();
            HotelCrystalReport hotelCrystalReport = new HotelCrystalReport();
            hotelCrystalReport.SetDatabaseLogon("sa_", "123");

            SqlDataAdapter adapter = new SqlDataAdapter($"Report", db.Connection);
            adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
            adapter.SelectCommand.Parameters.AddWithValue("@MAHD", invoiceId);
            DataSet data = new DataSet();
            adapter.Fill(data);
            db.Connection.Close();
            hotelCrystalReport.Database.Tables["Information"].SetDataSource(data.Tables[0]);
            hotelCrystalReport.Database.Tables["ListRooms"].SetDataSource(data.Tables[1]);
            hotelCrystalReport.Database.Tables["ListServices"].SetDataSource(data.Tables[2]);
            reportViewer.ReportSource = hotelCrystalReport;
            reportViewer.Refresh();
        }
    }
}
