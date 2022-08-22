using MySqlConnector;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);

/*const string MySqlAwsConnString = "server=aws-whereyouat-mysql.ckmu2uzvci2p.us-east-2.rds.amazonaws.com;user=fimtown;password=R3dst4ff;database=whereyouat";*/

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddTransient<MySqlConnection>(_ => new MySqlConnection());
var app = builder.Build();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
// }

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();



app.Run();
