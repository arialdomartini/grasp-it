
namespace GraspIt
{

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

        public override string ToString()
        {
            return string.Format("[HomeWork: MathResult={0}, AnswerOnHistory={1}, AnswerOnMusic={2}, AnswerOnBiology={3}]", MathResult, AnswerOnHistory, AnswerOnMusic, AnswerOnBiology);
        }
    }
    
}