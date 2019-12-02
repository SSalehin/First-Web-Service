using System;
using System.ServiceModel;

namespace CourseServiceHost {
	class Program {
		static void Main(string[] args) {
			using(ServiceHost host = new ServiceHost(typeof(CourseService.CourseService))){
				host.Open();
				Console.WriteLine("Service hosted...");
				Console.ReadLine();
			}
		}
	}
}
