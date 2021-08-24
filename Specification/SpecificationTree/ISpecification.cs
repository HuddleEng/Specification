namespace Specification.SpecificationTree
{
    // Specifications are explained in detail in my series of posts for the Huddle Dev Blog:
    // All-about-security/
    public interface ISpecification<TCandidate>
    {
        TReturn Accept<TReturn>(ISpecificationVisitor<TCandidate, TReturn> visitor);
    }
}