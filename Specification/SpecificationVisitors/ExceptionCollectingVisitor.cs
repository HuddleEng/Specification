using System;
using System.Collections.Generic;
using System.Linq;
using Specification.SpecificationTree;

namespace Specification.SpecificationVisitors
{
    public class ExceptionCollectingVisitor<TCandidate> : ISpecificationVisitor<TCandidate, IEnumerable<SpecificationError>>
    {
        readonly TCandidate _candidate;

        public ExceptionCollectingVisitor(TCandidate candidate)
        {
            _candidate = candidate;
        }

        public IEnumerable<SpecificationError> Visit(AndSpecification<TCandidate> specification)
        {
            var left = this.Visit(specification.Left);
            var right = this.Visit(specification.Right);

            return left.Concat(right);
        }

        public IEnumerable<SpecificationError> Visit(OrSpecification<TCandidate> specification)
        {
            // If either of them passed (returned no errors), then we pass (return no errors).
            // Otherwise return all the errors.

            var left = this.Visit(specification.Left);
            if (!left.Any())
            {
                return Enumerable.Empty<SpecificationError>();
            }

            var right = this.Visit(specification.Right);
            if (!right.Any())
            {
                return Enumerable.Empty<SpecificationError>();
            }

            return left.Concat(right);
        }

        public IEnumerable<SpecificationError> Visit(NotSpecification<TCandidate> specification)
        {
            var errors = this.Visit(specification.Spec);
            if (!errors.Any())
            {
                return Singleton(new SpecificationError(new Exception("A NotSpecification was not satisfied"), typeof(NotSpecification<TCandidate>)));
            }
            return Enumerable.Empty<SpecificationError>();
        }

        public IEnumerable<SpecificationError> Visit(LeafSpecification<TCandidate> specification)
        {
            if (specification.IsSatisfiedBy(_candidate))
            {
                return Enumerable.Empty<SpecificationError>();
            }

            var defaultException = new SpecificationError(new Exception("You do not have permission to perform that action"), specification.GetType());
            if (specification.Error == null)
            {
                return Singleton(defaultException);
            }
            if (specification.Error.Exception == null)
            {
                return Singleton(defaultException);
            }
            return Singleton(specification.Error);
        }
        
        private static IEnumerable<T> Singleton<T>(T t)
        {
            return new[] { t };
        }
    }
}