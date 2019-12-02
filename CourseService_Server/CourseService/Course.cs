using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace CourseService {
	class Course {
		public string id;
		public int maxStudents;
		public List<string> prerequisite;
		public List<string> corequisite;

		public Course(string id, int maxStudents = 3) {
			this.id = id;
			this.maxStudents = maxStudents;
			prerequisite = new List<string>();
			corequisite = new List<string>();
		}

		public Course(string id, List<string> prerequisites, List<string> corequisites, int maxStudents = 3) {
			this.id = id;
			this.maxStudents = maxStudents;
			prerequisite = prerequisites;
			corequisite = corequisites;
		}

		public void AddPrerequisite(string prerequisite) {
			if (prerequisite == "-" || prerequisite == " ") return;
			this.prerequisite.Add(prerequisite);
		}
		public void AddPrerequisite(string[] prerequisites) {
			if (prerequisites[0] == "-" || prerequisites[0] == " ") return;
			foreach (string course in prerequisites) this.prerequisite.Add(course);
		}

		public void AddCorequisite(string corequisite) {
			if (corequisite == "-" || corequisite == " ")
				this.corequisite.Add(corequisite);
		}
		public void AddCorequisite(string[] corequisites) {
			if (corequisite[0] == "-" || corequisites[0] == " ") return;
			foreach (string course in corequisites) this.corequisite.Add(course);
		}

		public static List<Course> GetCourseList(in string courseDescriptionFilePath) {
			string rawFileContent;
			using (StreamReader reader = new StreamReader(courseDescriptionFilePath)) rawFileContent = reader.ReadToEnd();
			string fileContent = rawFileContent.Split(new char[] { '#' })[0];
			List<Course> courses = fileContent
					.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries)
					.Select(line => {
						string[] parts = line.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
						string id = parts[0];
						List<string> prerequisites;
						if (parts[1] == "X") prerequisites = new List<string>();
						else prerequisites = parts[1]
											.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
											.ToList();
						List<string> corequisites;
						if (parts[2] == "X") corequisites = new List<string>();
						else corequisites = parts[2]
											.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
											.ToList();
						return new Course(id, prerequisites, corequisites);
					}).ToList();
			return courses;
		}

		public void print() {
			Console.WriteLine("ID: " + id);
			Console.Write("Prerequisite: ");
			foreach (string course in prerequisite) Console.Write(course + ",");
			Console.WriteLine("");
			Console.WriteLine("");
			Console.Write("Corequisite: ");
			foreach (string course in corequisite) Console.Write(course + ",");
			Console.WriteLine();
		}

	}


	class Student {
		public int id;
		public List<string> takenCourses;

		#region Constructor
		public Student(int id) {
			this.id = id;
			takenCourses = new List<string>();
		}

		public Student(int id, List<string> enrolledCourses) {
			this.id = id;
			this.takenCourses = new List<string>();
			foreach (string course in enrolledCourses) {
				if (course != "-" && course != " ") this.takenCourses.Add(course);
			}
		}
		#endregion

		public static List<Student> GetStudentList(in string studentDescriptionFilePath) {
			string rawFileContent;
			string fileContent;
			using (StreamReader reader = new StreamReader(studentDescriptionFilePath)) {
				rawFileContent = reader.ReadToEnd();
				fileContent = rawFileContent.Split(new char[] { '#' })[0];
			}
			List<Student> students = fileContent
					.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries)
					.Select(line => {
						string[] parts = line.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
						int id = int.Parse(parts[0]);
						List<string> enrolledCourses= parts[1].Split(new char[] { ';'}, StringSplitOptions.RemoveEmptyEntries).ToList();
						return new Student(id, enrolledCourses);
					}).ToList();
			return students;
		}

		public bool HasTaken(string courseID){
			return takenCourses.Where(course => course == courseID).Count() > 0;
		}

		public void Print() {
			Console.Write("ID:{0} ",id);
			Console.Write("Taken courses: ");
			foreach (string course in takenCourses) Console.Write(course + ",");
			Console.WriteLine();
		}

		public override string ToString() {
			string courseList;
			if (this.takenCourses.Count >= 1) {
				courseList = this.takenCourses
					.Aggregate((course1, course2) => course1 + ";" + course2);
			} else courseList = ";";
			string output = this.id.ToString() + ":" + courseList;
			return output;
		}
	}
}
//1000:BHV104;/1001:PRED356;MCS267;/1002:MATH167;BIO132;/1003:;/1004:BIO132;/1005:;/1006:PSYC201;MATH167;BIO132;/1007:;/1008:COMP167;/1009:;/1010:;/
