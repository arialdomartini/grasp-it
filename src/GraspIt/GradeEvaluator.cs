using System.Collections.Generic;
using System.Linq;
using System;

namespace GraspIt
{

    public class GradeEvaluator
    {
        Logger Log = new Logger();

        public Dictionary<Student, int> Eval(List<Student> students, Dictionary<string, HomeWork> results, Solution solution)
        {
//            try
            {
                var marks = new Dictionary<Student, int>();
                foreach(var pair in results)
                {
                    var studentId = pair.Key;
     
                    var student = GetStudentFromId(studentId, students);
                    if(student == null)
                        throw new ApplicationException("Student not found");
                    
                    var homework = pair.Value;

                    if(solution == homework)
                    {
                        marks.Add(student, solution.Super);
                    }
                    else
                    {
                        try
                        {
                            if(homework.CountErrors(solution) == 1)
                                marks.Add(student, solution.Ok);
                            else
                                marks.Add(student, solution.No);
                        }
                        catch
                        {
                            throw new ApplicationException("Can't add marks to student " + student.FirstName);
                        }

                    }
                }
                return marks;    
            }
//            catch(Exception e)
//            {
//                Log.Error("Something went wrong", e);
//                return null;
//            }
        }

        Student GetStudentFromId(string studentId, List<Student> students)
        {
            return students.Where(s => s.Id == studentId).First();
        }
    }
    
}