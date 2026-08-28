using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Icarus_Drone_Service_Software
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

			Queue<Drone> RegularService = new Queue<Drone>();
			Queue<Drone> ExpressService = new Queue<Drone>();

			List<Drone> FinishedList = new List<Drone>();

		}
	}
}