---
layout: post
title: Knock knock. Who's there? AbstractSpecificationNodeVisitorImpl
date_created: 31st of March, 2015
location: London, UK
author_name: Benjamin Hodgson
author_title: Ambassador of Funk
---

tl;dr
-----

Our journey into Huddle's security engine
ends with the Visitor pattern, which makes it easy to add
new operations to a closed set of data classes.


A problem looms
---------------

Let's imagine that you've followed my advice to the letter
(why on earth did you do that? I clearly have no idea what I'm talking about)
and built yourself a library of composable specifications.
Fast-forward twelve months and the library has been
modified to support a variety of new requirements: the CTO wants all
the security rules printed on a pile of paper every month so he
can ignore them; the QAs need to check if two specs are equal;
of course, we've got to support XML; oh, and it looks like someone thought
checking the number of specifications in a composite was important.

```csharp
interface ISpecification<T>;
{
  bool IsSatisfiedBy(T candidate);
  string PrettyPrint();
  bool Equals(ISpecification<T>; other);
  XmlNode Serialise();
  long Size();
}
```

Poor old `ISpecification` is having an identity crisis -
most of these methods have nothing to do with specifying things.
The more methods there are on an interface, the more likely it is that one
of the implementations isn't able to fill them all in - and then
`NotImplementedException`s threaten to leap out at you from behind every corner.

Object-oriented programming makes it easy to add new types of data
(just add a new class with the right methods) but hard to add new behaviours to that data
(you have to add a method to all the pre-existing classes).
We want to undo that property: we're unlikely to add new data
(the _and_, _or_ and _not_ combinations are eternal and unchanging),
but `ISpecification` already has a lot of behaviours
and it seems likely that we'll need to add more in the future.


Dumbing down the specifications
-------------------------------

The trick is to separate the roles of _combining_ specifications
into a tree and _operating on_ that tree of specifications.
Let's start by taking all the behaviour out of our classes,
so all that we have left is a tree of Boolean expressions:

```csharp
interface ISpecification<T>; { }
class AndSpecification<T>; : ISpecification<T>;
{
  public ISpecification<T>; Left { get; private set; }
  public ISpecification<T>; Right { get; private set; }
  public AndSpecification(
      ISpecification<T>; left,
      ISpecification<T>; right)
  {
    Left = left;
    Right = right;
  }
}
class OrSpecification<T>; : ISpecification<T>;
{
  public ISpecification<T>; Left { get; private set; }
  public ISpecification<T>; Right { get; private set; }
  public OrSpecification(
      ISpecification<T>; left,
      ISpecification<T>; right)
  {
    Left = left;
    Right = right;
  }
}
class NotSpecification<T>; : ISpecification<T>;
{
  public ISpecification<T>; Spec { get; private set; }
  public NotSpecification(ISpecification<T>; spec)
  {
    Spec = spec;
  }
}
abstract class AtomicSpecification<T>; : ISpecification<T>;
{
  abstract bool IsSatisfiedBy(T candidate);
}
```


Extracting behaviours
---------------------

Now, where to put the behaviours?
We can write classes that traverse the tree of
specifications by recursively calling themselves.
Each type of specification defined its own variation of each method,
so to recover that structure we'll need to overload the functions.

```csharp
class IsSatisfiedTester<T>;
{
  private T candidate;
  public IsSatisfiedTester(T candidate)
  {
    this.candidate = candidate;
  }
  public bool IsSatisfied(AtomicSpecification<T>; spec)
  {
    return spec.IsSatisfiedBy(this.candidate);
  }
  public bool IsSatisfied(AndSpecification<T>; spec)
  {
    return IsSatisfied(spec.Left)
        &amp;&amp; IsSatisfied(spec.Right);
  }
  public bool IsSatisfied(OrSpecification<T>; spec)
  {
    return IsSatisfied(spec.Left)
        || IsSatisfied(spec.Right);
  }
  public bool IsSatisfied(NotSpecification<T>; spec)
  {
    return !IsSatisfied(spec.Spec);
  }
}
class PrettyPrinter<T>;
{
  public string PrettyPrint(AtomicSpecification<T>; spec)
  {
    return spec.GetType().Name;
  }
  public string PrettyPrint(AndSpecification<T>; spec)
  {
    return string.Format("({0} AND {1})",
        PrettyPrint(spec.Left), PrettyPrint(spec.Right));
  }
  public string PrettyPrint(OrSpecification<T>; spec)
  {
    return string.Format("({0} OR {1})",
        PrettyPrint(spec.Left), PrettyPrint(spec.Right));
  }
  public string PrettyPrint(NotSpecification<T>; spec)
  {
    return string.Format("(NOT {0})", PrettyPrint(spec.Spec));
  }
}
```

Great! It's now super easy to add a new way to use the tree of specifications -
just add another one of these recursive traversal classes.
This is called the _Visitor pattern_.
One minor problem: it fails to compile. Overloads of a function are resolved at compile
time - so if all we have is an `ISpecification<T>`, there's no way of knowing which
version of `IsSatisfied` should be used in the recursive calls. I'm afraid we're stuck.


A feat of escapology
--------------------

Ha! Fooled you! We're not stuck.
The solution is to pick the right overload dynamically,
using a virtual method (`Accept`) on the specifications themselves.
We also need to introduce an interface for the visitors,
so that we can add new visitors without affecting the `Accept` method.

```csharp
interface ISpecification<T>;
{
  TReturn Accept<TReturn>(
      ISpecificationVisitor<T, TReturn> visitor);
}
class AndSpecification<T>; : ISpecification<T>;
{
  // ...
  TReturn Accept<TReturn>(
      ISpecificationVisitor<T, TReturn> visitor)
  {
    // the compiler knows the concrete type of 'this',
    // so the correct overload of Visit will be selected
    return visitor.Visit(this);
  }
}
// identical Accept methods in all the other types of specification
```

Here are the visitors. Note that `ISpecificationVisitor`
has _two_ generic type parameters - one for the type of
specification the visitor operates on, and another for
the type of object the visitor returns.

```csharp
interface ISpecificationVisitor<T, out TReturn>;
{
  TReturn Visit(AtomicSpecification<T>; spec);
  TReturn Visit(AndSpecification<T>; spec);
  TReturn Visit(OrSpecification<T>; spec);
  TReturn Visit(NotSpecification<T>; spec);
}

class IsSatisfiedTester<T>; : ISpecificationVisitor<T, bool>
{
  private T candidate;
  public IsSatisfiedTester(T candidate)
  {
    this.candidate = candidate;
  }
  public bool Visit(AtomicSpecification<T>; spec)
  {
    return spec.IsSatisfiedBy(this.candidate);
  }
  public bool Visit(AndSpecification<T>; spec)
  {
    return spec.Left.Accept(this) && spec.Right.Accept(this);
  }
  public bool Visit(OrSpecification<T>; spec)
  {
    return spec.Left.Accept(this)
        || spec.Right.Accept(this);
  }
  public bool Visit(NotSpecification<T>; spec)
  {
    return !spec.Spec.Accept(this);
  }
}
class PrettyPrinter<T>; : ISpecificationVisitor<T, string>
{
  public string Visit(AtomicSpecification<T>; spec)
  {
    return spec.GetType().Name;
  }
  public string Visit(AndSpecification<T>; spec)
  {
    return string.Format("({0} AND {1})",
        spec.Left.Accept(this), spec.Right.Accept(this));
  }
  public string Visit(OrSpecification<T>; spec)
  {
    return string.Format("({0} OR {1})",
        spec.Left.Accept(this), spec.Right.Accept(this));
  }
  public string Visit(NotSpecification<T>; spec)
  {
    return string.Format("(NOT {0})", spec.Spec.Accept(this));
  }
}
```

In our first attempt, the visitors recursively called
themselves to traverse the specification tree.
Now, that recursive call is implemented using the
specification's `Accept` method.


Recovering the original interface
---------------------------------

Since I think `visitor.Visit(spec)` reads better than `spec.Accept(visitor)`,
I'm going to provide an extension method that converts between the two forms:

```csharp
static TReturn Visit<TCandidate, TReturn>(
    this ISpecificationVisitor<TCandidate, TReturn> visitor,
    ISpecification<TCandidate> spec)
{
  return spec.Accept(visitor);
}
```

Finally, we can write more extension methods to get back all the
methods that were previously declared in `ISpecification`:

```csharp
static bool IsSatisfiedBy<T>;(this ISpecification<T>; spec, T candidate)
{
  var visitor = new IsSatisfiedTester<T>;(candidate);
  return visitor.Visit(spec);
}
static string PrettyPrint<T>;(this ISpecification<T>; spec)
{
  var visitor = new PrettyPrinter<T>;();
  return visitor.Visit(spec);
}
```

We can continue replacing all the methods on `ISpecification`
with extension methods which encapsulate a visitor,
so we don't have to change any client code.


Code review
-----------

I mentioned earlier that object-oriented programming makes
it easy to add new types of data but hard to add new behaviours.
This is because OO languages encourage you to put data and behaviour together
in a class; we had to put a lot of machinery in place to make C# allow us to
separate them.
This is the only refactoring I've shown you which hasn't
made the code markedly simpler. In particular, the technique of picking
the right overload of `Visit` by overriding `Accept` is arcane and bulky:
we've paid the price of going against The Object-Oriented Way.

On the other hand, we're now free to define as many ways to traverse a
specification tree as we like, without even recompiling any pre-existing code.
So the Visitor pattern is useful when the benefit of being able to add
new behaviours outweighs the cost of the complicated code.
This is only true when there are lots of behaviours associated with your data.

Functional programming, unlike OO, encourages you to
separate your program's data from its behaviour.
This way, it's easy to add new types of behaviour
(just add another function) but hard to add new types of data
(you have to change all the existing functions).
[_Pattern matching_](http://stackoverflow.com/a/2502787/1523776)
is a feature of many functional languages which makes the
Visitor pattern so simple it's hardly visible.
Here's a translation of this post's example into
[Scala](http://docs.scala-lang.org/tutorials/tour/pattern-matching.html):

```scala
def isSatisfied[T](spec : Specification[T], x : T) : Boolean =
  spec match {
    case AndSpecification(l, r) => isSatisfied(l, x) && isSatisfied(r, x)
    case OrSpecification(l, r) => isSatisfied(l, x) || isSatisfied(r, x)
    case NotSpecification(s) => !isSatisfied(s, x)
    case s : AtomicSpecification[T] => s.isSatisfiedBy(x)
}
```

_That's the whole thing_ - it's a single function!
All our hard work - the interface containing four overloads,
the confusing `Accept` method, the pages of code -
was just to get around C#'s lack of support for pattern matching.

This series of blog posts has been all about the power of
repeatedly applying abstractions to your code-base.
Often, one improvement will open doors to another:
for example, putting security rules behind the `ISpecification`
interface gave us room to break our specifications down and
recombine them flexibly. The techniques I've demonstrated can
be applied any time you can recognise a number of classes
fulfilling the same role and abstract away the differences.

For further reading, see Eric Evans and Martin Fowler's
[article](http://martinfowler.com/apsupp/spec.pdf)
introducing the Specification pattern, and Robert C. Martin's
[succinct explanation](http://butunclebob.com/ArticleS.UncleBob.IuseVisitor)
of Visitor.

In this series
--------------

1. [All about security](All-about-security.md)
2. [The power of Composite Specifications](Composite-specifications.md)
3. [Specifications 3: The DSL Strikes Back](Specifications-dsl.md)
4. **Knock knock. Who's there? AbstractSpecificationNodeVisitorImpl**
