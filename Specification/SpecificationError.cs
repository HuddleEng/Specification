using System;
using System.Collections.Generic;
using System.Linq;

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

        public override string ToString()
        {
            return GetType().Name + ": (" + Exception.Message + ")";
        }
    }


    public class SpecificationAggregateException : AggregateException
    {


        public SpecificationAggregateException(IEnumerable<SpecificationError> errors)
        {
            Errors = errors;
        }

        public IEnumerable<SpecificationError> Errors { get; }

        public SpecificationError GetFirstException<T>()
        {
            return Errors.SingleOrDefault(ex => ex.Exception is T);
        }
    }
}