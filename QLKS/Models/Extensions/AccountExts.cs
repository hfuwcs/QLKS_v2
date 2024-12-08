using System.Linq;

namespace QLKS.Models.Extensions
{
    public static class AccountExts
    {
        public static Employee GetEmployee(this Account account, DbContext context)
        {
            return context.GetTable<Employee>(x => x.Id == account.Employee).FirstOrDefault();
        }
    }
}
