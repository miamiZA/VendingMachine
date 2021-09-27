using System;
using UFF.VendingMachine.Models;

namespace UFF.VendingMachine.Interface
{
    public class Coin : INotionalValue
    {
       
        public int Nominal { get; }      
        public StockPrice StockPriceValue { get; }
        public Coin(int n)
        {
            Nominal = n;
            StockPriceValue = new StockPrice(0, (int)n);
        }
        public override bool Equals(object o)
        {
            if (o is Coin m)
            {
                return Nominal == m.Nominal;
            }
            return false;
        }   
        public override int GetHashCode()
        {
            return Nominal.GetHashCode();
        }
        public override string ToString()
        {
            return String.Format(".{0}", Nominal.ToString("D2"));
        }
    }
}
