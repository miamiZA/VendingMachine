using System;
using System.Collections.Generic;
using System.Text;

namespace UFF.VendingMachine.Models
{
    public class StockItem
    {
        private string _key;
        public string Key
        {
            get => _key;
            set => _key = value.ToUpper().Trim();
        }
        public StockPrice Price { get; set; }
        public string DisplayName { get; set; }
        public override string ToString() => $"'{Key}' Price:[{Price}]  '{DisplayName}'";
        public override bool Equals(object obj)
        {
            if (obj is StockItem p)
            {
                return _key == p.Key && Price == p.Price && DisplayName == p.DisplayName;
            }
            return false;
        }
        public override int GetHashCode()
        {
            return _key.GetHashCode();
        }
    }
}
