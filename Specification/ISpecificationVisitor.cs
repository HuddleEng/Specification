using Specification.SpecificationTree;

namespace Specification
{
    public interface ISpecificationVisitor<TCandidate, out TReturn>
    {
        TReturn Visit(AndSpecification<TCandidate> specification);
        TReturn Visit(OrSpecification<TCandidate> specification);
        TReturn Visit(NotSpecification<TCandidate> specification);
        TReturn Visit(LeafSpecification<TCandidate> specification);
        TReturn Visit(MessageOverridingSpecification<TCandidate> specification);
    }
}