using FluentAssertions;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Diagnostics;
using System.Security.Cryptography;
using System;
using System.Linq;
using System;
using System.Runtime.Remoting.Messaging;

namespace GraspIt.Test
{
    public class MyClassTest
    {
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

            var gradeEvaluator = new GradeEvaluator();
            var votes = gradeEvaluator.Eval(students, results, solution);

            votes[john].Should().Be(3);
            votes[mario].Should().Be(6);
            votes[giovanna].Should().Be(10);
        }
    }


    class Logger
    {
        public void Error(string message, Exception e)
        {
            Console.WriteLine(string.Format("{0}, {1} {2}", message, e.Message, e.StackTrace.ToString()));
        }
    }

    public class GradeEvaluator
    {
        Logger Log = new Logger();

        public Dictionary<Student, int> Eval(List<Student> students, Dictionary<string, HomeWork> results, Solution solution)
        {
            try
            {
                var votes = new Dictionary<Student, int>();
                foreach(var pair in results)
                {
                    var studentId = pair.Key;
                    var student = GetStudentFromId(studentId, students);
                    var homework = pair.Value;

                    if(solution == homework)
                    {
                        votes.Add(student, solution.Super);
                    }
                    else
                    {
                        if(homework.CountErrors(solution) == 1)
                            votes.Add(student, solution.Ok);
                        else
                            votes.Add(student, solution.No);

                    }
                }
                return votes;    
            }
            catch(Exception e)
            {
                Log.Error("Something went wrong", e);
                return null;
            }
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
        public int Super = 10;
        public int Ok = 6;
        public int No = 3;

        public static bool operator ==(Solution solution, HomeWork other)
        {
            //if (solution == other)
            //    return true;
            return solution.AnswerOnBiology == other.AnswerOnBiology &&
                solution.AnswerOnHistory == other.AnswerOnHistory &&
                solution.AnswerOnMusic == other.AnswerOnMusic &&
                solution.MathResult == other.MathResult;
        }

        public static bool operator !=(Solution solution, HomeWork homework)
        {
            return !(solution == homework);
        }

        public override bool Equals(object obj)
        {
            return object.ReferenceEquals(this, obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}