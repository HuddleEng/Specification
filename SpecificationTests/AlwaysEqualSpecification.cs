using Specification.SpecificationTree;

namespace SpecificationTests
{
    public class AlwaysEqualSpecification<T> : LeafSpecification<T>
    {
        public override bool IsSatisfiedBy(T context)
        {
            return false;
        }

        public override bool Equals(object obj)
        {
            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(AlwaysEqualSpecification<T> left, AlwaysEqualSpecification<T> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(AlwaysEqualSpecification<T> left, AlwaysEqualSpecification<T> right)
        {
            return !Equals(left, right);
        }
    }
}