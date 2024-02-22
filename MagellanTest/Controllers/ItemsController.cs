using Microsoft.AspNetCore.Mvc;
using Npgsql;


namespace MagellanTest.Controllers
{
    [ApiController]

    [Route("[controller]")]
    public class ItemsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public ItemsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("Post")]
        public async Task<IActionResult> Post([FromBody] NewItem new_item)
        {
            string query = @"INSERT INTO item (item_name, parent_item, cost, req_date) VALUES 
            (@item_name, @parent_item, @cost, @req_date) RETURNING id";

            //Connect to Postgres Database
            await using var conn= new NpgsqlConnection(_configuration.GetConnectionString("PartConnection"));

            var cmd = new NpgsqlCommand(query, conn);

            await conn.OpenAsync();

            cmd.Parameters.AddWithValue("@item_name", new_item.ItemName);
            cmd.Parameters.AddWithValue("@parent_item", ((object)new_item.ParentItem) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@cost", new_item.Cost);
            cmd.Parameters.AddWithValue("@req_date", new_item.ReqDate);

            var new_item_id = cmd.ExecuteScalar();

            await conn.CloseAsync();
            
            return Ok(new_item_id);
        }

        [HttpGet("Get")]
        public async Task<IActionResult> Get(int id)
        {
            String query = @"
                SELECT id, item_name, parent_item, cost, req_date FROM item
                WHERE id = @id
            ";

            //Connect to Postgres Database
            await using var conn = new NpgsqlConnection(_configuration.GetConnectionString("PartConnection"));

            await conn.OpenAsync();

            await using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("id", id);
            await using var reader = await cmd.ExecuteReaderAsync();


            if (reader.Read())
            {
                var item = new ItemFound
                {
                    Id = reader.GetFieldValue<int>(0),
                    ItemName = reader.GetFieldValue<String>(1),
                    ParentItem = reader.IsDBNull(2) ? null :
                    reader.GetFieldValue<int>(2),
                    Cost = reader.GetFieldValue<int>(3),
                    ReqDate = reader.GetFieldValue<DateTime>(4)

                };
                await conn.CloseAsync();
                return Ok(item);
            }
            else
            {
                await conn.CloseAsync();
                return NotFound("ID does not exist");
            }

            

        }
        [HttpGet("GetTotalCost")]
        public async Task<IActionResult> GetTotalCost(string itemName)
        {
            String query = @"
                SELECT Get_Total_Cost(@item_name)
            ";

            //Connect to Postgres Database
            await using var conn = new NpgsqlConnection(_configuration.GetConnectionString("PartConnection"));

            await conn.OpenAsync();

            await using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@item_name", itemName);
            var totalCost = await cmd.ExecuteScalarAsync();

            if (totalCost == DBNull.Value)
            {
                await conn.CloseAsync();
                return NotFound("This is either a subitem or does not exist");
            }
            else
            {
                await conn.CloseAsync();
                return Ok(totalCost);
            }
            
        }


        public class NewItem
        {
            public string ItemName { get; set; }
            public int? ParentItem { get; set; }
            public int Cost { get; set; }
            public DateTime ReqDate { get; set; }
        }

        public class ItemFound
        {
            public int Id { get; set; }
            public string ItemName { get; set; }
            public int? ParentItem { get; set; }
            public int Cost { get; set; }
            public DateTime ReqDate { get; set; }

        }
    }


}
