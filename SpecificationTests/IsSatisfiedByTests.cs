using System;
using NUnit.Framework;
using Specification;
using Specification.SpecificationTree;

namespace SpecificationTests
{
    public class IsSatisfiedByTests
    {
        [Test]
        public void PassingLeafNodeIsSatisfied()
        {
            AssertIsSatisfied(new PassingSpecification<object>());
        }

        [Test]
        public void FailingLeafNodeIsNotSatisfied()
        {
            AssertIsNotSatisfied(new FailingSpecification<object>());
        }

        [Test]
        public void AndNodeWithTwoFailingSpecsIsNotSatisfied()
        {
            AssertIsNotSatisfied(new FailingSpecification<object>().And(new FailingSpecification<object>()));
        }

        [Test]
        public void AndNodeWithFailingLeftSpecIsNotSatisfied()
        {
            AssertIsNotSatisfied(new FailingSpecification<object>().And(new PassingSpecification<object>()));
        }

        [Test]
        public void AndNodeWithFailingRightSpecIsNotSatisfied()
        {
            AssertIsNotSatisfied(new PassingSpecification<object>().And(new FailingSpecification<object>()));
        }

        [Test]
        public void AndNodeWithTwoPassingSpecsIsSatisfied()
        {
            AssertIsSatisfied(new PassingSpecification<object>().And(new PassingSpecification<object>()));
        }

        [Test]
        public void OrNodeWithTwoFailingSpecsIsNotSatisfied()
        {
            AssertIsNotSatisfied(new FailingSpecification<object>().Or(new FailingSpecification<object>()));
        }

        [Test]
        public void OrNodeWithPassingLeftSpecIsSatisfied()
        {
            AssertIsSatisfied(new PassingSpecification<object>().Or(new FailingSpecification<object>()));
        }

        [Test]
        public void OrNodeWithPassingRightSpecIsSatisfied()
        {
            AssertIsSatisfied(new FailingSpecification<object>().Or(new PassingSpecification<object>()));
        }

        [Test]
        public void OrNodeWithTwoPassingSpecsIsSatisfied()
        {
            AssertIsSatisfied(new PassingSpecification<object>().Or(new PassingSpecification<object>()));
        }

        [Test]
        public void NotNodeWithFailingSpecIsSatisfied()
        {
            AssertIsSatisfied(Not.This(new FailingSpecification<object>()));
        }

        [Test]
        public void NotNodeWithPassingSpecIsNotSatisfied()
        {
            AssertIsNotSatisfied(Not.This(new PassingSpecification<object>()));
        }

        [Test]
        public void ErrorOverridingNodeWithPassingSpecIsSatisfied()
        {
            AssertIsSatisfied(new PassingSpecification<object>().WithError(new Exception()));
        }

        [Test]
        public void ErrorOverridingNodeWithFailingSpecIsNotSatisfied()
        {
            AssertIsNotSatisfied(new FailingSpecification<object>().WithError(new Exception()));
        }

        private void AssertIsSatisfied(ISpecification<object> spec)
        {
            Assert.True(spec.IsSatisfiedBy(new object()));
        }

        private void AssertIsNotSatisfied(ISpecification<object> spec)
        {
            Assert.False(spec.IsSatisfiedBy(new object()));
        }
    }
}