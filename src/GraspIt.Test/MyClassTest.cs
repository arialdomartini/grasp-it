using GraspIt;
using FluentAssertions;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Diagnostics;
using System.Security.Cryptography;
using System;

namespace GraspIt.Test
{
    public class MyClassTest
    {
        [Test]
        public void ShouldEvaluate()
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

            var gradeEvaluator = new GradeEvaluator();
            var votes = gradeEvaluator.Eval(students, results, solution);

            votes[john].Should().Be(3);
            votes[mario].Should().Be(6);
            votes[giovanna].Should().Be(10);
        }
    }

    public class GradeEvaluator
    {
        public Dictionary<Student, int> Eval(List<Student> students, Dictionary<string, HomeWork> results, Solution solution)
        {
            var votes = new Dictionary<Student, int>();
            foreach(var pair in results)
            {
                var studentId = pair.Key;
                var student = GetStudentFromId(studentId, students);
                var homework = pair.Value;

                if(solution == homework)
                {
                    votes.Add(student, 10);
                }
                else
                {
                    if(homework.CountErrors(solution) == 1)
                        votes.Add(student, 6);
                    else
                        votes.Add(student, 3);

                }
            }
            return votes;
        }

        Student GetStudentFromId(string studentId, List<Student> students)
        {
            return students.Where(s => s.Id == studentId).First();
        }
    }

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

    public class HomeWork
    {
        public decimal MathResult {get; set;}
        public string AnswerOnHistory {get; set;}
        public string AnswerOnMusic {get; set;}
        public string AnswerOnBiology {get; set;}


        public int CountErrors(Solution solution)
        {
            int count = 0;
            if (solution.AnswerOnBiology != AnswerOnBiology)
                count++;
            if (solution.AnswerOnHistory != AnswerOnHistory)
                count++;
            if (solution.AnswerOnMusic != AnswerOnMusic)
                count++;
            if (solution.MathResult != MathResult)
                count++;

            return count;
        }
    }


    public class Solution : HomeWork
    {
        public static bool operator ==(Solution solution, HomeWork homework)
        {
            return solution.Equals(homework);
        }

        public static bool operator !=(Solution solution, HomeWork homework)
        {
            return !(solution == homework);
        }

        public override bool Equals(object obj)
        {
            var other = obj as HomeWork;
            return AnswerOnBiology == other.AnswerOnBiology &&
            AnswerOnHistory == other.AnswerOnHistory &&
            AnswerOnMusic == other.AnswerOnMusic &&
            MathResult == other.MathResult;
        }
    }
}