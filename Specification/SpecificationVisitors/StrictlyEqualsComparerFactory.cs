using Specification.SpecificationTree;

namespace Specification.SpecificationVisitors
{
    class StrictlyEqualsComparerFactory<T> : ISpecificationVisitor<T, ISpecificationVisitor<T, bool>>
    {
        public ISpecificationVisitor<T, bool> Visit(AndSpecification<T> specification)
        {
            return new StrictAndSpecificationComparer<T>(specification);
        }

        public ISpecificationVisitor<T, bool> Visit(OrSpecification<T> specification)
        {
            return new StrictOrSpecificationComparer<T>(specification);
        }

        public ISpecificationVisitor<T, bool> Visit(NotSpecification<T> specification)
        {
            return new StrictNotSpecificationComparer<T>(specification);
        }

        public ISpecificationVisitor<T, bool> Visit(LeafSpecification<T> specification)
        {
            return new StrictLeafSpecificationComparer<T>(specification);
        }

        public ISpecificationVisitor<T, bool> Visit(MessageOverridingSpecification<T> specification)
        {
            return new StrictMessageOverridingSpecificationComparer<T>(specification);
        }


        private class StrictAndSpecificationComparer<T2> : ConstantVisitor<T2, bool>
        {
            readonly AndSpecification<T2> _left;

            public StrictAndSpecificationComparer(AndSpecification<T2> left)
                : base(false)
            {
                _left = left;
            }

            public override bool Visit(AndSpecification<T2> right)
            {
                return _left.Left.StrictlyEquals(right.Left)
                    && _left.Right.StrictlyEquals(right.Right);
            }
        }

        private class StrictOrSpecificationComparer<T2> : ConstantVisitor<T2, bool>
        {
            readonly OrSpecification<T2> _left;

            public StrictOrSpecificationComparer(OrSpecification<T2> left)
                : base(false)
            {
                _left = left;
            }

            public override bool Visit(OrSpecification<T2> right)
            {
                return _left.Left.StrictlyEquals(right.Left)
                    && _left.Right.StrictlyEquals(right.Right);
            }
        }

        private class StrictNotSpecificationComparer<T2> : ConstantVisitor<T2, bool>
        {
            readonly NotSpecification<T2> _left;

            public StrictNotSpecificationComparer(NotSpecification<T2> left)
                : base(false)
            {
                _left = left;
            }

            public override bool Visit(NotSpecification<T2> right)
            {
                return _left.Spec.StrictlyEquals(right.Spec);
            }
        }

        private class StrictLeafSpecificationComparer<T2> : ConstantVisitor<T2, bool>
        {
            readonly LeafSpecification<T2> _left;

            public StrictLeafSpecificationComparer(LeafSpecification<T2> left)
                : base(false)
            {
                _left = left;
            }

            public override bool Visit(LeafSpecification<T2> right)
            {
                return _left.Equals(right);
            }
        }

        private class StrictMessageOverridingSpecificationComparer<T2> : ConstantVisitor<T2, bool>
        {
            readonly MessageOverridingSpecification<T2> _left;

            public StrictMessageOverridingSpecificationComparer(MessageOverridingSpecification<T2> left)
                : base(false)
            {
                _left = left;
            }

            public override bool Visit(MessageOverridingSpecification<T2> right)
            {
                return _left.Spec.StrictlyEquals(right.Spec)
                    && _left.Error.Exception.GetType() == right.Error.Exception.GetType()
                    && _left.Error.Exception.Message == right.Error.Exception.Message;
            }
        }
    }
}
