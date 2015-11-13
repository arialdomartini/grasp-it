using GraspIt;
using FluentAssertions;

namespace GraspIt.Test
{
    public class MyClassTest
    {
        public void ShouldPass()
        {
            new MyClass().DoSomething().Should().BeTrue();
        }
    }
}