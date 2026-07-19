
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.Text;
using Work.Configuration.Dependency;
using Work.Repository.IRepository;
using Work.Repository.Repository;
using Work.Service.IService;
using Work.Service.Services;
using WorkExperianceAPI.Models;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));

builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    // Options কন্টেইনার থেকে ডাটাবেজ সেটিংস রিড করা
    var settings = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<MongoDbSettings>>().Value;

    // মঙ্গোডিবি ক্লায়েন্ট এবং ডাটাবেজ ইনিশিয়ালাইজেশন
    var client = new MongoDB.Driver.MongoClient(settings.ConnectionString);
    return client.GetDatabase(settings.DatabaseName);
});


builder.Services.AddApplicationServices(builder.Configuration);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true, 
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"]!)),       
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5173") // আপনার ফ্রন্টএন্ড URL (React/Vue/Angular)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // কুকি পাস করার জন্য এটি মাস্ট!
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("CorsPolicy");

// অথেনটিকেশন এবং অথরাইজেশন মিডলওয়্যার (অর্ডার খুবই গুরুত্বপূর্ণ)
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
