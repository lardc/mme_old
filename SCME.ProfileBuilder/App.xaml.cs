using SCME.ProfileBuilder.Properties;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Threading;
using Application = System.Windows.Application;

namespace SCME.ProfileBuilder
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            //Подгрузка конфигурации
            UIServiceConfig.Settings.LoadSettings();
            //Проверка наличия файла параметров блоков
            if (!File.Exists("SCME.ParamsConfig.xml"))
            {
                MessageBox.Show("Файл с параметрами блоков не существует", "Ошибка");
                Current.Shutdown(1);
            }
            Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
            Thread.CurrentThread.CurrentCulture = new CultureInfo(Settings.Default.CurrentCulture);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(Settings.Default.CurrentCulture);
            FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentUICulture.IetfLanguageTag)));
        }

        private static void Current_DispatcherUnhandledException(object Sender, DispatcherUnhandledExceptionEventArgs E)
        {
            File.WriteAllText("error.txt", E.Exception.ToString());
        }
    }
}
