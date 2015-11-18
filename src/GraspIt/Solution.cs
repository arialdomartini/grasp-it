
namespace GraspIt
{

    public class Solution : HomeWork
    {
        public int HighestMark = 10;
        public int MediumMark = 6;
        public int LowestMark = 3;

        public static bool operator ==(Solution solution, HomeWork homeWork)
        {
            return HaveTheSameValues(solution, homeWork);
        }

        static bool HaveTheSameValues(Solution solution, HomeWork homeWork)
        {
            var other = homeWork as Solution;
            return solution.AnswerOnBiology == other.AnswerOnBiology && solution.AnswerOnHistory == other.AnswerOnHistory && solution.AnswerOnMusic == other.AnswerOnMusic && solution.MathResult == other.MathResult;
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