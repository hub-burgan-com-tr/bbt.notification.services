using System.Reflection;
using Microsoft.OpenApi.Models;
using bbt.service.notification_profile.Business;


var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

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
builder.Services.AddScoped<IinstandReminder, BInstantReminder>();


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
app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapHealthChecks("/health");
        });




app.Run();
