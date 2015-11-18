using System.Collections.Generic;
using System.Linq;
using System;
using NLog;
using System.Configuration;
using NLog.LayoutRenderers;

namespace GraspIt
{
    public class MarksEvaluator
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
                    {
                        Log.Error(string.Format("Cannot find student with id {0}", studentId));
                        break;
                    }
                    
                    var homework = pair.Value;
                    if(pair.Value == null)
                        break;
                    
                    bool perfectSolution = false;
                    try
                    {
                        if (solution == homework && homework.CountErrors(solution) == 0)
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
                        var highestMark = solution.HighestMark;
                        if(highestMark == 0)
                        {
                            Log.Error(string.Format("Skipping student {0}, no HighestMark found.", student.FirstName));
                            break;
                        }
                        marks.Add(student, highestMark);
                    }

                    try
                    {
                        if (homework.CountErrors(solution) == 1)
                        {
                            var mediumMark = solution.MediumMark;
                            if(mediumMark == 0)
                            {
                                Log.Error(string.Format("Skipping student {0}, no MediumMark found", student.FirstName));
                                break;
                            }
                            marks.Add(student, mediumMark);

                        }
                        else
                        {
                            var lowestMark = solution.LowestMark;
                            if(lowestMark == 0)
                            {
                                Log.Error(string.Format("Skipping student {0}, no LowestMark found", student.FirstName));
                                break;
                            }
                            marks.Add(student, lowestMark);
                            TweetResult(student.FirstName);
                        }
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
            TwitterClient.GetInstance().Connect();
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