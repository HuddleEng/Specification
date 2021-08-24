using System;
using Specification.SpecificationTree;

namespace Specification.SpecificationVisitors
{
    // Let's do some functional programming!
    // This class factors out the recursive structure of a specification visitor.
    // All you need to do is plug in functions to combine the results,
    // so you can visit a specification with very little code.
    public class SpecificationFold<TCandidate, TAccumulator> : ISpecificationVisitor<TCandidate, TAccumulator>
    {
        private readonly Func<TAccumulator, TAccumulator, TAccumulator> _foldAnd;
        private readonly Func<TAccumulator, TAccumulator, TAccumulator> _foldOr;
        private readonly Func<TAccumulator, TAccumulator> _foldNot;
        private readonly Func<LeafSpecification<TCandidate>, TAccumulator> _foldLeaf;
        private readonly Func<SpecificationError, TAccumulator, TAccumulator> _foldMessageOverride;

        public SpecificationFold(
            Func<TAccumulator, TAccumulator, TAccumulator> foldAnd,
            Func<TAccumulator, TAccumulator, TAccumulator> foldOr,
            Func<TAccumulator, TAccumulator> foldNot,
            Func<LeafSpecification<TCandidate>, TAccumulator> foldLeaf,
            Func<SpecificationError, TAccumulator, TAccumulator> foldMessageOverride)
        {
            _foldAnd = foldAnd;
            _foldOr = foldOr;
            _foldNot = foldNot;
            _foldLeaf = foldLeaf;
            _foldMessageOverride = foldMessageOverride;
        }

        public TAccumulator Visit(AndSpecification<TCandidate> specification)
        {
            return _foldAnd(this.Visit(specification.Left), this.Visit(specification.Right));
        }

        public TAccumulator Visit(OrSpecification<TCandidate> specification)
        {
            return _foldOr(this.Visit(specification.Left), this.Visit(specification.Right));
        }

        public TAccumulator Visit(NotSpecification<TCandidate> specification)
        {
            return _foldNot(this.Visit(specification.Spec));
        }

        public TAccumulator Visit(LeafSpecification<TCandidate> specification)
        {
            return _foldLeaf(specification);
        }

        public TAccumulator Visit(MessageOverridingSpecification<TCandidate> specification)
        {
            return _foldMessageOverride(specification.Error, this.Visit(specification.Spec));
        }
    }
}