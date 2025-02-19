using Keycloak.Authz.Net;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();
builder.Services.AddKeycloakAuthz(configure => builder.Configuration.GetSection("Keycloak").Bind(configure));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/universities", () =>
{
    return new[] { "University of Lagos", "University of Ibadan", "University of Nigeria" };
})
.RequireAuthz(["university#read"]);

app.UseKeycloakAuthz();
app.MapControllers();

app.Run();