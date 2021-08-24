---
layout: post
title: Specifications 3&#58; The DSL Strikes Back
date_created: 13th of March, 2015
location: London, UK
author_name: Benjamin Hodgson
author_title: Machine Data and Big Learning Specialist
description: Building a Domain-Specific Language on top of composite specifications
---


tl;dr
-----

You should consider building a domain-specific
language for certain parts of your code.
You can end up with very clear code that can be read
by non-programmers.


Designing a domain-specific language
------------------------------------

In the [last post](Composite-specifications),
we developed a few classes that allowed us to combine small,
atomic specifications into more complex ones.
The resulting code was very terse, but not particularly meaningful
to a non-programmer. Our design is DRY (**D**on't **R**epeat **Y**ourself)
but not DAMP (**D**escriptive **A**nd **M**eaningful **P**hrases).

Let's remedy that and build a domain-specific language that resembles English.
The end result is going to look like this:

```csharp
var renameDocumentRule =
  (new UserHasReadPermission().And(new UserCreatedTheDocument())
                              .AndNot(new DocumentIsLocked()))
  .Or(new UserIsWorkspaceManager());
```

The DSL uses _method chaining_ (sometimes known as a _fluent interface_)
to drastically increase the readability of the code;
while the example I started with in the [first post](All-about-security)
had _no obvious bugs_, this version has _obviously no bugs_. It's a huge difference!

We're going to set ourselves the goal of building this DSL
on top of the specifications library we developed in the last post,
_without_ changing any of the code we've already written.


Extension methods
-----------------
<div style="float: right; margin-left:5px; position: relative; top: 10px;">
  <img src="/img/2015-03-13-Specifications-dsl/scream.png"/>
  <p class="caption">Me when I use Java</p>
</div>

We want to write `And` and `Or` as [_infix_](http://en.wikipedia.org/wiki/Infix_notation)
methods because that's how English works, but `ISpecification`
doesn't contain methods of that name.
In Java (the horror!), you'd need to convert the interface
to an abstract base class and implement `And` and `Or` there.
That would pollute our beautiful, crisp interface with
a bunch of extra methods for combining specifications.
_Combining_ specifications has nothing to do with _being_ a specification,
so `And` and `Or` don't belong in an abstract base class.
What's more, a class can only have one parent class (though it can implement many interfaces),
so requiring specifications to inherit from an abstract base class is restrictive.

Luckily, we can use C#'s
[extension methods](https://msdn.microsoft.com/en-GB/library/bb383977.aspx)
(indicated by the `this` keyword) to surgically implant new methods
onto our interface from the outside, without changing everything that implements it.

```csharp
static ISpecification<T>; And<T>;(
    this ISpecification<T>; left,
    ISpecification<T>; right)
{
  return new AndSpecification<T>;(left, right);
}
static ISpecification<T>; Or<T>;(
    this ISpecification<T>; left,
    ISpecification<T>; right)
{
  return new OrSpecification<T>;(left, right);
}
```

These methods operate on an `ISpecification<T>`
and return another `ISpecification<T>`.
This means you can call `And` on the result of another call to `And`,
chaining the methods indefinitely.
Incidentally, this is how LINQ works - methods like `Select`
and `Where` are defined as extension methods which take an `IEnumerable`
and return a new `IEnumerable`, allowing you to chain them to one another.


Prefix functions
----------------

To make our DSL read like English, we want to implement a [_prefix_](http://en.wikipedia.org/wiki/Polish_notation)
function called `Not`. But C#'s scoping rules present a sticking point
- if `Not` isn't defined in the current class, we can't use its name unqualified.

We could bring `Not` into scope by defining it in a base class which
all users of our DSL have to inherit from, but that would be
stretching the meaning of inheritance, and has the annoying restriction
that our users can't inherit from any other classes.
If we're willing to give up on the idea of a prefix function,
a more sanitary option is to define new extension methods which
negate their argument:

```csharp
static ISpecification<T>; AndNot<T>;(
    this ISpecification<T>; left,
    ISpecification<T>; right)
{
  return new AndSpecification<T>;(
      left,
      new NotSpecification<T>;(right));
}
static ISpecification<T>; OrNot<T>;(
    this ISpecification<T>; left,
    ISpecification<T>; right)
{
  return new OrSpecification<T>;(
      left,
      new NotSpecification<T>;(right));
}
```

But extension methods need an instance of `ISpecification` to chain from.
What if the specification you want to negate isn't part of a method chain?
The best we can do is write a static method with the
least disruptive name we can think of:

```csharp
public static class Not
{
  public ISpecification<T>; This<T>;(ISpecification<T>; spec)
  {
    return new NotSpecification<T>;(spec);
  }
}

// example
var documentIsNotLocked = Not.This(new DocumentIsLocked());
```

Users still have the option of defining a local function called `Not`
as an alias for `Not.This`. C# 6's upcoming "using static"
feature would solve this problem entirely - I could define a class
containing a static `Not` method and users of the DSL would be able to
import the name directly.


When to use a DSL
-----------------

We've defined a simple, readable domain-specific language for composing
specifications using C#. When does it make sense to build a fluent DSL?

* **Use a DSL for self-contained sections of your business logic**.
    Huddle's specification DSL is limited in scope to
    permission-checking. It would be a mistake to use a fluent
    interface for _all_ usages of Boolean logic in your program!
* **Express small domains**.
    Domain-specific languages are not general-purpose.
    They're at their most effective when there are only a few
    operations within your DSL. Don't fall into the trap of trying to
    create a fully-fledged extensible programming language -
    leave that to the professionals.
* **Mathematical structures are good**.
    It's quite difficult to change the syntax or semantics of
    a domain-specific language because everyone would have to re-learn it.
    This makes them a good fit to model unchanging mathematical
    theories such as the Boolean logic we've been modelling in this example.
    An advantage of building on mathematics is that other people
    have already done the hard work - you'll be able to draw from
    long-established definitions, theorems, and beautiful structures when
    designing and optimising your DSL.
* **Use a DSL for rapidly-changing parts of your system**.
    This is the flip-side of the last point. If the business-oriented details
    of your code change with the winds, the simple and flexible framework
    provided by a DSL can help you immensely.
* **Use a DSL when business-people want to read the code**.
    At Huddle, developers often get asked questions like
    "Why can't I delete this document?" by other members
    of the team. I can open up the file containing our fluently-defined
    permissions rules and I don't need to explain the code!
    This makes everyone feel good.

In [the next post](Specification-visitor),
we'll enterprise-ify this code even more, to make it easier to add new
ways to evaluate specifications.

In this series
--------------

1. [All about security](All-about-security.md)
2. [The power of Composite Specifications](Composite-specifications.md)
3. **Specifications 3: The DSL Strikes Back**
4. [Knock knock. Who's there? AbstractSpecificationNodeVisitorImpl](Specification-visitor.md)
