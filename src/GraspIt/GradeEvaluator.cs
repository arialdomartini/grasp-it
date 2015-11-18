using System.Collections.Generic;
using System.Linq;
using System;
using NLog;

namespace GraspIt
{
    public class GradeEvaluator
    {
        NLog.Logger Log = LogManager.GetCurrentClassLogger();

        void TweetResult(string firstName)
        {
            // TODO implement in the future
            return;
        }

        public Dictionary<Student, int> Eval(List<Student> students, Dictionary<string, HomeWork> results, Solution solution)
        {
            try
            {
                var marks = new Dictionary<Student, int>();
                foreach(var pair in results)
                {
                    var studentId = pair.Key;
     
                    var student = GetStudentFromId(studentId, students);
                    if(student == null)
                        throw new ApplicationException("Student not found");
                    
                    var homework = pair.Value;

                    bool perfectSolution = false;
                    try
                    {
                        if (solution == homework)
                        {
                            perfectSolution = true;
                            TweetResult(student.FirstName);
                        }
                    }
                    catch
                    {
                        Log.Error("Cannot tweet result");
                    }

                    if(perfectSolution)
                    {
                        marks.Add(student, solution.HighestMark);

                    }
                    else
                    {
                        try
                        {
                            if (homework.CountErrors(solution) == 1)
                                marks.Add(student, solution.MediumMark);
                            else
                                marks.Add(student, solution.LowestMark);
                        } catch
                        {
                            throw new ApplicationException("Can't add marks to student " + student.FirstName);
                        }

                    }
                }
                return marks;    
            }
            catch(Exception e)
            {
                Log.Error(e);
                return null;
            }
        }

        Student GetStudentFromId(string studentId, List<Student> students)
        {
            try
            {
                return students.Where(s => s.Id == studentId).First();
            }
            catch
            {
                return null;
            }
        }
    }
    
}