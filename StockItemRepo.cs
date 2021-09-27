using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UFF.VendingMachine.Interface;
using UFF.VendingMachine.Models;

namespace UFF.VendingMachine
{
    public class StockItemRepo : IStockItemRepo
    {
        readonly Dictionary<string, (StockItem, int)> Repo = new Dictionary<string, (StockItem, int)>();

        public List<StockItem> StockItemList => Repo.Values.Select(p => p.Item1).ToList();

        #region IStockItemRepo interface

        /// <summary>
        /// Update Item
        /// </summary>
        /// <param name="stockItem"></param>
        /// <returns></returns>
        public bool UpdateItem(StockItem stockItem)
        {
            if (stockItem == null)
            {
                throw new ArgumentNullException(nameof(stockItem));
            }
            bool update = !Repo.ContainsKey(stockItem.Key);
            Repo[stockItem.Key] = (stockItem, 0);
            return update;
        }

        public int AddItem(string key, int number)
        {
            key = key.ToUpper();
            if (number < 0)
            {
                throw new InvalidDataException(nameof(number));
            }
            if (!Repo.ContainsKey(key))
            {
                throw new InvalidDataException(nameof(key));

            }
            (StockItem p, int oldnumber) = Repo[key];
            oldnumber += number;
            Repo[key] = (p, oldnumber);
            return oldnumber;
        }

        public int StockCount(string key)
        {
            key = key.ToUpper();
            if (Repo.ContainsKey(key))
            {
                return Repo[key].Item2;
            }
            return 0;
        }

        public int RemoveItem(string key)
        {
            key = key.ToUpper();
            if (!Repo.ContainsKey(key))
            {
                throw new InvalidDataException(nameof(key));

            }
            (StockItem p, int number) = Repo[key];
            if (number == 0)
            {
                throw new InvalidDataException(nameof(number));
            }
            number--;
            Repo[key] = (p, number);
            return number;
        }

        #endregion

    }
}
