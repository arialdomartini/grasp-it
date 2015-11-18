using FluentAssertions;
using System.Collections.Generic;
using NUnit.Framework;


namespace GraspIt.Test
{
    public class EvaluatorTest
    {
        [Test]
        public void ShouldAssignMarksToStudents()
        {
            var john = new Student("John", "Capioca", "johnc");
            var mario = new Student("Mario", "Esposito", "marioe");
            var giovanna = new Student("Giovanna", "Marinozzi", "giovannam");
            var students = new List<Student>() { john, mario, giovanna };

            var results = new Dictionary<string, HomeWork>() {
                { "johnc", new HomeWork() { MathResult = 5, AnswerOnBiology = "yes", AnswerOnHistory = "no", AnswerOnMusic = "no"} },
                { "marioe", new HomeWork() { MathResult = 10, AnswerOnBiology = "yes", AnswerOnHistory = "no", AnswerOnMusic = "no"} },
                { "giovannam", new HomeWork() { MathResult = 10, AnswerOnBiology = "no", AnswerOnHistory = "no", AnswerOnMusic = "no"} },
            };

            var solution = new Solution() { MathResult = 10, AnswerOnBiology = "no", AnswerOnHistory = "no", AnswerOnMusic = "no"};

            var gradeEvaluator = new MarksEvaluator();
            var votes = gradeEvaluator.Eval(students, results, solution);

            votes[john].Should().Be(3);
            votes[mario].Should().Be(6);
            votes[giovanna].Should().Be(10);
        }
    }
}