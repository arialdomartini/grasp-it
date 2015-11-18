using System.Collections.Generic;
using System.Linq;
using System;
using NLog;

namespace GraspIt
{
    public class GradeEvaluator
    {
        NLog.Logger Log = LogManager.GetCurrentClassLogger();

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
                        if (homework == solution && homework.CountErrors(solution) == 0)
                        {
                            perfectSolution = true;
                            TweetResult(student.FirstName);
                        }
                    }
                    catch(Exception e)
                    {
                        Log.Error(e, "Cannot tweet result");
                    }

                    if(perfectSolution)
                    {
                        marks.Add(student, solution.HighestMark);
                    }


                        try
                        {
                            if (homework.CountErrors(solution) == 1)
                                marks.Add(student, solution.MediumMark);
                            else
                                marks.Add(student, solution.LowestMark);
                        }
                        catch(Exception e)
                        {
                            Log.Error(e, "Can't add marks to student " + student.FirstName);
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

        void TweetResult(string firstName)
        {
            // TODO implement in the future
        }

        Student GetStudentFromId(string studentId, List<Student> students)
        {
            try
            {
                return students.Where(s => s.Id == studentId).FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }
    }
    
}