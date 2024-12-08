using System;
using System.Linq;

namespace QLKS.Models.Extensions
{
    public static class EmployeeExts
    {
        public static Account GetAccount(string uniqueNumber, DbContext db)
        {
            Employee empl = db.GetTable<Employee>(x => x.UniqueNumber.Equals(uniqueNumber, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            if (empl != null)
            {
                Account acc = db.GetTable<Account>(x => x.Employee == empl.Id).FirstOrDefault();
                if (acc != null)
                    return acc;
            }
            return null;
        }
    }
}
