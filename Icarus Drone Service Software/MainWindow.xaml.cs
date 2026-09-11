using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
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

		// User Interaction Methods

		private void AddNewItem(object sender, RoutedEventArgs e)
		{
			// This function triggers if the submit button is clicked.
			// It verifies that the fields are valid, then creates a new drone object, adds it to the relevant listview and queue, and displays a relevant status strip message.

			if (CheckEntryValidity() == true) // Check that there are no errors in the input fields.
			{
				// Create new drone object.
				Drone drone = new Drone(TbxClientName.Text, TbxDroneModel.Text, TbxServiceProblem.Text, double.Parse(TbxServiceCost.Text), int.Parse(IudServiceTag.Text));

				// Increment the service tag for the next drone.
				IncrementTag();


				if (GetServicePriority() == true) // Check express button is selected.
				{
					// Add express service surcharge to the service cost.
					drone.SetServiceCost(drone.GetServiceCost() * 1.15);
					// Add the drone to the express service queue
					ExpressService.Enqueue(drone);

					// Refresh the express listview.
					DisplayServiceQueue(ExpressService, LvwExpress);

					// Display a relevant status strip message.
					SbrStatus.Items.Clear();
					SbrStatus.Items.Add($"Item {drone.GetServiceTag()} added to express queue.");
				}
				else // If regular button is selected.
				{
					// Add the drone to the regular service queue.
					RegularService.Enqueue(drone);

					// Refresh the regular listview.
					DisplayServiceQueue(RegularService, LvwRegular);

					// Display a relevant status strip message
					SbrStatus.Items.Clear();
					SbrStatus.Items.Add($"Item {drone.GetServiceTag()} added to standard queue.");
				}

				// DEBUG: Output the details of the added drone to the debug console
				Debug.WriteLine($"Added new drone for {drone.GetClientName()} to {(RbtExpress.IsChecked == true ? "Express" : "Regular")} Service.");

				// Clear the input fields
				ClearInputFields();
			}
		}

		// Helper Methods

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
			// This function increments the service tag by the predefined increment value (10).
			// It also keeps the global service tag value updated.
			
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
			// This function runs several checks to see if the inputs fields contain errors.
			// If so, it reurns true and displays an appropriate error message. 

			SbrStatus.Items.Clear();
			string errorMessage = "";
			if (string.IsNullOrWhiteSpace(TbxClientName.Text)) // Check if client name field is empty.
			{
				errorMessage = "Client Name is required.";
				SbrStatus.Items.Add(errorMessage);
			}
			if (string.IsNullOrWhiteSpace(TbxDroneModel.Text)) // Check if drone model feild is empty.
			{
				errorMessage = "Drone Model is required.";
				SbrStatus.Items.Add(errorMessage);
			}
			if (string.IsNullOrWhiteSpace(TbxServiceProblem.Text)) // Check if service problem field is empty.
			{
				errorMessage = "Service Problem is required.";
				SbrStatus.Items.Add(errorMessage);
			}
			if (string.IsNullOrWhiteSpace(TbxServiceCost.Text)) // Check if service cost field is empty.
			{
				errorMessage = "Service Cost is required.";
				SbrStatus.Items.Add(errorMessage);
			}
			// check that service cost is a valid double with two decimal places
			if (!double.TryParse(TbxServiceCost.Text, out double serviceCost) || serviceCost < 0) // Check if service cost is a positive number.
			{
				errorMessage = "Service Cost must be a valid positive number.";
				SbrStatus.Items.Add(errorMessage);
			}
			else if (Math.Abs(serviceCost - Math.Round(serviceCost, 2)) > 0.000001) // Check if service cost has at most two decimal places.
			{
				errorMessage = "Service Cost must have at most two decimal places.";
				SbrStatus.Items.Add(errorMessage);
			}
			if (errorMessage != "") // If no errors are detected, return false.
			{
				return false;
			}
			else // If errors are detected, return true.
			{
				return true;
			}
			
		}

		private void LvwRegular_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			// This function triggers when the selection changes in the Regular list view.
			// It fills in the details from the selected item into the input fields on the left hand side, setting them to read only.
			// It checks if an item from the Express list view is selected. If so, it deselects it.
			// It also detects if an item is manually deselected, which empties the fields, and makes them writeable again.
			
			if (LvwRegular.SelectedItem is Drone selectedDrone) // Check if selected item is a drone. If so, creates selectedDrone with pattern matching.
			{
				// Set express listview selection to null.
				LvwExpress.SelectedItem = null;

				// Fill all input fields with drone variables.
				TbxClientName.Text = selectedDrone.GetClientName();
				TbxDroneModel.Text = selectedDrone.GetDroneModel();
				TbxServiceProblem.Text = selectedDrone.GetServiceProblem();
				TbxServiceCost.Text = selectedDrone.GetServiceCost().ToString("F2");
				IudServiceTag.Value = selectedDrone.GetServiceTag();

				// Disable the input fields.
				TbxClientName.IsEnabled = false;
				TbxDroneModel.IsEnabled = false;
				TbxServiceProblem.IsEnabled = false;
				TbxServiceCost.IsEnabled = false;
				IudServiceTag.IsEnabled = false;
				RbtExpress.IsEnabled = false;
				RbtRegular.IsEnabled = false;
				BtnSubmit.IsEnabled = false;

				// Display relevant status strip message.
				SbrStatus.Items.Clear();
				SbrStatus.Items.Add($"Selected Drone {selectedDrone.GetServiceTag()}. (Ctrl+Click to deselect)");

			}
			else
			{
				if (LvwRegular.SelectedItem == null && LvwExpress.SelectedItem == null) // Check if there is no selected item in either listview.
				{
					// Clear the input fields and enable them.
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

					// Remove status strip message.
					SbrStatus.Items.Clear();
				}
				
			}
		}

		private void LvwExpress_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			// This function triggers when the selection changes in the Express list view.
			// It fills in the details from the selected item into the input fields on the left hand side, setting them to read only.
			// It checks if an item from the Regular list view is selected. If so, it deselects it.
			// It also detects if an item is manually deselected, which empties the fields, and makes them writeable again.

			// Highlight the selected drone in the listview and display its details in the fill in fields, disable fields until no item is selected
			if (LvwExpress.SelectedItem is Drone selectedDrone)
			{
				// Set regular listview selection to null.
				LvwRegular.SelectedItem = null;

				// Fill all input fields with drone variables.
				TbxClientName.Text = selectedDrone.GetClientName();
				TbxDroneModel.Text = selectedDrone.GetDroneModel();
				TbxServiceProblem.Text = selectedDrone.GetServiceProblem();
				TbxServiceCost.Text = selectedDrone.GetServiceCost().ToString("F2");
				IudServiceTag.Value = selectedDrone.GetServiceTag();

				// Disable the input fields.
				TbxClientName.IsEnabled = false;
				TbxDroneModel.IsEnabled = false;
				TbxServiceProblem.IsEnabled = false;
				TbxServiceCost.IsEnabled = false;
				IudServiceTag.IsEnabled = false;
				RbtExpress.IsEnabled = false;
				RbtRegular.IsEnabled = false;
				BtnSubmit.IsEnabled = false;

				// Display relevant status strip message.
				SbrStatus.Items.Clear();
				SbrStatus.Items.Add($"Selected Drone: {selectedDrone.GetClientName()} - Service Tag: {selectedDrone.GetServiceTag()} (Ctrl+Click to deselect)");
			}
			else
			{
				if (LvwExpress.SelectedItem == null && LvwRegular.SelectedItem == null) // Check if there is no selected item in either listview.
				{
					// Clear the input fields and enable them.
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

					// Remove status strip message.
					SbrStatus.Items.Clear();
				}
			}
		}

		private void BtnProcessRegular_Click(object sender, RoutedEventArgs e)
		{
			// This function triggers when the Process button for the regular queue is clicked.
			// It removes the next item from the regular queue, and then places that item in the finished list.
			// It also refreshes the regular listview and adds the item to the listbox.

			// Remove the oldest drone from the regular service queue and turn it into a variable.
			Drone added = RegularService.Dequeue();

			// Add the item to the finished list.
			FinishedList.Add(added);
			// Add the item to the finished listbox.
			LbxFinished.Items.Add(added);
			// Refresh regular queue listview.
			DisplayServiceQueue(RegularService, LvwRegular);

			// Display a relevant message in the status strip
			SbrStatus.Items.Clear();
			SbrStatus.Items.Add($"Item {added.GetServiceTag()} deqeued from standard queue.");

		}

		private void BtnProcessExpress_Click(object sender, RoutedEventArgs e)
		{
			// This function triggers when the Process button for the express queue is clicked.
			// It removes the next item from the regular queue, and then places that item in the finished list.
			// It also refreshes the regular listview and adds the item to the listbox.

			// Remove the oldest drone from the regular service queue and turn it into a variable.
			Drone added = ExpressService.Dequeue();

			// Add the item to the finished list.
			FinishedList.Add(added);
			// Add the item to the finished listbox.
			LbxFinished.Items.Add(added);
			// Refresh regular queue listview.
			DisplayServiceQueue(ExpressService, LvwExpress);

			// Display a relevant message in the status strip
			SbrStatus.Items.Clear();
			SbrStatus.Items.Add($"Item {added.GetServiceTag()} deqeued from express queue.");
		}

		private void LbxFinished_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			// This function triggers when the finished listbox is double clicked.
			// It removes the selected item from the finished listbox and the corresponding item from the finished list.

			if (LbxFinished.SelectedItem != null) // Check if selected is not null
			{
				Drone removed = (Drone)LbxFinished.SelectedItem;
				LbxFinished.Items.Remove(removed);
				FinishedList.Remove(removed);
			}
		}

	}
}