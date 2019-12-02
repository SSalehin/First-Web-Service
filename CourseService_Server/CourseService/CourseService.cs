using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace CourseService {
	public class CourseService : ICourseService, IDisposable {
		private static Manager manager;
		private static int instanceCount = 0;

		public CourseService() {
			Console.WriteLine("Constructor called");
			instanceCount++;
			CourseService.manager = Manager.GetManager();
		}


		public List<string> GetRegistrationList(int studentID) {
			//(1) the student exist
			bool studentExists = manager.StudentExists(studentID);
			if (!studentExists) {
				throw new FaultException<StudentNotExistsFault>(
					new StudentNotExistsFault() {
						Message = studentID + " is not registered as a student."
					}
				);
			}

			Student student = manager.GetStudent(studentID);
			return student.takenCourses;
		}

		public void Register(int studentID, string courseID) {
			//1. The student exist
			bool studentExists = manager.StudentExists(studentID);
			if (!studentExists) {
				throw new FaultException<StudentNotExistsFault>(
					new StudentNotExistsFault() {
						Message = studentID + " is not registered as a student."
					}
				);
			}				
			//1a. Course exists
			bool courseExists = manager.CourseExists(courseID);
			if (!courseExists) {
				throw new FaultException<CourseNotExistsFault>(
					new CourseNotExistsFault {
						Message = courseID + " is not a listed course."
					}
				);
			}
			//2. The course is not full
			bool courseFull = true;
			try {
				courseFull = manager.CourseFull(courseID);
			} catch (Exception e) {
				Console.WriteLine("Manager.CourseFull:: Error: " + e.Message);
			}
			if (courseFull) {
				throw new FaultException<CourseFullFault>(
					new CourseFullFault() {
						Message = courseID + " is already full"
					}
				);
			}
			//3. The student has already taken/registered in the course
			bool hasTaken = manager.HasStudentTaken(studentID, courseID);
			if (hasTaken) {
				throw new FaultException<AlreadyRegisteredFault>(
					new AlreadyRegisteredFault() {
						Message = studentID + " has already taken " + courseID
					}
				);
			}
			//4. The student has already registered for the maximum courses
			bool maxRegistered = manager.HasRegisteredMax(studentID);
			if (maxRegistered) {
				throw new FaultException<TakenTooManyCoursesFault>(
					new TakenTooManyCoursesFault() {
						Message = studentID + " has already registered for max amount of courses."
					}
				);
			}
			//5. the requested course has a pre-requisite
			bool prerequisiteMet = manager.MeetsPrerequisite(studentID, courseID);
			if (!prerequisiteMet){
				throw new FaultException<PrerequisiteNotMetFault>(
					new PrerequisiteNotMetFault() {
						Message = studentID + " does not meet all the prerequisite for " + courseID
					}
				);
			} 
			//6. Check for co-requisites	
			List<string> unmetCorequisites = manager.GetUnmetCorequisites(studentID, courseID);
			foreach (string req in unmetCorequisites) Console.WriteLine("*" + req);
			Student student = manager.GetStudent(studentID);
			if (student.takenCourses.Count + unmetCorequisites.Count + 1 > manager.maxCoursePerStudent) {
				throw new FaultException<TakenTooManyCoursesFault>(
					new TakenTooManyCoursesFault() {
						Message = "Including corequisites for " + courseID + "," + studentID + " does not have enough available slots."
					}
				);
			}
			// All okay. Now register
			lock (manager) {
				student.takenCourses.Add(courseID);
			}
		}

		public void Unregister(int studentID, string courseID) {
			//1a. The student exist
			bool studentExists = manager.StudentExists(studentID);
			if (!studentExists) {
				throw new FaultException<StudentNotExistsFault>(
					new StudentNotExistsFault() {
						Message = studentID + " is not registered as a student."
					}
				);
			}
			//1b. Course exists
			bool courseExists = manager.CourseExists(courseID);
			if (!courseExists) {
				throw new FaultException<CourseNotExistsFault>(
					new CourseNotExistsFault {
						Message = courseID + " is not a listed course."
					}
				);
			}

			Student student = manager.GetStudent(studentID);
			Course course = manager.GetCourse(courseID);

			//2. Prerequisite of another course
			List<string> dependentAsPrerequisiteCourses = manager.DependentCoursesAsPrerequisite(courseID);
			List<string> potentialPreZombies = dependentAsPrerequisiteCourses
				.Where(course_ => student.takenCourses.Contains(course_)).ToList();
			if (potentialPreZombies.Count > 0) {
				string courseList = potentialPreZombies.Aggregate("", (acc, course_) => acc += ',' + course_);
				throw new FaultException<PrerequisiteOfAnotherFault>(
					new PrerequisiteOfAnotherFault() {
						Message = "Couldn't unregister " + courseID + " because it is prerequisite of these courses: " + courseList
					}
				);
			}
			//3. the requested course is a corequisite of another course which the student should also unregister.
			List<string> dependentAsCorequisiteCourses = manager.DependentCoursesAsCorequisite(courseID);
			List<string> potentialCoZombies = dependentAsCorequisiteCourses
				.Where(course_ => student.takenCourses.Contains(course_)).ToList();
			lock (manager) {
				student.takenCourses.Remove(courseID);
				foreach (string pcz in potentialCoZombies) student.takenCourses.Remove(pcz);
			}
		}


		public void Dispose() {
			--instanceCount;
			if (instanceCount == 0) { 
				lock(this){
					manager.WriteToFile();
				}
			}
		}
	}
}
