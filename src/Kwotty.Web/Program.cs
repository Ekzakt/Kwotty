using Kwotty.Application.Configuration;
using Kwotty.Data;
using Kwotty.Data.Configuration;
using Kwotty.Infrastructure.Configuration;
using Kwotty.Web.EndPoints;
using Kwotty.Web.EndPoints.Authentication;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddRazorPages();
builder.Services.AddSwaggerGen();
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices();
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddHttpContextAccessor();
builder.Services.AddRouting(options => options.LowercaseQueryStrings = true);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler(Constants.ErrorPagePath);
    app.UseHsts();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapAuthenticationEndpoints();
app.MapQuoteEndpoints();

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
