using Dapper;
using DapperExamples.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace DapperExamples.Controllers
{
    /// <summary>
    /// One by one, examples from each Dapper extension
    /// </summary>
    [Route("[controller]")]
    public class MethodsController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public MethodsController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }


        [HttpGet, Route("Query")]
        public IActionResult Query()
        {
            IEnumerable<Product> produtos;
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                produtos = connection.Query<Product>(@"SELECT TOP 10 * FROM Products");
            }

            return Ok(produtos);
        }

        [HttpGet, Route("QueryAsync")]
        public async Task<IActionResult> QueryAsync()
        {
            IEnumerable<Product> produtos;
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                produtos = await connection.QueryAsync<Product>(@"SELECT TOP 10 * FROM Products");
            }

            return Ok(produtos);
        }

        [HttpGet, Route("Execute")]
        public IActionResult Execute()
        {
            var quantidade = 0;
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                quantidade = connection.Execute(@"UPDATE Products SET ProductName = 'Kolgate' WHERE ProductId = 1");
            }

            return Ok(quantidade);
        }

        [HttpGet, Route("ExecuteAsync")]
        public async Task<IActionResult> ExecuteAsync()
        {
            var quantidade = 0;
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                quantidade = await connection.ExecuteAsync(@"UPDATE Products SET ProductName = 'Chai' WHERE ProductId = 1");
            }

            return Ok(quantidade);
        }


        [HttpGet, Route("QueryFirst")]
        public IActionResult QueryFirst()
        {
            Product produto;
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                // For test pourposes, it's taking top 10. On a reasonable scenario that'll be a top 1.
                produto = connection.QueryFirst<Product>(@"SELECT TOP 10 * FROM Products");
            }

            return Ok(produto);
        }

        [HttpGet, Route("QueryFirstAsync")]
        public async Task<IActionResult> QueryFirstAsync()
        {
            Product produto;
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                // For test pourposes, it's taking top 10. On a reasonable scenario that'll be a top 1.
                produto = await connection.QueryFirstAsync<Product>(@"SELECT TOP 10 * FROM Products");
            }

            return Ok(produto);
        }

        [HttpGet, Route("QueryFirstOrDefault")]
        public IActionResult QueryFirstOrDefault()
        {
            Product produto;
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                // For test pourposes, it's taking top 10. On a reasonable scenario that'll be a top 1.
                produto = connection.QueryFirstOrDefault<Product>(@"SELECT TOP 10 * FROM Products");
            }

            return Ok(produto);
        }

        [HttpGet, Route("QueryFirstOrDefaultAsync")]
        public async Task<IActionResult> QueryFirstOrDefaultAsync()
        {
            Product produto;
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                // For test pourposes, it's taking top 10. On a reasonable scenario that'll be a top 1.
                produto = await connection.QueryFirstOrDefaultAsync<Product>(@"SELECT TOP 10 * FROM Products");
            }

            return Ok(produto);
        }

        [HttpGet, Route("QuerySingle")]
        public IActionResult QuerySingle()
        {
            Product produto;
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                // For test pourposes, it's taking top 10. On a reasonable scenario that'll be a top 1.
                produto = connection.QuerySingle<Product>(@"SELECT * FROM Products WHERE ProductID = @Id", param: new { id = 2 });
            }

            return Ok(produto);
        }

        [HttpGet, Route("QuerySingleAsync")]
        public async Task<IActionResult> QuerySingleAsync()
        {
            Product produto;
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                // For test pourposes, it's taking top 10. On a reasonable scenario that'll be a top 1.
                produto = await connection.QuerySingleAsync<Product>(@"SELECT * FROM Products WHERE ProductID = @Id", new { id = 2 });
            }

            return Ok(produto);
        }



        [HttpGet, Route("QuerySingleOrDefault")]
        public IActionResult QuerySingleOrDefault()
        {
            Product produto;
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                // For test pourposes, it's taking top 10. On a reasonable scenario that'll be a top 1.
                produto = connection.QuerySingle<Product>(@"SELECT * FROM Products WHERE ProductID = @Id", param: new { id = 2 });
            }

            return Ok(produto);
        }

        [HttpGet, Route("QuerySingleOrDefaultAsync")]
        public async Task<IActionResult> QuerySingleOrDefaultAsync()
        {
            Product produto;
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                // For test pourposes, it's taking top 10. On a reasonable scenario that'll be a top 1.
                produto = await connection.QuerySingleAsync<Product>(@"SELECT * FROM Products WHERE ProductID = @Id", new { id = 2 });
            }

            return Ok(produto);
        }

        [HttpGet, Route("QueryMultiple")]
        public IActionResult QueryMultiple(int id)
        {
            Product produto = null;
            using (var cn = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                var query = @"
SELECT * FROM Products WHERE ProductID = @Id
SELECT * FROM  [Order Details] WHERE ProductID = @Id
";
                using (var result = cn.QueryMultiple(query, new { id }))
                {
                    produto = result.ReadSingle<Product>();
                    produto.OrderDetails = result.Read<OrderDetail>().ToList();
                }
            }
            return Ok(produto);
        }

        [HttpGet, Route("QueryMultipleAsync")]
        public async Task<IActionResult> QueryMultipleAsync(int id)
        {
            Product produto = null;
            using (var cn = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                var query = @"
SELECT * FROM Products WHERE ProductID = @Id
SELECT * FROM  [Order Details] WHERE ProductID = @Id
";
                using (var result = await cn.QueryMultipleAsync(query, new { id }))
                {
                    produto = await result.ReadFirstAsync<Product>();
                    produto.OrderDetails = (await result.ReadAsync<OrderDetail>()).ToList();
                }
            }
            return Ok(produto);
        }
    }
}
