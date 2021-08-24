using System;
using Specification.SpecificationTree;

namespace SpecificationTests
{
    public class FailingSpecification<T> : LeafSpecification<T>
    {
        private readonly Exception _exception;

        public FailingSpecification()
        {
            _exception = new Exception("It went awry");
        }

        public FailingSpecification(Exception exception) : base(exception)
        {
            _exception = exception;
        }

        public T Context { get; private set; }

        public override bool IsSatisfiedBy(T context)
        {
            StoreError(_exception);
            Context = context;
            return false;
        }
    }
}