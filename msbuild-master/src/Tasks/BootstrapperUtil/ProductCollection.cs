﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Microsoft.Build.Tasks.Deployment.Bootstrapper
{
    /// <summary>
    /// This class contains a collection of Product objects. This collection is a closed set that is generated by the BootstrapperBuilder based on the Path property. The client cannot add or remove items from this collection.
    /// </summary>
    [ComVisible(true), Guid("EFFA164B-3E87-4195-88DB-8AC004DDFE2A"), ClassInterface(ClassInterfaceType.None)]
    public class ProductCollection : IProductCollection, IEnumerable
    {
        private readonly List<Product> _list = new List<Product>();
        private readonly Dictionary<string, Product> _table = new Dictionary<string, Product>(StringComparer.OrdinalIgnoreCase);

        internal ProductCollection()
        {
        }

        internal void Add(Product product)
        {
            if (!_table.ContainsKey(product.ProductCode))
            {
                _list.Add(product);
                _table.Add(product.ProductCode, product);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Product '{0}' has already been added to the product list", product.ProductCode.ToUpperInvariant());
            }
        }

        /// <summary>
        /// Gets the Product at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get</param>
        /// <returns>The Product at the specified index</returns>
        public Product Item(int index)
        {
            return _list[index];
        }

        /// <summary>
        /// Gets the product with the specified product code
        /// </summary>
        /// <param name="productCode"></param>
        /// <returns>The product with the given name, null if the spercified product code is not found</returns>
        public Product Product(string productCode)
        {
            _table.TryGetValue(productCode, out Product product);
            return product;
        }

        /// <summary>
        /// Gets the number of elements actually contained in the ProductCollection
        /// </summary>
        public int Count => _list.Count;

        internal void Clear()
        {
            _list.Clear();
            _table.Clear();
        }

        /// <summary>
        /// Returns an enumerator that can iterate through the ProductCollection
        /// </summary>
        /// <returns>An enumerator that can iterate through the ProductCollection</returns>
        public IEnumerator GetEnumerator()
        {
            return _list.GetEnumerator();
        }
    }
}
