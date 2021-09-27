
using System;
using System.IO;

namespace UFF.VendingMachine.Models
{
    public struct StockPrice
    {
        public int Number { get; }  
        public int Fraction { get; }      
        public static int FractionSize { get; set; } = 100;     
        public static StockPrice Zero => new StockPrice();

        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="number">The value of numbers</param>
        /// <param name="fraction">The value of fractions.</param>
        public StockPrice(int number, int fraction)
        {
            if (number < 0)
            {
                throw new InvalidDataException(nameof(number));
            }

            if (fraction < 0)
            {
                throw new InvalidDataException(nameof(fraction));
            }

            Number = number;
            Number += fraction / FractionSize;
            Fraction = fraction % FractionSize;
        }
        public int ToFraction() => Number * FractionSize + Fraction;   
        public static bool operator <(StockPrice m1, StockPrice m2) => m1.ToFraction() < m2.ToFraction();     
        public static bool operator >(StockPrice m1, StockPrice m2) => m1.ToFraction() > m2.ToFraction();     
        public static bool operator <=(StockPrice m1, StockPrice m2) => m1.ToFraction() <= m2.ToFraction();
        public static bool operator >=(StockPrice m1, StockPrice m2) => m1.ToFraction() >= m2.ToFraction();
        public static StockPrice operator +(StockPrice m1, StockPrice m2)
        {
            var fraction = m1.ToFraction() + m2.ToFraction();
            return new StockPrice(0, fraction);
        }
        public static StockPrice operator -(StockPrice m1, StockPrice m2)
        {
            var fraction = m1.ToFraction() - m2.ToFraction();
            return new StockPrice(0, fraction);
        }
        public static bool operator ==(StockPrice m1, StockPrice m2) => m1.Equals(m2);
        public static bool operator !=(StockPrice m1, StockPrice m2) => !m1.Equals(m2);
        public override bool Equals(object o)
        {
            if (o is StockPrice m)
            {
                return Number == m.Number && Fraction == m.Fraction;
            }
            return false;
        }
        public override int GetHashCode()
        {
            return Number.GetHashCode() ^ Fraction.GetHashCode();
        }
        public override string ToString()
        {
            return String.Format("{0}.{1}", Number, Fraction.ToString("D2"));
        }
    }
}
