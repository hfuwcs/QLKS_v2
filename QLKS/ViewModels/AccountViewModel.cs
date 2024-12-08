using QLKS.Models;
using QLKS.Models.Extensions;
using System;
using System.Collections.Generic;

namespace QLKS.ViewModels
{
    public class AccountViewModel
    {
        Account account;
        Employee employee;

        public Account Account => account;
        public Employee Employee => employee;
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string UniqueNumber { get; set; }
        public string Role { get; set; }
        public bool IsActive { get; set; }

        public AccountViewModel(Account account, DbContext db)
        {
            this.account = account;
            employee = account.GetEmployee(db);
            Id = employee.Id;
            Name = employee.Name;
            Phone = employee.Phone;
            UniqueNumber = employee.UniqueNumber;
            Role = account.Role;
            IsActive = account.IsActive;
        }

        public static IEnumerable<AccountViewModel> GetAccounts(DbContext db, string whereCondition = "", params Type[] fromAddition)
        {
            foreach (Account a in db.GetTable<Account>(whereCondition, fromAddition: fromAddition))
            {
                yield return new AccountViewModel(a, db);
            }
        }
    }
}
