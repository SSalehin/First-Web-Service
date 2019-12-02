using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace CourseService {
	[ServiceContract]
	public interface ICourseService {
		[OperationContract]
		[FaultContract(typeof(StudentNotExistsFault))]
		[FaultContract(typeof(CourseNotExistsFault))]
		[FaultContract(typeof(CourseFullFault))]
		[FaultContract(typeof(AlreadyRegisteredFault))]
		[FaultContract(typeof(PrerequisiteNotMetFault))]
		[FaultContract(typeof(TakenTooManyCoursesFault))]
		void Register(int studentID, string courseID);

		[OperationContract]
		[FaultContract(typeof(StudentNotExistsFault))]
		[FaultContract(typeof(CourseNotExistsFault))]
		[FaultContract(typeof(PrerequisiteOfAnotherFault))]
		void Unregister(int studentID, string courseID);

		[OperationContract]
		[FaultContract(typeof(StudentNotExistsFault))]
		List<string> GetRegistrationList(int studentID);
	}
}
