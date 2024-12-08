using QLKS.Models;
using System.Collections.Generic;
using System.Linq;

namespace QLKS.ViewModels
{
    public class InvoiceViewModel
    {
        Invoice invoice;
        Customer customer;
        Employee employee;

        public Invoice Invoice => invoice;
        public Customer Customer => customer;
        public Employee Employee => employee;

        public int Id => invoice.Id;

        public string InvoiceDate => invoice.InvoiceDate.ToString("dd/MM/yyyy HH:mm");

        public string TotalMoney => string.Format("{0:C0}", invoice.TotalPrice);

        public string EmployeeName => employee.Name;

        public string CustomerName => customer.Name;

        public string CustomerPhone => customer.Phone;


        public InvoiceViewModel(Invoice invoice, DbContext db)
        {
            this.invoice = invoice;
            employee = db.GetTable<Employee>(x => x.Id == invoice.Employee).First();
            BookingRoom booking = db.GetTable<BookingRoom>(x => x.Id == invoice.BookingRoom).First();
            customer = db.GetTable<Customer>(x => x.Id == booking.Customer).First();
        }

        public static IEnumerable<InvoiceViewModel> GetInvoices(DbContext db)
        {
            foreach (Invoice i in db.GetTable<Invoice>())
            {
                yield return new InvoiceViewModel(i, db);
            }
        }
    }
}
