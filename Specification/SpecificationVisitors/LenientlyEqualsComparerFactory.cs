using Specification.SpecificationTree;

namespace Specification.SpecificationVisitors
{
    public class LenientlyEqualsComparerFactory<T> : ISpecificationVisitor<T, ISpecificationVisitor<T, bool>>
    {
        public ISpecificationVisitor<T, bool> Visit(AndSpecification<T> specification)
        {
            return new LenientAndSpecificationComparer<T>(specification);
        }

        public ISpecificationVisitor<T, bool> Visit(OrSpecification<T> specification)
        {
            return new LenientOrSpecificationComparer<T>(specification);
        }

        public ISpecificationVisitor<T, bool> Visit(NotSpecification<T> specification)
        {
            return new LenientNotSpecificationComparer<T>(specification);
        }

        public ISpecificationVisitor<T, bool> Visit(LeafSpecification<T> specification)
        {
            return new LenientLeafSpecificationComparer<T>(specification);
        }

        public ISpecificationVisitor<T, bool> Visit(MessageOverridingSpecification<T> specification)
        {
            return new LenientMessageOverridingSpecificationComparer<T>(specification);
        }

        private class LenientAndSpecificationComparer<T2> : ConstantVisitor<T2, bool>
        {
            readonly AndSpecification<T2> _left;

            public LenientAndSpecificationComparer(AndSpecification<T2> left) : base(false)
            {
                _left = left;
            }

            public override bool Visit(AndSpecification<T2> right)
            {
                return (_left.Left.LenientlyEquals(right.Left) && _left.Right.LenientlyEquals(right.Right))
                    || (_left.Left.LenientlyEquals(right.Right) && _left.Right.LenientlyEquals(right.Left));
            }
        }

        private class LenientOrSpecificationComparer<T2> : ConstantVisitor<T2, bool>
        {
            readonly OrSpecification<T2> _left;

            public LenientOrSpecificationComparer(OrSpecification<T2> left) : base(false)
            {
                _left = left;
            }

            public override bool Visit(OrSpecification<T2> right)
            {
                return (_left.Left.LenientlyEquals(right.Left) && _left.Right.LenientlyEquals(right.Right))
                    || (_left.Left.LenientlyEquals(right.Right) && _left.Right.LenientlyEquals(right.Left));
            }
        }

        private class LenientNotSpecificationComparer<T2> : ConstantVisitor<T2, bool>
        {
            readonly NotSpecification<T2> _left;

            public LenientNotSpecificationComparer(NotSpecification<T2> left) : base(false)
            {
                _left = left;
            }

            public override bool Visit(NotSpecification<T2> right)
            {
                return _left.Spec.LenientlyEquals(right.Spec);
            }
        }

        private class LenientLeafSpecificationComparer<T2> : ConstantVisitor<T2, bool>
        {
            readonly LeafSpecification<T2> _left;

            public LenientLeafSpecificationComparer(LeafSpecification<T2> left) : base(false)
            {
                _left = left;
            }

            public override bool Visit(LeafSpecification<T2> right)
            {
                return _left.Equals(right);
            }
        }

        private class LenientMessageOverridingSpecificationComparer<T2> : ConstantVisitor<T2, bool>
        {
            readonly MessageOverridingSpecification<T2> _left;

            public LenientMessageOverridingSpecificationComparer(MessageOverridingSpecification<T2> left) : base(false)
            {
                _left = left;
            }

            public override bool Visit(MessageOverridingSpecification<T2> right)
            {
                return _left.Spec.LenientlyEquals(right.Spec)
                    && _left.Error.Exception.GetType() == right.Error.Exception.GetType()
                    && _left.Error.Exception.Message == right.Error.Exception.Message;
            }
        }
    }
}