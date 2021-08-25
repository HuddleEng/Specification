Specification pattern

Simple example in the tests can be found here
[UserPreviouslyActiveIsPartOfTheRecentlyInactiveCohort.cs](SpecificationTests/UserPreviouslyActiveIsPartOfTheRecentlyInactiveCohort.cs)

```csharp
ISpecification<UserContext> inactiveCohort =
                new UserIsInactiveForOverTwoWeeks()
                .And(new UserHasViewedSomeThing())
                .And(new UserHasUploadedSomeThing())
                .And(new UserHasNotReceivedAReportThisMonth());

var userContext = new UserContext
{
    LastActiveDate = DateTime.UtcNow.AddDays(-15),
    LastReportSent = DateTime.UtcNow.AddMonths(-1),
    TotalUploads = 10,
    TotalViews = 50
};

bool userInCohort = inactiveCohort.IsSatisfiedBy(userContext)
```

In this series
--------------

1. [All about security](docs/All-about-security.md)
2. [The power of Composite Specifications](docs/Composite-specifications.md)
3. [Specifications 3: The DSL Strikes Back](docs/Specifications-dsl.md)
4. [Knock knock. Who's there? AbstractSpecificationNodeVisitorImpl](docs/Specification-visitor.md)
