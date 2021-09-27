using System;
using System.Collections.Generic;
using System.Text;
using UFF.VendingMachine.Models;

namespace UFF.VendingMachine.Interface
{
    public interface INotionalValue
    {
        /// <summary>
        /// Nominal
        /// </summary>
        int Nominal 
        { 
            get; 
        }

        /// <summary>
        /// StockPriceValue
        /// </summary>
        StockPrice StockPriceValue 
        { 
            get; 
        }
    }
}
