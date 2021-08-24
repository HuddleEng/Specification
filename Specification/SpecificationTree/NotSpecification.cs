namespace Specification.SpecificationTree
{
    public class NotSpecification<T> : ISpecification<T>
    {
        public ISpecification<T> Spec { get; private set; }

        public NotSpecification(ISpecification<T> spec)
        {
            Spec = spec;
        }

        public TReturn Accept<TReturn>(ISpecificationVisitor<T, TReturn> visitor)
        {
            return visitor.Visit(this);
        }
    }
}