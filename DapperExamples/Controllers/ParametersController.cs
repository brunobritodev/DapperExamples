using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using DapperExamples.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DapperExamples.Controllers
{
    [Route("[controller]")]
    public class ParametersController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public ParametersController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpGet, Route("Anonymous")]
        public IActionResult Anonymous()
        {
            IEnumerable<Product> produto;
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                produto = connection.Query<Product>(@"SELECT * FROM Products WHERE ProductID = @ID", new { id = 2 });
            }
            return Ok(produto);
        }

        [HttpGet, Route("AnonymousManyTimes")]
        public IActionResult AnonymousManyTimes()
        {
            var cleanQuery = @"DELETE FROM Shippers WHERE CompanyName IN @companies";
            var insertQuery = @"INSERT [Shippers] VALUES(@CompanyName, @Phone)";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                var totalRows = connection.Execute(insertQuery, new[] {
                    new { CompanyName = "Correios", Phone="+55 11 2254-6566"},
                    new { CompanyName = "Fedex", Phone="+1 5 222-654-988"},
                    new { CompanyName = "China Air Post", Phone="123456789"},
                });

                var totalDeletedRows = connection.Execute(cleanQuery, new { companies = new[] { "Correios", "Fedex", "China Air Post" } });

                return Ok(new { Inserted = totalRows, Removed = totalDeletedRows });
            }
        }

        [HttpGet, Route("Dynamic")]
        public IActionResult Dynamic(string categoryName = "Produce", int orderYear = 1998)
        {
            var sql = "SalesByCategory";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                var parameter = new DynamicParameters();

                parameter.Add("@CategoryName", categoryName, DbType.String, ParameterDirection.Input);
                parameter.Add("@OrdYear", orderYear.ToString(), DbType.String, ParameterDirection.Input);
                parameter.Add("@RowCount", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

                connection.Execute(sql, parameter, commandType: CommandType.StoredProcedure);

                return Ok(parameter.Get<int>("@RowCount"));
            }
        }

        [HttpGet, Route("List")]
        public IActionResult List()
        {
            IEnumerable<Product> produto;
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                produto = connection.Query<Product>(@"SELECT * FROM Products WHERE ProductID IN @ID", new { id = new[] { 2, 3, 10, 4, 3, 20 } });
            }
            return Ok(produto);

        }

        [HttpGet, Route("Like")]
        public IActionResult Like()
        {
            IEnumerable<Product> produto;
            var name = "chef";
            var likeTrick = $"%{name}%";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                produto = connection.Query<Product>(@"SELECT * FROM Products WHERE ProductName LIKE @productName", new { productName = likeTrick });
            }
            return Ok(produto);

        }

    }
}