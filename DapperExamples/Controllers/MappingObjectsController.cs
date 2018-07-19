using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using DapperExamples.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DapperExamples.Controllers
{
    [Route("[controller]")]
    public class MappingObjectsController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public MappingObjectsController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpGet, Route("Typed")]
        public IActionResult StrongTyped()
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                var produto = connection.QueryFirst<Product>(@"SELECT Top 1 * FROM Products");
                return Ok(produto);
            }
        }

        [HttpGet, Route("Dynamic")]
        public IActionResult Dynamic()
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                var produto = connection.Query(@"SELECT TOP 10 * FROM Products JOIN  [Order Details] ON  [Order Details].ProductId = Products.ProductId");
                return Ok(produto);
            }
        }

        [HttpGet]
        [Route("MultiMapping")]
        public IActionResult MultiMapping()
        {
            var name = $"%chef%";
            using (var cn = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                var productDictionary = new Dictionary<int, Product>();
                var query = @"SELECT * FROM Products JOIN  [Order Details] ON [Order Details].ProductId = Products.ProductId WHERE Products.ProductName like @name";
                var list = cn.Query<Product, OrderDetail, Product>(
                        query,
                        (product, orderDetail) =>
                        {
                            if (!productDictionary.TryGetValue(product.ProductId, out var productEntry))
                            {
                                productEntry = product;
                                productEntry.OrderDetails = new List<OrderDetail>();
                                productDictionary.Add(productEntry.ProductId, productEntry);
                            }
                            productEntry.OrderDetails.Add(orderDetail);
                            return productEntry;
                        },
                        new { name },
                        splitOn: "OrderID")
                        .Distinct()
                        .ToList();

                return Ok(list);
            }

        }

    }
}