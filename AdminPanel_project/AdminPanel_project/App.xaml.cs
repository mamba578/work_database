using System.Configuration;
using System.Data;
using System.Net.Http;
using System.Windows;

namespace AdminPanel_project
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static readonly HttpClient client = new HttpClient();

    }

}
