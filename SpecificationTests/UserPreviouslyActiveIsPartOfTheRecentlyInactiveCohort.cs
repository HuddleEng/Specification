using System;
using NUnit.Framework;
using Specification;
using Specification.SpecificationTree;

namespace SpecificationTests
{
    internal class UserPreviouslyActiveIsPartOfTheRecentlyInactiveCohort
    {
        private readonly ISpecification<UserContext> _inactiveCohort;
        private UserContext _userContext;

        public UserPreviouslyActiveIsPartOfTheRecentlyInactiveCohort()
        {
            _inactiveCohort = new UserIsInactiveForOverTwoWeeks()
                .And(new UserHasViewedSomeThing())
                .And(new UserHasUploadedSomeThing())
                .And(new UserHasNotReceivedAReportThisMonth());
           
        }

        [SetUp]
        public void Setup()
        {
            _userContext = new UserContext
            {
                LastActiveDate = DateTime.UtcNow.AddDays(-15),
                LastReportSent = DateTime.UtcNow.AddMonths(-1),
                TotalUploads = 10,
                TotalViews = 50
            };
        }

        [Test]
        public void UserIsPartOfTheCohort()
        {
            Console.WriteLine(_inactiveCohort.PrettyPrint());
            Assert.IsTrue(_inactiveCohort.IsSatisfiedBy(_userContext));
        }

        [Test]
        public void UserHasBeenActiveInTheLastTwoWeeks()
        {
            _userContext.LastActiveDate = DateTime.UtcNow.AddDays(-10);

            Assert.IsFalse(_inactiveCohort.IsSatisfiedBy(_userContext));
        }

        [Test]
        public void UserHasNeverViewedAnyThing()
        {
            _userContext.TotalViews = 0;

            Assert.IsFalse(_inactiveCohort.IsSatisfiedBy(_userContext));
        }

        [Test]
        public void UserHasNeverUploadedAnyThing()
        {
            _userContext.TotalUploads = 0;

            Assert.IsFalse(_inactiveCohort.IsSatisfiedBy(_userContext));
        }

        [Test]
        public void UserHasReceivedAReportThisMonth()
        {
            _userContext.LastReportSent = DateTime.UtcNow;

            Assert.IsFalse(_inactiveCohort.IsSatisfiedBy(_userContext));
        }
    }

    internal class UserHasUploadedSomeThing : LeafSpecification<UserContext>
    {
        public override bool IsSatisfiedBy(UserContext context)
        {
            return context.TotalUploads > 0;
        }
    }

    internal class UserHasNotReceivedAReportThisMonth : LeafSpecification<UserContext>
    {
        public override bool IsSatisfiedBy(UserContext context)
        {
            var now = DateTime.UtcNow;
            return context.LastReportSent.Year == now.Year && context.LastReportSent.Month != now.Month;
        }
    }

    internal class UserHasViewedSomeThing : LeafSpecification<UserContext>
    {
        public override bool IsSatisfiedBy(UserContext context)
        {
            return context.TotalViews > 0;
        }
    }

    internal class UserIsInactiveForOverTwoWeeks : LeafSpecification<UserContext>
    {
        public override bool IsSatisfiedBy(UserContext context)
        {
            var timeSpan = DateTime.UtcNow - context.LastActiveDate.ToUniversalTime();
            return timeSpan.Days > 14;
        }
    }

    internal class UserContext
    {
        public DateTime LastActiveDate { get; set; }
        public int TotalViews { get; set; }
        public int TotalUploads { get; set; }
        public DateTime LastReportSent { get; set; }
    }
}