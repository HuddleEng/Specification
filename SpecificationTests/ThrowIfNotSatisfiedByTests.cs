using System;
using NUnit.Framework;
using Specification;
using Specification.SpecificationTree;

namespace SpecificationTests
{
    public class ThrowIfNotSatisfiedByTests
    {
        [Test]
        public void PassingSpecificationDoesNotThrow()
        {
            AssertDoesNotThrow(new PassingSpecification<object>());
        }

        [Test]
        public void FailingSpecificationThrows()
        {
            AssertThrows<SomeException>(new FailingSpecification<object>(new SomeException()));
        }

        [Test]
        public void AndNodeWithTwoFailingSpecsThrowsLeftException()
        {
            AssertThrows<SomeException>(new FailingSpecification<object>(new SomeException()).And(new FailingSpecification<object>(new SomeOtherException())));
        }

        [Test]
        public void AndNodeWithFailingLeftSpecThrows()
        {
            AssertThrows<SomeException>(new FailingSpecification<object>(new SomeException()).And(new PassingSpecification<object>()));
        }

        [Test]
        public void AndNodeWithFailingRightSpecThrows()
        {
            AssertThrows<SomeException>(new PassingSpecification<object>().And(new FailingSpecification<object>(new SomeException())));
        }

        [Test]
        public void AndNodeWithTwoPassingSpecsDoesNotThrow()
        {
            AssertDoesNotThrow(new PassingSpecification<object>().And(new PassingSpecification<object>()));
        }

        [Test]
        public void OrNodeWithTwoFailingSpecsThrowsLeftException()
        {
            AssertThrows<SomeException>(new FailingSpecification<object>(new SomeException()).Or(new FailingSpecification<object>(new SomeOtherException())));
        }

        [Test]
        public void OrNodeWithPassingLeftSpecDoesNotThrow()
        {
            AssertDoesNotThrow(new PassingSpecification<object>().Or(new FailingSpecification<object>()));
        }

        [Test]
        public void OrNodeWithPassingRightSpecDoesNotThrow()
        {
            AssertDoesNotThrow(new FailingSpecification<object>().Or(new PassingSpecification<object>()));
        }

        [Test]
        public void OrNodeWithTwoPassingSpecsDoesNotThrow()
        {
            AssertDoesNotThrow(new PassingSpecification<object>().Or(new PassingSpecification<object>()));
        }

        [Test]
        public void NotNodeWithFailingSpecDoesNotThrow()
        {
            AssertDoesNotThrow(Not.This(new FailingSpecification<object>()));
        }

        [Test]
        public void NotNodeWithPassingSpecThrows()
        {
            AssertThrows<Exception>(Not.This(new PassingSpecification<object>()));
        }

        [Test]
        public void ErrorOverridingNodeWithPassingSpecDoesNotThrow()
        {
            AssertDoesNotThrow(new PassingSpecification<object>().WithError(new Exception()));
        }

        [Test]
        public void ErrorOverridingNodeWithFailingSpecThrows()
        {
            AssertThrows<SomeException>(new FailingSpecification<object>(new SomeOtherException()).WithError(new SomeException()));
        }

        private void AssertThrows<T>(ISpecification<object> spec) where T : Exception
        {
            Assert.Throws<T>(() => spec.ThrowIfNotSatisfiedBy(new object()));
        }

        private void AssertDoesNotThrow(ISpecification<object> spec)
        {
            Assert.DoesNotThrow(() => spec.ThrowIfNotSatisfiedBy(new object()));
        }

        private class SomeException : Exception
        { }

        private class SomeOtherException : Exception
        { }
    }
}
