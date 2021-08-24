namespace Specification.SpecificationTree
{
    public class OrSpecification<T> : ISpecification<T>
    {
        public ISpecification<T> Left { get; private set; }
        public ISpecification<T> Right { get; private set; }

        public OrSpecification(ISpecification<T> left, ISpecification<T> right)
        {
            Left = left;
            Right = right;
        }

        public TReturn Accept<TReturn>(ISpecificationVisitor<T, TReturn> visitor)
        {
            return visitor.Visit(this);
        }
    }
}