using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace CourseServiceClient {
	enum state{ Menu, Show, Register, Unregister}

	class Program {
		static CourseServiceReference.CourseServiceClient client;
		static state currentState;

		static void Main(string[] args) {
			client = new CourseServiceReference.CourseServiceClient("BasicHttpBinding_ICourseService");
			currentState = state.Menu;
			while(true){
				int studentID = -1;
				string courseID;
				bool loop;

				switch (currentState) {

					case state.Menu:
					ShowMenu();
					int input = int.Parse(Console.ReadLine());
					switch (input) {
						case 1:
						currentState = state.Show;
						break;
						case 2:
						currentState = state.Register;
						break;
						case 3:
						currentState = state.Unregister;
						break;
						case 4:
						System.Environment.Exit(0);
						break;
						default:
						Console.WriteLine("You need to put numbers between 1 and 4 :p");
						currentState = state.Menu;
						break;
					}
					break;

					case state.Show:
					loop = true;
					while (loop) {
						try {
							Console.WriteLine("Enter desired STUDENT ID:");
							studentID = int.Parse(Console.ReadLine());
							loop = false;
						} catch (Exception e) {
							Console.WriteLine(e.Message);
							Console.WriteLine("In other words, try to put an integer number");
						}
					}
					string[] courses = GetRegistrationList(studentID);
					Console.WriteLine("---------------------------------------------------------");
					if (courses.Length == 0) {
						Console.WriteLine("This guy is not into any courses. Maybe, we can register him into one.");
						currentState = state.Menu;
						break;
					} else {
						Console.WriteLine("Registered courses:");
						foreach (string course in courses) Console.WriteLine(course);
					}
					currentState = state.Menu;
					break;

					case state.Register:
					loop = true;
					while(loop){
						try{
							Console.WriteLine("Enter desired STUDENT ID:");
							studentID = int.Parse(Console.ReadLine());
							loop = false;
						}catch(Exception e){
							Console.WriteLine(e.Message);
							Console.WriteLine("In other words, try to put an integer number");
						}
					}
					Console.WriteLine("Enter desired COURSE ID:");
					courseID = Console.ReadLine();
					Register(studentID, courseID); 
					currentState = state.Menu;
					break;

					case state.Unregister:
					loop = true;
					while (loop) {
						try {
							Console.WriteLine("Enter desired STUDENT ID:");
							studentID = int.Parse(Console.ReadLine());
							loop = false;
						} catch (Exception e) {
							Console.WriteLine(e.Message);
							Console.WriteLine("In other words, try to put an integer number");
						}
					}
					Console.WriteLine("Enter desired COURSE ID:");
					courseID = Console.ReadLine();
					Unregister(studentID, courseID);
					currentState = state.Menu;
					break;


					default:
					currentState = state.Menu;
					break;
				}
			}
			Console.ReadLine();
		}

		static void ShowMenu(){
			Console.WriteLine("==========================================================");
			Console.WriteLine("Greetings, Madam. :D ");
			Console.WriteLine("I can do the following things for you with this application:\n");
			Console.WriteLine("1. See registration list");
			Console.WriteLine("2. Register a student into a course");
			Console.WriteLine("3. Unregister a student out of a course");
			Console.WriteLine("4. Exit");
			Console.WriteLine();
			Console.WriteLine("So, what should I do?");
		}

		static string[] GetRegistrationList(int studentID) {
			try {
				string[] answer = client.GetRegistrationList(studentID);
				return answer;
			}catch(FaultException<CourseServiceReference.StudentNotExistsFault> e){
				Console.WriteLine(e.Detail.Message);
			}
			return new string[] { };
		}

		static void Unregister(int studentID, string courseID) {
			Console.WriteLine("---------------------------------------------------------");
			try {
				client.Unregister(studentID, courseID);
			}catch(FaultException<CourseServiceReference.StudentNotExistsFault> e){
				Console.WriteLine(e.Detail.Message);
			} catch (FaultException<CourseServiceReference.CourseNotExistsFault> e) {
				Console.WriteLine(e.Detail.Message);
			} catch (FaultException<CourseServiceReference.PrerequisiteOfAnotherFault> e) {
				Console.WriteLine(e.Detail.Message);//(1011, "MATH167")
			} catch (FaultException e){
				Console.WriteLine("OtherException" + e.Message);
			}

		}

		static void Register(int studentID, string courseID) {
			Console.WriteLine("---------------------------------------------------------");
			try {
				client.Register(studentID, courseID);
			} catch (FaultException<CourseServiceReference.StudentNotExistsFault> e) {
				Console.WriteLine(e.Detail.Message);
			} catch (FaultException<CourseServiceReference.CourseNotExistsFault> e) {
				Console.WriteLine(e.Detail.Message);
			} catch (FaultException<CourseServiceReference.CourseFullFault> e) {
				Console.WriteLine(e.Detail.Message);
			} catch (FaultException<CourseServiceReference.AlreadyRegisteredFault> e) {
				Console.WriteLine(e.Detail.Message);
			} catch (FaultException<CourseServiceReference.TakenTooManyCoursesFault> e) {
				Console.WriteLine(e.Detail.Message);
			} catch (FaultException<CourseServiceReference.PrerequisiteNotMetFault> e) {
				Console.WriteLine(e.Detail.Message);
			} catch (FaultException e) {
				Console.WriteLine("OtherException" + e.Message);
			}
		}
	}
}
//ALCM101:X:QNTM102/BHV104:X:ALCM101/BVLC203:BHV104,ALCM101:X/PSYC201:X:BBIO214/BBIO124:BIO132:X/BIO132:X:X/QNTM102:MATH167:X/MATH167:X:X/COMP167:X:X/BIOM214:MATH167,BIO132:QNTM102/CBIO236:COMP236,BIO132:X/MCS267:MATH167,BIO132:X/QBIO317:QNTM102,BIO132:PSYC201/PRED356:PSYC201,MATH167:X/CHAO456:PRED356,MCS267:X/SLNM478:PSYC201,PRED356:CHAO456/
//1000:BHV104;MATH167/1001:PRED356;MCS267/1002:MATH167;BIO132/1003:;/1004:BIO132/1005:;/1006:PSYC201;MATH167;BIO132/1007:;/1008:COMP167/1009:;/1010:;/1011:BIOM214;MATH167;BIO132/1012:BIOM214;QNTM102;PRED356;