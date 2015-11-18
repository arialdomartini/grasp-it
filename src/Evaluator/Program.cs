using System;
using System.Collections.Generic;
using GraspIt;

namespace Evaluator
{
    class MainClass
    {
        public static void Main(string[] args)
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

            Console.WriteLine("The solution should be" + solution.PrintReport());

            var gradeEvaluator = new GradeEvaluator();
            var grades = gradeEvaluator.Eval(students, results, solution);
            foreach(var grade in grades)
            {
                Console.WriteLine("{0} got {1}", grade.Key.FirstName, grade.Value);
                Console.WriteLine("  Homework report: {0}", results[grade.Key.Id]);

                if (grade.Value == 3)
                    Console.WriteLine(string.Format("  -> {0} will be rejected from school", grade.Key.FirstName));
            }
        }
    }
}
