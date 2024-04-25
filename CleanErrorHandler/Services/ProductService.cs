using System;
using System.Collections.Generic;
using System.Linq;
using CleanErrorHandler.Contracts;

namespace CleanErrorHandler.Services;

public interface IProductService
{
    IEnumerable<ProductResponse> GetProducts();
    ProductResponse? GetProductById(int id);
}

public class ProductService : IProductService
{
    public IEnumerable<ProductResponse> GetProducts()
    {
        var data = Enumerable.Range(1, 10)
            .Select(index => new ProductResponse(index, $"Product {index}", index * 100));
        return data;
    }

    public ProductResponse? GetProductById(int id)
    {
        if (id == 10)
        {
            // Global exception handler will handle this exception
            // ProbelmDetails Customization will not be applied here
            throw new Exception("Product id must be between 1 and 10");
        }
        if (id == 15)
        {
            // ProbelmDetails Customization will be applied for the NotFound response
            return null;
        }

        return new ProductResponse(id, $"Product {id}", id * 100);
    }
}