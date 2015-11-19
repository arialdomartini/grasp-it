using System.Text;

namespace GraspIt
{
    public class Solution : HomeWork
    {
        public int HighestMark = 10;
        public int MediumMark = 6;
        public int LowestMark = 3;


        public string PrintReport()
        {
            var sb = new StringBuilder();

            sb.AppendLine("Question abot maths: what is 5 + 5? The right reply is");
            sb.AppendLine("  Maths: " + MathResult.ToString());

            sb.AppendLine("Question abot History is: did ancient Roman live in Australia?");
            sb.AppendLine("  History: " + this.AnswerOnHistory);

            sb.AppendLine("Question abot Music is: was Mozart a country singer?");
            sb.AppendLine("  Music: " + this.AnswerOnMusic);

            sb.AppendLine("Question abot Biology is: are mushrooms animals?");
            sb.AppendLine("  Biology: " + this.AnswerOnBiology);

            return sb.ToString();
        }

        public static bool operator ==(Solution solution, HomeWork homeWork)
        {
            return solution.AnswerOnBiology == (homeWork as Solution).AnswerOnBiology && 
                solution.AnswerOnHistory == (homeWork as Solution).AnswerOnHistory && 
                solution.AnswerOnMusic == (homeWork as Solution).AnswerOnMusic && 
                solution.MathResult == (homeWork as Solution).MathResult;
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