using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace CourseService {
	[DataContract]
	class CourseNotExistsFault {
		[DataMember]
		public string Message;
	}

	[DataContract]
	class StudentNotExistsFault {
		[DataMember]
		public string Message;
	}

	[DataContract]
	class CourseFullFault {
		[DataMember]
		public string Message;
	}

	[DataContract]
	class AlreadyRegisteredFault {
		[DataMember]
		public string Message;
	}

	[DataContract]
	class AlreadyUnregisteredFault {
		[DataMember]
		public string Message;
	}

	[DataContract]
	class PrerequisiteOfAnotherFault {
		[DataMember]
		public string Message;
	}

	[DataContract]
	class PrerequisiteNotMetFault {
		[DataMember]
		public string Message;
	}

	[DataContract]
	class TakenTooManyCoursesFault {
		[DataMember]
		public string Message;
	}
}
