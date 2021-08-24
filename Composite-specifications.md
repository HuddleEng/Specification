---
layout: post
title: The power of Composite Specifications
date_created: 23rd of February, 2015
location: London, UK
author_name: Benjamin Hodgson
author_title: Cardigan Inspector
description: Extending our previous example to a more flexible design
---


tl;dr
-----

I'm going to refactor the specifications I showed
you in the [previous post](All-about-security.md)
into a powerful, composable object model.

The generic Specification interface
-----------------------------------

First of all I'm going to generalise `ISecurityRule`
by replacing `SecurityContext` with a generic type parameter.

```csharp
interface ISpecification<in T>
{
  bool IsSatisfiedBy(T candidate);
}
```

You can now write a specification for any type you like.
The old `ISecurityRule` interface is equivalent to
`ISpecification<SecurityContext>`.

Extracting smaller specifications
---------------------------------

One code smell from the earlier example was that our two rules
contained duplicated code to check whether the user is a manager.
It'd be nice if we could reuse the constituent parts of each specification.

We can make our rules more reusable by breaking them down into
tests of individual cases.

```csharp
class UserIsWorkspaceManager : ISpecification<SecurityContext>
{
  public bool IsSatisfiedBy(SecurityContext context)
  {
    return context.CurrentUser.IsManager;
  }
}

class UserHasReadPermission : ISpecification<SecurityContext>
{
  public bool IsSatisfiedBy(SecurityContext context)
  {
    var folder = context.Document.ParentFolder;
    return folder.TeamsWithReadPermission
            .Any(team => team.ContainsUser(context.CurrentUser));
  }
}

class UserCreatedTheDocument : ISpecification<SecurityContext>
{
  public bool IsSatisfiedBy(SecurityContext context)
  {
    return context.Document.Creator == context.CurrentUser;
  }
}
```

We can build up a library of 'atomic' specifications like these,
each of which tests one fact, and reuse them in larger rules straightforwardly:

```csharp
class ReadDocumentRule : ISpecification<SecurityContext>
{
  public bool IsSatisfiedBy(SecurityContext context)
  {
    var userIsManager = new UserIsWorkspaceManager();
    var userHasReadPermission = new UserHasReadPermission();

    return userIsManager.IsSatisfiedBy(context)
        || userHasReadPermission.IsSatisfiedBy(context);
  }
}

class DeleteDocumentRule : ISpecification<SecurityContext>
{
  public bool IsSatisfiedBy(SecurityContext context)
  {
    var userIsManager = new UserIsWorkspaceManager();
    var userCreatedTheDocument = new UserCreatedTheDocument();

    return userIsManager.IsSatisfiedBy(context)
        || userCreatedTheDocument.IsSatisfiedBy(context);
  }
}
```

Composing specifications
------------------------

This last change has revealed a pattern in the higher-level specifications:
each one is built from smaller specifications, combining them using Boolean logic.
We can remove the duplicated code in those classes by writing some
_composite specifications_ to express Boolean combinations of specifications.

```csharp
class OrSpecification<T>; : ISpecification<T>;
{
  private readonly ISpecification<T>; left;
  private readonly ISpecification<T>; right;

  public OrSpecification(
      ISpecification<T>; left,
      ISpecification<T>; right)
  {
    this.left = left;
    this.right = right;
  }

  public bool IsSatisfiedBy(T candidate)
  {
    return this.left.IsSatisfiedBy(candidate)
        || this.right.IsSatisfiedBy(candidate);
  }
}

class AndSpecification<T>; : ISpecification<T>;
{
  private readonly ISpecification<T>; left;
  private readonly ISpecification<T>; right;

  public AndSpecification(
      ISpecification<T>; left,
      ISpecification<T>; right)
  {
    this.left = left;
    this.right = right;
  }

  public bool IsSatisfiedBy(T candidate)
  {
    return this.left.IsSatisfiedBy(candidate)
        &amp;&amp; this.right.IsSatisfiedBy(candidate);
  }
}

class NotSpecification<T>; : ISpecification<T>;
{
  private readonly ISpecification<T>; spec;

  public NotSpecification(ISpecification<T>; spec)
  {
    this.spec = spec;
  }

  public bool IsSatisfiedBy(T candidate)
  {
    return !this.spec.IsSatisfiedBy(candidate);
  }
}
```

Now that we've encapsulated the code to combine specifications
in these three classes, our larger specifications
couldn't be simpler:

```csharp
var readDocumentRule = new OrSpecification<SecurityContext>(
      new UserIsWorkspaceManager(),
      new UserHasReadPermission());
var deleteDocumentRule = new OrSpecification<SecurityContext>(
      new UserIsWorkspaceManager(),
      new UserCreatedTheDocument());
```

This design is fractal - you can build up specifications
which are composed of specifications which are composed of specifications.
For example, _renaming_ a document could be considered a _read_ followed by a _delete_:

```csharp
var renameDocumentRule = new AndSpecification<SecurityContext>(
      readDocumentRule,
      deleteDocumentRule);
```

Despite its simplicity, this is a really powerful technique! Even with only a
few atomic specifications, you can build up a large catalogue of rules
by composing them with one another.

In the [next post](Specifications-dsl.md),
I'll show you how to turn this model of specifications into a clear,
readable domain-specific language.

In this series
--------------

1. [All about security](All-about-security.md)
2. **The power of Composite Specifications**
3. [Specifications 3: The DSL Strikes Back](Specifications-dsl.md)
4. [Knock knock. Who's there? AbstractSpecificationNodeVisitorImpl](Specification-visitor.md)
