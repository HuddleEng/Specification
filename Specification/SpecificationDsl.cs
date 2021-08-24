using System;
using Specification.SpecificationTree;

namespace Specification
{
    public static class SpecificationDsl
    {
        public static ISpecification<T> WithError<T>(this ISpecification<T> spec, Exception exc)
        {
            return new MessageOverridingSpecification<T>(exc, spec);
        }

        public static ISpecification<T> And<T>(this ISpecification<T> left, ISpecification<T> right)
        {
            return new AndSpecification<T>(left, right);
        }
        public static ISpecification<T> Or<T>(this ISpecification<T> left, ISpecification<T> right)
        {
            return new OrSpecification<T>(left, right);
        }

        public static ISpecification<T> AndNot<T>(this ISpecification<T> left, ISpecification<T> right)
        {
            return left.And(new NotSpecification<T>(right));
        }
        public static ISpecification<T> OrNot<T>(this ISpecification<T> left, ISpecification<T> right)
        {
            return left.Or(new NotSpecification<T>(right));
        }
    }

    public static class Not
    {
        public static ISpecification<T> This<T>(ISpecification<T> spec)
        {
            return new NotSpecification<T>(spec);
        }
    }
}