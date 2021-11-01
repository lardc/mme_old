using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SCME.UI.CustomControl
{
    /// <summary>
    /// Логика взаимодействия для FaultDialog.xaml
    /// </summary>
    public partial class FaultDialog : Window
    {
        public FaultDialog()
        {
            InitializeComponent();
        }

        private void ButtonVAH_Click(object sender, RoutedEventArgs e)
        {
            SetFault("Несоответствующий вид ВАХ");
            Close();
        }

        private void ButtonMetal_Click(object sender, RoutedEventArgs e)
        {
            SetFault("Повреждение металлизации");
            Close();
        }

        private void SetFault(string reason)
        {
            string ConnString = @"Data Source=192.168.0.120\SCME; Initial Catalog=SCME_ResultsDB; User ID=sa; Password=Hpl1520";
            try
            {
                using (SqlConnection Connection = new SqlConnection(ConnString))
                using (SqlCommand Command = Connection.CreateCommand())
                {
                    Connection.Open();
                    Command.CommandText = "UPDATE DEVICES SET STATUS = 'Fault', REASON = @Reason WHERE DEV_ID = (SELECT MAX(DEV_ID) FROM DEVICES WHERE MME_CODE = @MmeCode)";
                    Command.Parameters.AddWithValue("Reason", reason);
                    Command.Parameters.AddWithValue("MmeCode", Cache.Main.VM.MmeCode);
                    Command.ExecuteNonQuery();
                }
            }
            catch { }
        }
    }
}
