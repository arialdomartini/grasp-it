using GraspIt;
using FluentAssertions;
using NUnit.Framework;

namespace GraspIt.Test
{
    public class MyClassTest
    {
        [Test]
        public void ShouldPass()
        {
            new MyClass().DoSomething().Should().BeTrue();
        }
    }
}