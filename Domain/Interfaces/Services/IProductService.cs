using System;
using System.Collections.Generic;
using System.Text;
using Xenon.Domain.Models;

namespace Xenon.Domain.Interfaces.Services
{
    
        public interface IProductService
        {
            Task<Product?> GetProductAsync(long id);
            Task<IEnumerable<Product?>> GetProductsAsync(int page, int pageSize, string category = null);
            Task<int> GetTotalCountAsync(string category = null);
        }
    
}
