
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

/******

//Run PostgreSqlScript to create Part Database
var connString = "Host=localhost;Username=postgres;Password=password1;Database=Part";

//var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"PostgreSqlScript\PartDB.sql");
string script = File.ReadAllText(@"PostgreSqlScript\PartDB.sql");

await using var conn = new NpgsqlConnection(connString);
var create_db_command = new NpgsqlCommand(script, conn);

await conn.OpenAsync();
await create_db_command.ExecuteNonQueryAsync();
await conn.CloseAsync();
*******/

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
