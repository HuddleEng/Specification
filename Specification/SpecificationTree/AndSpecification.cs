namespace Specification.SpecificationTree
{
    public class AndSpecification<T> : ISpecification<T>
    {
        public AndSpecification(ISpecification<T> left, ISpecification<T> right)
        {
            Left = left;
            Right = right;
        }

        public ISpecification<T> Left { get; }
        public ISpecification<T> Right { get; }

        public TReturn Accept<TReturn>(ISpecificationVisitor<T, TReturn> visitor)
        {
            return visitor.Visit(this);
        }
    }
}