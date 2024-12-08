using QLKS.Forms;
using System;
using System.Windows.Forms;

namespace QLKS
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            System.Globalization.CultureInfo.DefaultThreadCurrentCulture = new System.Globalization.CultureInfo("vi-VN");
            bool logout = false;
            do
            {
                logout = false;
                FormLogin login = new FormLogin();
                Application.Run(login);
                if (login.Account != null)
                {
                    FormMainMenu main = new FormMainMenu(login.Account);
                    Application.Run(main);
                    logout = main.Logout;
                }
            } while (logout);
        }
    }
}
