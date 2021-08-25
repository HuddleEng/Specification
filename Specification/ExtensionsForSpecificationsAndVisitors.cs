using System;
using System.Linq;
using Specification.SpecificationTree;
using Specification.SpecificationVisitors;

namespace Specification
{
    public static class ExtensionsForSpecificationsAndVisitors
    {
        public static bool IsSatisfiedBy<T>(this ISpecification<T> spec, T candidate)
        {
            return spec.Fold(
                foldAnd: (l, r) => l && r,
                foldOr: (l, r) => l || r,
                foldNot: (x) => !x,
                foldLeaf: s => s.IsSatisfiedBy(candidate)
            );
        }

        public static void ThrowIfNotSatisfiedBy<T>(this ISpecification<T> spec, T candidate)
        {
            var visitor = new ExceptionCollectingVisitor<T>(candidate);
            var errors = visitor.Visit(spec);
            if (errors.Any())
            {
                errors.First().Throw();
            }
        }

        public static bool StrictlyEquals<T>(this ISpecification<T> left, ISpecification<T> right)
        {
            var visitor = GetStrictlyEqualsComparer(left);
            return visitor.Visit(right);
        }

        static ISpecificationVisitor<T, bool> GetStrictlyEqualsComparer<T>(ISpecification<T> spec)
        {
            var visitor = new StrictlyEqualsComparerFactory<T>();
            return visitor.Visit(spec);
        }

        public static bool LenientlyEquals<T>(this ISpecification<T> left, ISpecification<T> right)
        {
            var visitor = GetLenientlyEqualsComparer(left);
            return visitor.Visit(right);
        }

        static ISpecificationVisitor<T, bool> GetLenientlyEqualsComparer<T>(ISpecification<T> spec)
        {
            var visitor = new LenientlyEqualsComparerFactory<T>();
            return visitor.Visit(spec);
        }

        public static string PrettyPrint<T>(this ISpecification<T> spec)
        {
            return spec.Fold(
                foldAnd: (l, r) => $"({l} AND {r})",
                foldOr: (l, r) => $"({l} OR {r})",
                foldNot: x => $"(NOT {x})",
                foldLeaf: s => s.GetType().Name);
        }

        public static TReturn Visit<TCandidate, TReturn>(this ISpecificationVisitor<TCandidate, TReturn> visitor, ISpecification<TCandidate> spec)
        {
            return spec.Accept(visitor);
        }

        public static TReturn Fold<TCandidate, TReturn>(
            this ISpecification<TCandidate> spec,
            Func<TReturn, TReturn, TReturn> foldAnd,
            Func<TReturn, TReturn, TReturn> foldOr,
            Func<TReturn, TReturn> foldNot,
            Func<LeafSpecification<TCandidate>, TReturn> foldLeaf)
        {
            var visitor = new SpecificationFold<TCandidate, TReturn>(foldAnd, foldOr, foldNot, foldLeaf);
            return visitor.Visit(spec);
        }
    }
}