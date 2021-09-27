using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UFF.VendingMachine.Interface;

namespace UFF.VendingMachine
{
    public class PurseRepo : IPurseRepo
    {
        protected readonly Dictionary<INotionalValue, int> Repo = new Dictionary<INotionalValue, int>();

        public List<(INotionalValue, int)> ItemList => Repo.Where(r => r.Value > 0).Select(r => (r.Key, r.Value)).ToList();

        #region IPurseRepo interface

        public int AddItem(INotionalValue notionalValue, int number)
        {
            if (notionalValue == null)
            {
                throw new ArgumentNullException(nameof(notionalValue));
            }
            if (number < 0)
            {
                throw new InvalidDataException(nameof(number));
            }
            if (Repo.ContainsKey(notionalValue))
            {
                Repo[notionalValue] += number;
            }
            else
            {
                Repo[notionalValue] = number;
            }
            return Repo[notionalValue];
        }

        public int RemoveItem(INotionalValue notionalValue, int number)
        {
            if (notionalValue == null)
            {
                throw new ArgumentNullException(nameof(notionalValue));
            }
            if (!Repo.ContainsKey(notionalValue))
            {
                throw new InvalidDataException(nameof(notionalValue));
            }
            if ((number < 0) || (number > Repo[notionalValue]))
            {
                throw new InvalidDataException(nameof(number));
            }
            return Repo[notionalValue] -= number;
        }

        public void ClearItems()
        {
            Repo.Clear();
        }
        #endregion
    }
    public class PaymentPurseRepo : PurseRepo, IPurseRepo
    {
        public new void ClearItems()
        {
            ItemList.Clear();
        }
    }
}
