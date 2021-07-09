using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Configuration;

namespace LinqToSQL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        LinqToSQLDataClassesDataContext dataContext;
        
        public MainWindow()
        {
            InitializeComponent();

            string connectionString = ConfigurationManager.ConnectionStrings["LinqToSQL.Properties.Settings.DefaultDBConnectionString"].ConnectionString;
            dataContext = new LinqToSQLDataClassesDataContext(connectionString);

            DeleteTom();
        }

        public void InsertUniversities()
        {
            dataContext.ExecuteCommand("delete from University");
                        
            University liverpool = new University();
            liverpool.Name = "Liverpool";
            dataContext.Universities.InsertOnSubmit(liverpool);

            University leeds = new University();
            leeds.Name = "Leeds";
            dataContext.Universities.InsertOnSubmit(leeds);

            dataContext.SubmitChanges();

            MainDataGrid.ItemsSource = dataContext.Universities;
        }

        public void InsertStudents()
        {
            University liverpool = dataContext.Universities.First(un => un.Name.Equals("Liverpool"));
            University leeds = dataContext.Universities.First(un => un.Name.Equals("Leeds"));

            List<Student> students = new List<Student>();
            students.Add(new Student { Name = "Jack", Gender = "Male", UniversityId = liverpool.Id });
            students.Add(new Student { Name = "Tom", Gender = "Male", University = leeds });
            students.Add(new Student { Name = "Kira", Gender = "Female", University = leeds });
            students.Add(new Student { Name = "Matt", Gender = "Male", University = liverpool });

            dataContext.Students.InsertAllOnSubmit(students);

            dataContext.SubmitChanges();

            MainDataGrid.ItemsSource = dataContext.Students;
        }

        public void InsertLectures()
        {
            dataContext.Lectures.InsertOnSubmit(new Lecture { Name = "Math" });
            dataContext.Lectures.InsertOnSubmit(new Lecture { Name = "Law" });

            dataContext.SubmitChanges();

            MainDataGrid.ItemsSource = dataContext.Lectures;
        }

        public void InsertStudentLectureAssociations()
        {
            Student Jack = dataContext.Students.First(st => st.Name.Equals("Jack"));
            Student Tom = dataContext.Students.First(st => st.Name.Equals("Tom"));
            Student Kira = dataContext.Students.First(st => st.Name.Equals("Kira"));
            Student Matt = dataContext.Students.First(st => st.Name.Equals("Matt"));

            Lecture Math = dataContext.Lectures.First(le => le.Name.Equals("Math"));
            Lecture Law = dataContext.Lectures.First(le => le.Name.Equals("Law"));

            dataContext.StudentLectures.InsertOnSubmit(new StudentLecture { Student = Jack, Lecture = Law });
            dataContext.StudentLectures.InsertOnSubmit(new StudentLecture { Student = Tom, Lecture = Math });
            dataContext.StudentLectures.InsertOnSubmit(new StudentLecture { Student = Kira, Lecture = Math });
            dataContext.StudentLectures.InsertOnSubmit(new StudentLecture { Student = Matt, Lecture = Law });

            dataContext.SubmitChanges();

            MainDataGrid.ItemsSource = dataContext.StudentLectures;
        }

        public void GetUniversityOfJack()
        {
            Student Jack = dataContext.Students.First(st => st.Name.Equals("Jack"));

            University JacksUniversity = Jack.University;

            List<University> universities = new List<University>();
            universities.Add(JacksUniversity);

            MainDataGrid.ItemsSource = universities;
        }

        public void GetLecturesOfJack()
        {
            Student Jack = dataContext.Students.First(st => st.Name.Equals("Jack"));

            var JacksLectures = from sl in Jack.StudentLectures select sl.Lecture;

            MainDataGrid.ItemsSource = JacksLectures;
        }

        public void GetAllStudentsFromLiverpool()
        {
            var studentsFromLiverpool = from student in dataContext.Students
                                        where student.University.Name == "Liverpool"
                                        select student;

            MainDataGrid.ItemsSource = studentsFromLiverpool;
        }

        public void GetAllUniversitiesWithFemales()
        {
            var femaleUniversities = from student in dataContext.Students
                                     join university in dataContext.Universities
                                     on student.University equals university
                                     where student.Gender == "Female"
                                     select university;

            MainDataGrid.ItemsSource = femaleUniversities;
        }

        public void GetAllLecturesFromLeeds()
        {
            var lecturesFromLeeds = from sl in dataContext.StudentLectures
                                    join student in dataContext.Students on sl.Student.Id equals student.Id
                                    where student.University.Name == "Leeds"
                                    select sl.Lecture;

            MainDataGrid.ItemsSource = lecturesFromLeeds;
        }

        public void UpdateJack()
        {
            Student Jack = dataContext.Students.FirstOrDefault(st => st.Name.Equals("Jack"));

            Jack.Name = "Jaxx";

            dataContext.SubmitChanges();

            MainDataGrid.ItemsSource = dataContext.Students;
        }

        public void DeleteTom()
        {
            Student Tom = dataContext.Students.FirstOrDefault(st => st.Name.Equals("Tom"));
            dataContext.Students.DeleteOnSubmit(Tom);
            dataContext.SubmitChanges();

            /* IF ERROR OCCURS, REBUILD CONNECTION

            string connectionString = ConfigurationManager.ConnectionStrings["LinqToSQL.Properties.Settings.DefaultDBConnectionString"].ConnectionString;
            LinqToSQLDataClassesDataContext db = new LinqToSQLDataClassesDataContext(connectionString);
            */

            MainDataGrid.ItemsSource = dataContext.Students;
        }
    }
}
