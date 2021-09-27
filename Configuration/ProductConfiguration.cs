using System;
using System.Collections.Generic;
using System.Text;

namespace UFF.VendingMachine.Configuration
{
    /// POCO container maps back to configuation - StockConfiguration
    public class ProductConfiguration
    {

        /// <summary>
        /// Gets or sets the title of the product.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the cost of the product in fraction nominal.
        /// </summary>
        public int Cost { get; set; }

        /// <summary>
        /// The number of items in the stock
        /// </summary>
        public int NumberInStock { get; set; }
    }
}
