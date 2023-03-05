using System.Diagnostics.CodeAnalysis;

namespace Polygonal.Core
{
    public enum PeriodBasis { Minute, Hour, Day, Week, Month, Quarter, Year };

    public struct Period
    {
        public int Multiple { get; private set; }

        public PeriodBasis Basis { get; private set; }

        public Period(int multiple, PeriodBasis periodBasis)
        {
            Multiple = multiple;
            Basis = periodBasis;
        }

        public bool Equals(Period other)
        {
            return Multiple == other.Multiple && Basis == other.Basis;
        }

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            Period? other = obj as Period?;
            return other.HasValue && Equals(other.Value);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Multiple, Basis);
        }

        public override string ToString()
        {
            return String.Concat(Multiple.ToString(), " ", Basis.ToString());
        }
    }
}
