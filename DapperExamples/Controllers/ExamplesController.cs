﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Dapper;
using DapperExamples.Model;
using DapperExamples.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DapperExamples.Controllers
{
    [Route("[controller]")]
    public class ExamplesController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public ExamplesController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        /// <summary>
        /// Get product by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Product, OrdersDetail</returns>
        [HttpGet]
        [Route("getProductByIdAndOrders"), ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
        public IActionResult GetOneProductAndOrders(int id)
        {
            Product produto = null;
            using (var cn = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                cn.Open();

                var query = @"
SELECT * FROM Products WHERE ProductID = @Id
SELECT * FROM OrderDetails WHERE ProductID = @Id
";
                using (var result = cn.QueryMultiple(query, new { id }))
                {
                    produto = result.Read<Product>().Single();
                    produto.OrderDetails = result.Read<OrderDetail>().ToList();
                }
                return Ok(produto);
            }
        }

        /// <summary>
        /// Get products by name
        /// </summary>
        /// <param name="name">default: chef</param>
        /// <returns>Product, OrdersDetail</returns>
        [HttpGet]
        [Route("getProductsByName")]
        public IActionResult GetProductsByName(string name = "chef")
        {
            List<Product> produto = null;
            name = $"%{name}%";
            using (var cn = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                cn.Open();

                var query = @"
SELECT * FROM Products WHERE ProductName like @name
SELECT OrderDetails.* FROM OrderDetails JOIN Products ON OrderDetails.ProductId = Products.ProductId WHERE Products.ProductName like @name
";
                using (var result = cn.QueryMultiple(query, new { name }))
                {
                    produto = result.Read<Product>().ToList();
                    produto.Map(result.Read<OrderDetail>(), prod => prod.ProductId, order => order.ProductId, (prod, details) => prod.OrderDetails = details.ToList());
                }
                return Ok(produto);
            }
        }

        /// <summary>
        /// Get products by name
        /// </summary>
        /// <param name="name">default: chef</param>
        /// <returns>Product, OrdersDetail</returns>
        [HttpGet]
        [Route("queryMultiplePerformance")]
        public IActionResult QueryMultiplePerformance()
        {
            List<Product> produto = null;
            var name = $"%chef%";
            var stop = Stopwatch.StartNew();

            for (int i = 0; i < 5000; i++)
            {

                using (var cn = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
                {
                    cn.Open();

                    var query = @"
SELECT * FROM Products WHERE ProductName like @name
SELECT OrderDetails.* FROM OrderDetails JOIN Products ON OrderDetails.ProductId = Products.ProductId WHERE Products.ProductName like @name
";
                    using (var result = cn.QueryMultiple(query, new { name }))
                    {
                        produto = result.Read<Product>().ToList();
                        produto.Map(result.Read<OrderDetail>(), prod => prod.ProductId, order => order.ProductId, (prod, details) => prod.OrderDetails = details.ToList());
                    }
                }
            }

            stop.Stop();
            return Ok(stop.ElapsedMilliseconds);
        }

        /// <summary>
        /// Get products by name
        /// </summary>
        /// <param name="name">default: chef</param>
        /// <returns>Product, OrdersDetail</returns>
        [HttpGet]
        [Route("multiMappingMultiplePerformance")]
        public IActionResult MultiMappingMultiplePerformance()
        {
            var name = $"%chef%";
            var stop = Stopwatch.StartNew();

            for (int i = 0; i < 5000; i++)
            {

                using (var cn = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
                {
                    cn.Open();

                    var productDictionary = new Dictionary<int, Product>();
                    var query = @"SELECT * FROM Products JOIN OrderDetails ON OrderDetails.ProductId = Products.ProductId WHERE Products.ProductName like @name";
                    var list = cn.Query<Product, OrderDetail, Product>(query,
        (product, orderDetail) =>
        {
            Product productEntry;

            if (!productDictionary.TryGetValue(product.ProductId, out productEntry))
            {
                productEntry = product;
                productEntry.OrderDetails = new List<OrderDetail>();
                productDictionary.Add(productEntry.ProductId, productEntry);
            }
            productEntry.OrderDetails.Add(orderDetail);
            return productEntry;
        }, new { name },
        splitOn: "OrderID")
    .Distinct()
    .ToList();
                }
            }

            stop.Stop();
            return Ok(stop.ElapsedMilliseconds);
        }
    }
}