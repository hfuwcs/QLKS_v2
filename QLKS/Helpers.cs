using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace QLKS
{
    public static class Helpers
    {
        public static readonly List<string> Countries = new List<string>
        {
            "Afghanistan", "Ai Cập", "Albania", "Algérie", "Andorra", "Angola", "Antigua và Barbuda",
            "Áo", "Argentina", "Armenia", "Azerbaijan", "Ấn Độ", "Ba Lan", "Bahamas", "Bahrain",
            "Bangladesh", "Barbados", "Belarus", "Belize", "Bénin", "Bhutan", "Bỉ", "Bồ Đào Nha",
            "Bolivia", "Bosna và Hercegovina", "Botswana", "Brazil", "Brunei", "Bulgaria", "Burkina Faso",
            "Burundi", "Cabo Verde", "Các Tiểu vương quốc Ả Rập Thống nhất", "Cameroon", "Campuchia",
            "Canada", "Chile", "Colombia", "Comoros", "Cộng hòa Congo", "Cộng hòa Dân chủ Congo",
            "Cộng hòa Séc", "Costa Rica", "Croatia", "Cuba", "Djibouti", "Dominica", "Đan Mạch",
            "Đông Timor", "Ecuador", "El Salvador", "Eritrea", "Estonia", "Eswatini", "Ethiopia",
            "Fiji", "Gabon", "Gambia", "Ghana", "Grenada", "Gruzia", "Guatemala", "Guinea",
            "Guinea-Bissau", "Guyana", "Hà Lan", "Haiti", "Hàn Quốc", "Hoa Kỳ", "Honduras",
            "Hungary", "Hy Lạp", "Iceland", "Indonesia", "Iran", "Iraq", "Ireland", "Israel", "Italia",
            "Jamaica", "Jordan", "Kazakhstan", "Kenya", "Kiribati", "Kosovo", "Kuwait", "Kyrgyzstan",
            "Lào", "Latvia", "Lesotho", "Liban", "Liberia", "Libya", "Liechtenstein", "Litva",
            "Luxembourg", "Madagascar", "Malawi", "Malaysia", "Maldives", "Mali", "Malta", "Maroc",
            "Marshall", "Mauritania", "Mauritius", "Mexico", "Micronesia", "Moldova", "Monaco", "Mông Cổ",
            "Montenegro", "Mozambique", "Myanmar", "Namibia", "Nam Phi", "Nam Sudan", "Na Uy",
            "Nepal", "New Zealand", "Nicaragua", "Niger", "Nigeria", "Nga", "Nhật Bản", "Oman",
            "Pakistan", "Palau", "Panama", "Papua New Guinea", "Paraguay", "Peru", "Pháp", "Philippines",
            "Phần Lan", "Qatar", "Quần đảo Solomon", "Romania", "Rwanda", "Saint Kitts và Nevis",
            "Saint Lucia", "Saint Vincent và Grenadines", "Samoa", "San Marino", "São Tomé và Príncipe",
            "Séc", "Sénégal", "Serbia", "Seychelles", "Sierra Leone", "Singapore", "Síp",
            "Slovakia", "Slovenia", "Somalia", "Sri Lanka", "Sudan", "Suriname", "Syria",
            "Tajikistan", "Tanzania", "Tây Ban Nha", "Thái Lan", "Thành Vatican", "Thổ Nhĩ Kỳ",
            "Thụy Điển", "Thụy Sĩ", "Togo", "Tonga", "Trinidad và Tobago", "Trung Quốc", "Tunisia",
            "Turkmenistan", "Tuvalu", "Úc", "Uganda", "Ukraine", "Uruguay", "Uzbekistan",
            "Vanuatu", "Venezuela", "Việt Nam", "Ý", "Yemen", "Zambia", "Zimbabwe"
        };
        public static void ClearControl(Control control)
        {
            foreach (Control control1 in control.Controls)
            {
                if (control1 is TextBox)
                {
                    TextBox textBox = (TextBox)control1;
                    textBox.Clear();
                }
                else if (control1 is ComboBox)
                {
                    ComboBox comboBox = (ComboBox)control1;
                    comboBox.SelectedIndex = -1;
                    comboBox.Text = "";
                }
                else if (control1 is DateTimePicker)
                {
                    DateTimePicker dateTimePicker = (DateTimePicker)control1;
                    dateTimePicker.Value = DateTime.Now;
                }
                else if (control is DataGridView)
                {
                    DataGridView dataGridView = (DataGridView)control1;
                    dataGridView.Rows.Clear();
                }
            }
        }

        public static string HashSHA256(this string input)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                    builder.Append(bytes[i].ToString("x2"));
                return builder.ToString();
            }
        }
    }
}
