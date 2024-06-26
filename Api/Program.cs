using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString=builder.Configuration.GetSection("connectionString").Value;
builder.Services.AddDbContext<ToDoDbContext>(options =>options.UseSqlServer(connectionString));
builder.Services.AddScoped<IAddRecord,AddRecordService>();
builder.Services.AddScoped<IDeleteRecord,DeleteRecordService>();
builder.Services.AddScoped<IEditRecord,EditRecordService>();
builder.Services.AddScoped<IShowAllRecords,ShowAllRecordsService>();
builder.Services.AddCors();



var app = builder.Build();
app.UseCors(builder => builder.AllowAnyOrigin());

await using (var scope = app.Services.CreateAsyncScope()) {
var context = scope.ServiceProvider.GetRequiredService<ToDoDbContext>();
context.Database.EnsureCreated();
}

app.MapGet("/showAllRecords", ([FromServices]IShowAllRecords showAllRecords) =>Results.Ok(showAllRecords.ShowAllRecords()));

app.MapPost("/deleteRecord/{id}", 
(int id,[FromServices]IDeleteRecord deleteRecord) =>
{
    int status=deleteRecord.DeleteRecord(id);
    return status==0?Results.Ok():Results.Problem(statusCode:400);
});


app.MapPost("/addRecord/{title}/{content}", 
(string title,string content,[FromServices]IAddRecord addRecord) => 
{
    int status=addRecord.AddRecord(title,content);
    return status==0?Results.Ok():Results.Problem(statusCode:400);
});

app.MapPut("/editRecord/{id}/{title}/{content}", 
(int id,string title,string content, [FromServices]IEditRecord editRecord) => 
{
    int status=editRecord.EditRecord(id, title, content);
    return status==0?Results.Ok():Results.Problem(statusCode:400);
});

app.Run();