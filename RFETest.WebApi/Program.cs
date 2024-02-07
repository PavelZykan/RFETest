using AutoMapper;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using RFETest.Core.Diff;
using RFETest.Core.Values;
using RFETest.WebApi.Formatters;
using RFETest.WebApi.Middleware;
using System.Reflection;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services
    .AddControllers()
    .AddMvcOptions(opt =>
    {
        // remove all formatter except the json formatter
        var toRemove = opt.InputFormatters.Where(x => x is not SystemTextJsonInputFormatter).ToList();
        foreach (var remove in toRemove)
        {
            opt.InputFormatters.Remove(remove);
        }
        // add custom base64 formatter
        opt.InputFormatters.Add(new Base64InputFormatter());
    })
    .AddJsonOptions(opt => opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(setup =>
{
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    setup.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});
builder.Services.AddSingleton<IDiffValueStorage, InMemoryDiffValueStorage>();
builder.Services.AddSingleton<IDiffProvider, DiffProvider>();

var mapperConfiguration = new MapperConfiguration(cfg => cfg.AddMaps(typeof(Program).Assembly));
mapperConfiguration.CompileMappings();
builder.Services.AddSingleton(mapperConfiguration.CreateMapper());

var app = builder.Build();

app.UseMiddleware<ErrorHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
