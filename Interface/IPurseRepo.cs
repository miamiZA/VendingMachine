using System;
using System.Collections.Generic;
using System.Text;

namespace UFF.VendingMachine.Interface
{
    public interface IPurseRepo
    {
        List<(INotionalValue, int)> ItemList 
        { 
            get; 
        }
        /// <summary>
        /// Remove from Item List
        /// </summary>
        /// <param name="notionalValue"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        int RemoveItem(INotionalValue notionalValue, int number);

        /// <summary>
        /// Add to Item List.
        /// </summary>
        /// <param name="notionalValue"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        int AddItem(INotionalValue notionalValue, int number);
        void ClearItems();
    }
}
