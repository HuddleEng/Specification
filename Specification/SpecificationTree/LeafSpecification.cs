using System;

namespace Specification.SpecificationTree
{
    public abstract class LeafSpecification<T> : ISpecification<T>
    {
        protected LeafSpecification()
        { }

        protected LeafSpecification(Exception defaultException)
        {
            Error = new SpecificationError(defaultException, GetType());
        }

        public SpecificationError Error { get; private set; }

        public TReturn Accept<TReturn>(ISpecificationVisitor<T, TReturn> visitor)
        {
            return visitor.Visit(this);
        }

        public abstract bool IsSatisfiedBy(T context);

        protected void StoreError(Exception exception)
        {
            Error = new SpecificationError(exception, GetType());
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            return GetType() == obj.GetType();
        }

        public override int GetHashCode()
        {
            return GetType().GetHashCode();
        }

        public static bool operator ==(LeafSpecification<T> left, LeafSpecification<T> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(LeafSpecification<T> left, LeafSpecification<T> right)
        {
            return !Equals(left, right);
        }
    }
}