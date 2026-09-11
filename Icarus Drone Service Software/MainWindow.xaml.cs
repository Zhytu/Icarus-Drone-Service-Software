using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
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
		Queue<Drone> RegularService = new Queue<Drone>();
		Queue<Drone> ExpressService = new Queue<Drone>();

		List<Drone> FinishedList = new List<Drone>();

		int currentServiceTag;

		public MainWindow()
        {
            InitializeComponent();
		}

		// Button Methods

		private void AddNewItem(object sender, RoutedEventArgs e)
		{
			if (CheckEntryValidity() == true)
			{
				Drone drone = new Drone(TbxClientName.Text, TbxDroneModel.Text, TbxServiceProblem.Text, double.Parse(TbxServiceCost.Text), int.Parse(IudServiceTag.Text));

				// Increment the service tag for the next drone
				IncrementTag();


				if (GetServicePriority() == true)
				{
					// express

					// add express service surcharge to the service cost
					// add the drone to the express service queue
					drone.SetServiceCost(drone.GetServiceCost() * 1.15);
					ExpressService.Enqueue(drone);

					//refresh listview
					DisplayServiceQueue(ExpressService, LvwExpress);

					SbrStatus.Items.Clear();
					SbrStatus.Items.Add($"Item {drone.GetServiceTag()} added to express queue.");
				}
				else
				{
					// regular

					// add the drone to the regular service queue
					RegularService.Enqueue(drone);

					//refresh listview
					DisplayServiceQueue(RegularService, LvwRegular);

					SbrStatus.Items.Clear();
					SbrStatus.Items.Add($"Item {drone.GetServiceTag()} added to standard queue.");
				}

				// DEBUG: Output the details of the added drone to the debug console
				Debug.WriteLine($"Added new drone for {drone.GetClientName()} to {(RbtExpress.IsChecked == true ? "Express" : "Regular")} Service.");

				// Clear the input fields
				ClearInputFields();
			}
		}

		// UI Helper Methods

		private void ClearInputFields()
		{
			// This function empties the input fields on the left side of the program.

			TbxClientName.Clear();
			TbxDroneModel.Clear();
			TbxServiceProblem.Clear();
			TbxServiceCost.Clear();
		}

		private void IncrementTag()
		{
			// This function increments the service tag by the predefined increment value (10). It also keeps the global service tag value updated.
			
			IudServiceTag.Value += IudServiceTag.Increment;
			currentServiceTag = (int)IudServiceTag.Value; // update global service tag variable
		}

		private void DisplayServiceQueue(Queue<Drone> serviceQueue, ListView listView)
		{
			// This function clears a listview, then displays a queues contents within it.

			listView.Items.Clear();
			foreach (Drone drone in serviceQueue)
			{
				listView.Items.Add(drone);
			}
		}

		private bool GetServicePriority()
		{
			// This function returns a bool value depending on what state the radio buttons are in.
			// True = express, False = regular

			if (RbtExpress.IsChecked == true)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		private bool CheckEntryValidity()
		{
			// This function 
			SbrStatus.Items.Clear();
			string errorMessage = "";
			if (string.IsNullOrWhiteSpace(TbxClientName.Text))
			{
				errorMessage = "Client Name is required.";
				SbrStatus.Items.Add(errorMessage);
			}
			if (string.IsNullOrWhiteSpace(TbxDroneModel.Text))
			{
				errorMessage = "Drone Model is required.";
				SbrStatus.Items.Add(errorMessage);
			}
			if (string.IsNullOrWhiteSpace(TbxServiceProblem.Text))
			{
				errorMessage = "Service Problem is required.";
				SbrStatus.Items.Add(errorMessage);
			}
			if (string.IsNullOrWhiteSpace(TbxServiceCost.Text))
			{
				errorMessage = "Service Cost is required.";
				SbrStatus.Items.Add(errorMessage);
			}
			// check that service cost is a valid double with two decimal places
			if (!double.TryParse(TbxServiceCost.Text, out double serviceCost) || serviceCost < 0)
			{
				errorMessage = "Service Cost must be a valid positive number.";
				SbrStatus.Items.Add(errorMessage);
			}
			else if (Math.Abs(serviceCost - Math.Round(serviceCost, 2)) > 0.000001)
			{
				errorMessage = "Service Cost must have at most two decimal places.";
				SbrStatus.Items.Add(errorMessage);
			}
			if (errorMessage != "")
			{
				return false;
			}
			else
			{
				return true;
			}
			
		}

		private void LvwRegular_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			
			// Highlight the selected drone in the listview and display its details in the fill in fields, disable fields until no item is selected
			if (LvwRegular.SelectedItem is Drone selectedDrone)
			{
				// deselect express listview
				LvwExpress.SelectedItem = null;

				TbxClientName.Text = selectedDrone.GetClientName();
				TbxDroneModel.Text = selectedDrone.GetDroneModel();
				TbxServiceProblem.Text = selectedDrone.GetServiceProblem();
				TbxServiceCost.Text = selectedDrone.GetServiceCost().ToString("F2");
				IudServiceTag.Value = selectedDrone.GetServiceTag();
				// Disable the input fields
				TbxClientName.IsEnabled = false;
				TbxDroneModel.IsEnabled = false;
				TbxServiceProblem.IsEnabled = false;
				TbxServiceCost.IsEnabled = false;
				IudServiceTag.IsEnabled = false;
				RbtExpress.IsEnabled = false;
				RbtRegular.IsEnabled = false;
				BtnSubmit.IsEnabled = false;

				// status strip message
				SbrStatus.Items.Clear();
				SbrStatus.Items.Add($"Selected Drone {selectedDrone.GetServiceTag()}. (Ctrl+Click to deselect)");

			}
			else
			{
				if (LvwRegular.SelectedItem == null && LvwExpress.SelectedItem == null)
				{
					// Clear the input fields and enable them if no item is selected
					ClearInputFields();
					TbxClientName.IsEnabled = true;
					TbxDroneModel.IsEnabled = true;
					TbxServiceProblem.IsEnabled = true;
					TbxServiceCost.IsEnabled = true;
					IudServiceTag.IsEnabled = true;
					IudServiceTag.Value = currentServiceTag;
					RbtExpress.IsEnabled = true;
					RbtRegular.IsEnabled = true;
					BtnSubmit.IsEnabled = true;

					SbrStatus.Items.Clear();
				}
				
			}
		}

		private void LvwExpress_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			
			// Highlight the selected drone in the listview and display its details in the fill in fields, disable fields until no item is selected
			if (LvwExpress.SelectedItem is Drone selectedDrone)
			{
				// deselect regular listview
				LvwRegular.SelectedItem = null;

				TbxClientName.Text = selectedDrone.GetClientName();
				TbxDroneModel.Text = selectedDrone.GetDroneModel();
				TbxServiceProblem.Text = selectedDrone.GetServiceProblem();
				TbxServiceCost.Text = selectedDrone.GetServiceCost().ToString("F2");
				IudServiceTag.Value = selectedDrone.GetServiceTag();
				// Disable the input fields
				TbxClientName.IsEnabled = false;
				TbxDroneModel.IsEnabled = false;
				TbxServiceProblem.IsEnabled = false;
				TbxServiceCost.IsEnabled = false;
				IudServiceTag.IsEnabled = false;
				RbtExpress.IsEnabled = false;
				RbtRegular.IsEnabled = false;
				BtnSubmit.IsEnabled = false;
				// status strip message
				SbrStatus.Items.Clear();
				SbrStatus.Items.Add($"Selected Drone: {selectedDrone.GetClientName()} - Service Tag: {selectedDrone.GetServiceTag()} (Ctrl+Click to deselect)");
			}
			else
			{
				if (LvwExpress.SelectedItem == null && LvwRegular.SelectedItem == null)
				{
					// Clear the input fields and enable them if no item is selected
					ClearInputFields();
					TbxClientName.IsEnabled = true;
					TbxDroneModel.IsEnabled = true;
					TbxServiceProblem.IsEnabled = true;
					TbxServiceCost.IsEnabled = true;
					IudServiceTag.IsEnabled = true;
					IudServiceTag.Value = currentServiceTag;
					RbtExpress.IsEnabled = true;
					RbtRegular.IsEnabled = true;
					BtnSubmit.IsEnabled = true;

					SbrStatus.Items.Clear();
				}
			}
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			//remove the oldest drone from the regular service queue and add it to the finished list
			Drone added = RegularService.Dequeue();

			

			FinishedList.Add(added);
			LbxFinished.Items.Add(added);
			DisplayServiceQueue(RegularService, LvwRegular);


			SbrStatus.Items.Clear();
			SbrStatus.Items.Add($"Item {added.GetServiceTag()} deqeued from standard queue.");

		}

		private void BtnProcessExpress_Click(object sender, RoutedEventArgs e)
		{
			//remove the oldest drone from the express service queue and add it to the finished list
			Drone added = ExpressService.Dequeue();

			

			FinishedList.Add(added);
			LbxFinished.Items.Add(added);
			DisplayServiceQueue(ExpressService, LvwExpress);

			SbrStatus.Items.Clear();
			SbrStatus.Items.Add($"Item {added.GetServiceTag()} deqeued from express queue.");
		}

		private void LbxFinished_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			Drone removed = (Drone)LbxFinished.SelectedItem;
			LbxFinished.Items.Remove(removed);
			FinishedList.Remove(removed); 
		}
	}
}