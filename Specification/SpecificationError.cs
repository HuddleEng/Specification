using System;

namespace Specification
{
    public class SpecificationError
    {
        public SpecificationError(Exception exception, Type source)
        {
            Exception = exception ?? throw new ArgumentNullException(nameof(exception));
            Source = source ?? throw new ArgumentNullException(nameof(source));
        }

        public Exception Exception { get; }
        public Type Source { get; }

        public void Throw()
        {
            throw Exception;
        }
    }
}