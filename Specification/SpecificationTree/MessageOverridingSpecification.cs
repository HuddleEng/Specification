using System;

namespace Specification.SpecificationTree
{
    public class MessageOverridingSpecification<T> : ISpecification<T>
    {
        public SpecificationError Error { get; private set; }
        public ISpecification<T> Spec { get; }

        public MessageOverridingSpecification(Exception exception, ISpecification<T> spec)
        {
            if (spec == null)
            {
                throw new ArgumentNullException(nameof(spec));
            }
            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }
            Spec = spec;
            Error = new SpecificationError(exception, Spec.GetType());
        }

        public TReturn Accept<TReturn>(ISpecificationVisitor<T, TReturn> visitor)
        {
            return visitor.Visit(this);
        }
    }
}