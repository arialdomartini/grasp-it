namespace GraspIt
{
    public class Student
    {
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string Id { get; set; }

        public Student(string firstName, string secondName, string id)
        {
            SecondName = secondName;
            FirstName = firstName;
            Id = id;
        }
    }
    
}