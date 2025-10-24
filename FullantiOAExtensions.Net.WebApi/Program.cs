using Microsoft.OpenApi.Models;
using System.Reflection;
using Newtonsoft.Json.Serialization;
using FullantiOAExtensions.Core.Utils;
using FullantiOAExtensions.Core.Database;
using FullantiOAExtensions.Net.WebApi.Controllers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    //jsonС�շ�
    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
    //����ѭ������
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
    //��ʽ��ʱ��
    options.SerializerSettings.DateFormatString = "yyyy-MM-dd";
});

//swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "������OA��չ",
        Description = "OpenAPI"
    });
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename), true);
});
//cors
builder.Services.AddCors(options => options.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));
//·��Сд
builder.Services.AddRouting(options => options.LowercaseUrls = true);
//����ע��
builder.Services.AddHttpClient()
    .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
    .AddScoped<HttpWebClient>()
    .AddScoped<HttpUtil>()
    .AddScoped<HttpFile>()
    .AddScoped<ConfigHelper>()
    .AddScoped<OaExtendDbContext>()
    .AddScoped<HRController>();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors();
app.UseAuthorization();
app.MapControllers();
app.Run();
