using Specification.SpecificationTree;

namespace SpecificationTests
{
    public class PassingSpecification<T> : LeafSpecification<T>
    {
        public T Context { get; private set; }

        public override bool IsSatisfiedBy(T context)
        {
            Context = context;
            return true;
        }
    }
}