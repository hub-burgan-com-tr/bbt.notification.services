using System.Reflection;
using bbt.framework.dengage.Business;
using Elastic.Apm;
using Elastic.Apm.NetCoreAll;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Notification.Profile.Business;
using Notification.Profile.Helper;

IConfigurationRoot configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{GetEnviroment()}.json", false, true)
    .AddEnvironmentVariables()
    .Build();

string? GetEnviroment()
{
    return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
}
var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddScoped<ILogHelper, LogHelper>();
builder.Services.AddControllers();
builder.Services.AddHealthChecks();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "bbt.service.notification-profile", Version = "v1" });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlTopic = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlTopic);

                c.EnableAnnotations(enableAnnotationsForInheritance: true, enableAnnotationsForPolymorphism: true);
                c.CustomSchemaIds(x => x.FullName);
            });
builder.Services.AddDbContext<DatabaseContext>();
builder.Services.AddSingleton(n => Agent.Tracer);
var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "bbt.service.notification-profile v1");
                //c.RoutePrefix = "";

            });
//}
app.UseRouting();
app.UseAuthorization();
app.UseAllElasticApm(configuration);
app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapHealthChecks("/health");
        });


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<DatabaseContext>();    
    context.Database.Migrate();
}


app.Run();
