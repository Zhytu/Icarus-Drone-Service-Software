using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Icarus_Drone_Service_Software
{
	// Programming Criteria 6.1
	class Drone
	{
		// properties
		public string clientName;
		private string droneModel;
		private string serviceProblem;
		private double serviceCost;
		private int serviceTag;


		// public, read-only properties for XAML binding and data access
		public string ClientName => char.ToUpper(clientName[0]) + clientName.Substring(1);
		public string DroneModel => droneModel;
		public string ServiceProblem => serviceProblem;
		public double ServiceCost => serviceCost;
		public int ServiceTag => serviceTag;


		// contructor
		public Drone(string clientName, string droneModel, string serviceProblem, double serviceCost, int serviceTag)
		{
			this.clientName = clientName;
			this.droneModel = droneModel;
			this.serviceProblem = serviceProblem;
			this.serviceCost = serviceCost;
			this.serviceTag = serviceTag;
		}
		// methods
		public override string ToString()
		{
			return $"Name: {char.ToUpper(clientName[0]) + clientName.Substring(1)}, Cost: ${serviceCost:F2}";
		}

		public string GetClientName()
		{
			// capitalise the first letter of the client name for display purposes
			return char.ToUpper(clientName[0]) + clientName.Substring(1);
		}
		public string GetDroneModel()
		{
			return droneModel;
		}
		public string GetServiceProblem()
		{
			return ToSentenceCase(serviceProblem);

		}
		public double GetServiceCost()
		{
			return serviceCost;
		}
		public void SetServiceCost(double cost)
		{
			serviceCost = cost;
		}
		public int GetServiceTag()
		{
			return serviceTag;
		}

		//helper method

		public static string ToSentenceCase(string input)
		{
			if (string.IsNullOrWhiteSpace(input))
				return input;

			// Trim any accidental leading or trailing whitespaces
			input = input.Trim();

			// 1. Capitalize the first letter
			string formatted = char.ToUpper(input[0]) + input.Substring(1);

			// 2. Add a period if it doesn't end with ., !, or ?
			if (!formatted.EndsWith(".") && !formatted.EndsWith("!") && !formatted.EndsWith("?"))
			{
				formatted += ".";
			}

			return formatted;
		}
	}
}
