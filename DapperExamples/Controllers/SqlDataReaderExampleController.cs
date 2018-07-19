using DapperExamples.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace DapperExamples.Controllers
{
    [Route("[controller]")]
    public class SqlDataReaderExampleController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public SqlDataReaderExampleController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpGet, Route("GetById")]
        public IActionResult GetById(int id)
        {
            var produtos = new List<Product>();
            var sql = @"SELECT * FROM Products WHERE ProductID = @Id";
            using (SqlConnection connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.CommandTimeout = 7200;
                    command.Parameters.Add(new SqlParameter("@Id", id));

                    connection.Open();
                    var dr = command.ExecuteReader();
                    while (dr.Read())
                    {
                        produtos.Add(new Product()
                        {
                            ProductId = dr.GetInt32(dr.GetOrdinal("ProductId")),
                            CategoryId = dr.IsDBNull(dr.GetOrdinal("CategoryId")) ? -1 : dr.GetInt32(dr.GetOrdinal("CategoryId")),
                            Discontinued = dr.GetBoolean(dr.GetOrdinal("Discontinued")),
                            ProductName = dr.IsDBNull(dr.GetOrdinal("ProductName")) ? string.Empty : dr.GetString(dr.GetOrdinal("ProductName")),
                            QuantityPerUnit = dr.IsDBNull(dr.GetOrdinal("QuantityPerUnit")) ? string.Empty : dr.GetString(dr.GetOrdinal("QuantityPerUnit")),
                            ReorderLevel = dr.IsDBNull(dr.GetOrdinal("ReorderLevel")) ? (short)-1 : dr.GetInt16(dr.GetOrdinal("ReorderLevel")),
                            SupplierId = dr.IsDBNull(dr.GetOrdinal("SupplierId")) ? -1 : dr.GetInt32(dr.GetOrdinal("SupplierId")),
                            UnitPrice = dr.IsDBNull(dr.GetOrdinal("UnitPrice")) ? -1 : dr.GetDecimal(dr.GetOrdinal("UnitPrice")),
                            UnitsInStock = dr.IsDBNull(dr.GetOrdinal("UnitsInStock")) ? (short)-1 : dr.GetInt16(dr.GetOrdinal("UnitsInStock")),
                            UnitsOnOrder = dr.IsDBNull(dr.GetOrdinal("UnitsOnOrder")) ? (short)-1 : dr.GetInt16(dr.GetOrdinal("UnitsOnOrder")),

                        });
                    }
                }
            }

            return Ok(produtos);
        }
    }
}
