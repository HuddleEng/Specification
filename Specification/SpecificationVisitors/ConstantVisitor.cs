using Specification.SpecificationTree;

namespace Specification.SpecificationVisitors
{
    public class ConstantVisitor<TCandidate, TReturn> : ISpecificationVisitor<TCandidate, TReturn>
    {
        readonly TReturn _cannedValue;

        public ConstantVisitor(TReturn cannedValue)
        {
            _cannedValue = cannedValue;
        }

        public virtual TReturn Visit(AndSpecification<TCandidate> specification)
        {
            return _cannedValue;
        }

        public virtual TReturn Visit(OrSpecification<TCandidate> specification)
        {
            return _cannedValue;
        }

        public virtual TReturn Visit(NotSpecification<TCandidate> specification)
        {
            return _cannedValue;
        }

        public virtual TReturn Visit(LeafSpecification<TCandidate> specification)
        {
            return _cannedValue;
        }

        public virtual TReturn Visit(MessageOverridingSpecification<TCandidate> specification)
        {
            return _cannedValue;
        }
    }
}