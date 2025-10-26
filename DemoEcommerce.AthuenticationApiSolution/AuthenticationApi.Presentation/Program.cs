using AuthenticationApi.Infrastructure.DependencyInjection;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddSwaggerGen();
builder.Services.AddInfraStructureService(builder.Configuration);


var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseInfrastructurePolicy();

app.MapOpenApi();

app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
