using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using DapperExamples.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DapperExamples.Controllers
{
    [Route("[controller]")]
    public class SimpleExamplesController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public SimpleExamplesController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpGet, Route("GetById")]
        public IActionResult GetById(int id)
        {
            IEnumerable<Product> produtos;
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                produtos = connection.Query<Product>(@"SELECT * FROM Products WHERE ProductID = @Id", new { id });
            }

            return Ok(produtos);
        }

        [HttpGet, Route("Buffered")]
        public IActionResult Buffered()
        {
            IEnumerable<Product> produtos;
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                produtos = connection.Query<Product>(@"SELECT * FROM Products", buffered: true);
            }

            return Ok(produtos);
        }
    }
}