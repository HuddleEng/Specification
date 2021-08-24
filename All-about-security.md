---
layout: post
title: All About Security
date_created: 2nd of February, 2015
location: London, UK
author_name: Benjamin Hodgson
author_title: Chief Dude
description: An explanation of how Huddle's security engine works.
---

tl;dr
-----

At Huddle, we use the Specification pattern to implement the
security our customers rely upon. The next few blog posts are going to be
an in-depth look at how our security engine works.

Security rules at Huddle
------------------------

[We take security seriously](http://www.huddle.com/product-overview/secure-collaboration).
A big part of our product is permission control -
allowing our customers to make sure people can't see content
they're not supposed to see.

Often, the rules determining permissions can be satisfied
in more than one way. For example, in order to view a document
that's been uploaded to Huddle, your team must have been given
permission to do so by your boss - or you must be the boss yourself.
'Bosses' in Huddle are known as _workspace managers_.

A procedural approach
---------------------

Here's a short example of how you might naïvely implement the
security check I described above.

```csharp
Document ReadDocument(int documentId, User currentUser)
{
  var document = this.documentRepository.Get(documentId);

  var folder = document.ParentFolder;
  var userIsManager = currentUser.IsManager;
  var userHasReadPermission = folder.TeamsWithReadPermission
        .Any(team => team.ContainsUser(currentUser));
  if (!(userIsManager || userHasReadPermission))
  {
    throw new PermissionException();
  }

  return document;
}
```

This code does not adhere to the single responsibility principle - the
logic to check permissions is tangled up with the logic to read the document.
Huddle's security requirements involve lots of complex checks,
and defining the rules inline like this quickly becomes
error-prone and hard to understand. Let's refactor this procedural
code into a more scalable, object-oriented solution.

Extracting the rule definition
------------------------------

One way of separating the definition of a rule from its usage is
to extract it into a new class. We can apply a uniform interface
to our 'rule' classes to make them easier to use.

```csharp
interface ISecurityRule
{
  bool IsSatisfiedBy(SecurityContext candidate);
}
```

This is called the _Specification_ pattern.
A Specification (I've called it a 'security rule' here) is an
object with an `IsSatisfiedBy` method defining a Boolean rule.
When we pass an object to `IsSatisfiedBy`, the Specification tests
the object to see if it satisfies the criteria defined
by the Specification.

The `SecurityContext` here represents "the facts of the matter" -
it simply wraps up the information needed by security rules:

```csharp
class SecurityContext
{
  public Document Document { get; set; }
  public User CurrentUser { get; set; }
}
```

With this interface in place, we can encapsulate the code
to check permissions into a class:

```csharp
class ReadDocumentRule : ISecurityRule
{
  public bool IsSatisfiedBy(SecurityContext context)
  {
    var folder = context.Document.ParentFolder;

    var userIsManager = context.CurrentUser.IsManager;
    var userHasReadPermission = folder.TeamsWithReadPermission
          .Any(team => team.ContainsUser(currentUser));

    return userIsManager || userHasReadPermission;
  }
}

// another example: you may delete a document if you created it,
// or if you're a workspace manager
class DeleteDocumentRule : ISecurityRule
{
  public bool IsSatisfiedBy(SecurityContext context)
  {
    var userIsManager = context.CurrentUser.IsManager;
    var userCreatedTheDocument = context.CurrentUser == context.Document.Creator;

    return userIsManager || userCreatedTheDocument;
  }
}
```

Using specifications
--------------------

Here's a general, reusable method to test security rules:

```csharp
void CheckPermissions(ISecurityRule rule, SecurityContext context)
{
  if (!rule.IsSatisfiedBy(context))
  {
    throw new PermissionException();
  }
}
```

Finally, let's rewrite the `ReadDocument` method from earlier.
This version is shorter and cleaner, because it's no longer concerned
with the specifics of how to check permissions:

```csharp
Document ReadDocument(int documentId, User currentUser)
{
  var document = this.documentRepository.Get(documentId);

  CheckPermissions(
    new ReadDocumentRule(),
    new SecurityContext
    {
      Document = document,
      CurrentUser = currentUser
    });

  return document;
}
```

The main advantage this pattern gives us is that it enables
us to keep security rules separate from the code that uses them.
The specification classes are small and easy to understand;
this means fewer bugs, safer users and happier programmers.

In the [next post](Composite-specifications)),
I'll demonstrate a more sophisticated and powerful version
of this design, which makes handling complex rules much easier.

In this series
--------------

1. **All about security**
2. [The power of Composite Specifications](Composite-specifications.md)
3. [Specifications 3: The DSL Strikes Back](Specifications-dsl.md)
4. [Knock knock. Who's there? AbstractSpecificationNodeVisitorImpl](Specification-visitor.md)
