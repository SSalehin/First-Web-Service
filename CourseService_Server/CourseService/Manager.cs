using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceModel;

namespace CourseService {
	class Manager {
		private static Manager instance;
		public int maxCoursePerStudent;
		public readonly List<Course> courses;
		public List<Student> students;

		private static readonly string courseDescriptionFilePath = @"../../Courses.txt";
		private static readonly string studentDescriptionFilePath = @"../../Students.txt";


		public Student GetStudent(int studentID) {
			foreach (Student student in students) if (student.id == studentID) return student;
			throw new FaultException<StudentNotExistsFault>(
				new StudentNotExistsFault() { 
					Message = studentID + " is not a valid student id." 
				}
			);
		}

		public Course GetCourse(string courseID){
			foreach (Course course in courses) if (course.id == courseID) return course;
			throw new FaultException<CourseNotExistsFault>(
				new CourseNotExistsFault() { 
					Message = courseID + " is not a valid course." 
				}
			);
		}

		public static Manager GetManager(int maxCoursePerStudent = 3) {
			if (instance == null) instance = new Manager(maxCoursePerStudent);
			return instance;
		}

		private Manager(int maxCoursePerStudent = 3) {

			try {
				this.maxCoursePerStudent = maxCoursePerStudent;
				courses = Course.GetCourseList(Manager.courseDescriptionFilePath);
				students = Student.GetStudentList(Manager.studentDescriptionFilePath);
			} catch (Exception e) {
				Console.WriteLine("FATAL ERROR:Check Course and Student description file location and format");
				Console.ReadKey();
				Process.GetCurrentProcess().Kill();
			}
			//foreach (Course course in courses) course.print();
			//foreach (Student student in students) student.Print();
		}

		public bool StudentExists(int studentID){
			bool answer = this.students
			   .Where(student => student.id == studentID)
			   .ToList()
			   .Count
			   != 0;
			return answer;
		}

		public bool CourseExists(string courseID){
			bool answer = courses
				.Where(course => course.id == courseID)
				.ToList()
				.Count
				!= 0;
			return answer;
		}

		public bool CourseFull(string courseID){
			Course[] potentialCourses = this.courses.Where(course => course.id == courseID).ToArray();
			if (potentialCourses.Length == 0) {
				throw new FaultException<CourseNotExistsFault>(
					new CourseNotExistsFault() {
						Message = courseID + " is not a valid course"
					}
				);
			} else {
				int courseCapasity = potentialCourses[0].maxStudents;
				int occurrance = this.students
				.Select(student => student.takenCourses
					.Select(course => course == courseID)
					.Where(exists => exists == true)
					.ToList()
					.Count
					> 0
				).Where(taken => taken == true)
				.ToList()
				.Count;
				bool answer = occurrance >= courseCapasity;
				return answer;
			}			
		}

		public bool HasStudentTaken(int studentID, string courseID) {
			return GetStudent(studentID).HasTaken(courseID);
		}

		public bool HasRegisteredMax(int studentID){
			return GetStudent(studentID).takenCourses.Count >= maxCoursePerStudent;
		}

		public bool MeetsPrerequisite(int studentID, string courseID) {
			Course context = GetCourse(courseID);
			Student student = GetStudent(studentID);
			return context.prerequisite
				.Select(preCourse => student.HasTaken(preCourse))
				.Aggregate(true, (acc, entry) => acc &= entry);
		}

		public List<string> GetUnmetCorequisites(int studentID, string courseID){
			Student student = GetStudent(studentID);
			Course course = GetCourse(courseID);
			return course.corequisite
				.Where(corequisite => !student.HasTaken(corequisite))
				.ToList();
		}

		public List<string> DependentCoursesAsPrerequisite(string courseID){
			return courses
				.Where(course => course.prerequisite.Contains(courseID))
				.Select(course => course.id)
				.ToList();
		}

		public List<string> DependentCoursesAsCorequisite(string courseID) {
			return courses
				.Where(course => course.corequisite.Contains(courseID))
				.Select(course => course.id)
				.ToList();
		}

		public void WriteToFile() {
			try {
				string description = this.students
					.Select(student => student.ToString())
					.Aggregate((line1, line2) => line1 + "/" + line2);
				lock(this){
					using (StreamWriter writer = new StreamWriter(Manager.studentDescriptionFilePath)) {
						writer.Write(description);
						writer.Flush();
					}
				}
			}catch(Exception e){
				Console.WriteLine("::"+e.Message);
			}
		}
	}	
}