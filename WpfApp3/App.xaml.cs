using System.Windows;
using WpfApp3.Core.ViewModels;
using WpfApp3.Core;

namespace WpfApp3
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var repo = new NoteRepository(); 
            var viewModel = new MainViewModel(repo); 
            var window = new MainWindow { DataContext = viewModel };
            window.Show();
        }
    }
}