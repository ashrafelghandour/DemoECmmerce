using OrderApi.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfastructureService(builder.Configuration);

builder.Services.AddControllers();

builder.Services.AddSwaggerGen();
// Add services to the container.



var app = builder.Build();

 app.UseInfastructurePolicy();


app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
