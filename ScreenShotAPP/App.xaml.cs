using ScreenShotAPP.ViewModel;
using ScreenShotAPP.Views;
using System.Windows;

namespace ScreenShotAPP
{
    public partial class App : Application
    {
        MainWindow mw = new();

        protected override void OnStartup(StartupEventArgs e)
        {
            mw.DataContext = new MainViewModel();
            mw.Show();
        }
    }
}
