using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Icarus_Drone_Service_Software
{
	class Drone
	{
		// properties
		private string clientName;
		private string droneModel;
		private string serviceProblem;
		private float serviceCost;
		private int serviceTag;
		// contructor
		public Drone(string clientName, string droneModel, string serviceProblem, float serviceCost, int serviceTag)
		{
			this.clientName = clientName;
			this.droneModel = droneModel;
			this.serviceProblem = serviceProblem;
			this.serviceCost = serviceCost;
			this.serviceTag = serviceTag;
		}
		// methods
		public string GetDetails()
		{
			return $"Client Name: {clientName}\nService Cost: {serviceCost}\nService Tag: {serviceTag}";
		}

		public string GetClientName()
		{
			return clientName;
		}
		public string GetDroneModel()
		{
			return droneModel;
		}
		public string GetServiceProblem()
		{
			return serviceProblem;
		}
		public float GetServiceCost()
		{
			return serviceCost;
		}
		public int GetServiceTag()
		{
			return serviceTag;
		}
	}
}
