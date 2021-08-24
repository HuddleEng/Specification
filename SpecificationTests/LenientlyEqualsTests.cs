using System;
using NUnit.Framework;
using Specification;
using Specification.SpecificationTree;

namespace SpecificationTests
{
    public class LenientlyEqualsTests
    {
        [Test]
        public void LeafNodesAreEqualIfTheyAreOfTheSameType()
        {
            AssertEqual(new PassingSpecification<object>(), new PassingSpecification<object>());
        }

        [Test]
        public void LeafNodesAreNotEqualIfTheyAreNotOfTheSameType()
        {
            AssertNotEqual(new PassingSpecification<object>(), new FailingSpecification<object>());
        }

        [Test]
        public void LeafNodesAreAbleToOverrideEquals()
        {
            AssertEqual(new AlwaysEqualSpecification<object>(), new FailingSpecification<object>());
        }

        [Test]
        public void AndNodesWithEqualChildrenAreEqual()
        {
            AssertEqual(new PassingSpecification<object>().And(new FailingSpecification<object>()), new PassingSpecification<object>().And(new FailingSpecification<object>()));
        }

        [Test]
        public void AndNodesWithFlippedChildrenAreEqual()
        {
            AssertEqual(new FailingSpecification<object>().And(new PassingSpecification<object>()), new PassingSpecification<object>().And(new FailingSpecification<object>()));
        }

        [Test]
        public void AndNodesWithDifferentChildrenAreNotEqual()
        {
            AssertNotEqual(new FailingSpecification<object>().And(new FailingSpecification<object>()), new PassingSpecification<object>().And(new FailingSpecification<object>()));
        }

        [Test]
        public void OrNodesWithEqualChildrenAreEqual()
        {
            AssertEqual(new PassingSpecification<object>().Or(new FailingSpecification<object>()), new PassingSpecification<object>().Or(new FailingSpecification<object>()));
        }

        [Test]
        public void OrNodesWithFlippedChildrenAreEqual()
        {
            AssertEqual(new FailingSpecification<object>().Or(new PassingSpecification<object>()), new PassingSpecification<object>().Or(new FailingSpecification<object>()));
        }

        [Test]
        public void OrNodesWithDifferentChildrenAreNotEqual()
        {
            AssertNotEqual(new FailingSpecification<object>().Or(new FailingSpecification<object>()), new PassingSpecification<object>().Or(new FailingSpecification<object>()));
        }

        [Test]
        public void NotNodesWithEqualChildrenAreEqual()
        {
            AssertEqual(Not.This(new PassingSpecification<object>()), Not.This(new PassingSpecification<object>()));
        }

        [Test]
        public void NotNodesWithDifferentChildrenAreNotEqual()
        {
            AssertNotEqual(Not.This(new PassingSpecification<object>()), Not.This(new FailingSpecification<object>()));
        }

        [Test]
        public void MessageOverrideNodesWithEqualChildrenAndEqualExceptionsAreEqual()
        {
            var exc = new Exception();
            AssertEqual(new PassingSpecification<object>().WithError(exc), new PassingSpecification<object>().WithError(exc));
        }

        [Test]
        public void MessageOverrideNodesWithDifferentChildrenAreNotEqual()
        {
            var exc = new Exception();
            AssertNotEqual(new PassingSpecification<object>().WithError(exc), new FailingSpecification<object>().WithError(exc));
        }

        [Test]
        public void MessageOverrideNodesWithDifferentExceptionsAreNotEqual()
        {
            AssertNotEqual(new PassingSpecification<object>().WithError(new Exception("a message")), new PassingSpecification<object>().WithError(new Exception("a different message")));
        }

        [Test]
        public void AndNodesAndOrNodesAreNotEqual()
        {
            AssertNotEqual(new PassingSpecification<object>().And(new FailingSpecification<object>()), new PassingSpecification<object>().Or(new FailingSpecification<object>()));
        }

        [Test]
        public void AndNodesAndNotNodesAreNotEqual()
        {
            AssertNotEqual(new FailingSpecification<object>().And(new PassingSpecification<object>()), Not.This(new FailingSpecification<object>()));
        }

        [Test]
        public void AndNodesAndMessageOverrideNodesAreNotEqual()
        {
            AssertNotEqual(new FailingSpecification<object>().And(new PassingSpecification<object>()), new PassingSpecification<object>().WithError(new Exception()));
        }

        [Test]
        public void OrNodesAndNotNodesAreNotEqual()
        {
            AssertNotEqual(new FailingSpecification<object>().Or(new FailingSpecification<object>()), new PassingSpecification<object>().Or(new FailingSpecification<object>()));
        }

        [Test]
        public void OrNodesAndMessageOverrideNodesAreNotEqual()
        {
            AssertNotEqual(new FailingSpecification<object>().Or(new PassingSpecification<object>()), new PassingSpecification<object>().WithError(new Exception()));
        }

        [Test]
        public void NotNodesAndMessageOverrideNodesAreNotEqual()
        {
            AssertNotEqual(Not.This(new PassingSpecification<object>()), new PassingSpecification<object>().WithError(new Exception()));
        }

        static void AssertEqual(ISpecification<object> left, ISpecification<object> right)
        {
            Assert.True(left.LenientlyEquals(right));
        }

        static void AssertNotEqual(ISpecification<object> left, ISpecification<object> right)
        {
            Assert.False(left.LenientlyEquals(right));
        }
    }
}
