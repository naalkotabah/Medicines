using Medicines.Data;
using Medicines.Mapping;
using Medicines.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ✅ 1️⃣ إعداد الاتصال بقاعدة البيانات
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("myconnection")));

// ✅ 2️⃣ إضافة الخدمات الأساسية
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ✅ 3️⃣ تمكين CORS (السماح بطلبات محددة)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        policy =>
        {
            policy.WithOrigins("http://127.0.0.1:5500") // السماح بالنطاق المطلوب
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// ✅ 4️⃣ إعداد المصادقة باستخدام JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtConfig = builder.Configuration.GetSection("Jwt");
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtConfig["Issuer"],
            ValidAudience = jwtConfig["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig["Key"]))
        };
    });

// ✅ 5️⃣ إعداد Swagger لدعم المصادقة JWT
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "WebsiteAPI By Nael",
        Version = "v1"
    });
});

// ✅ 6️⃣ إعداد سياسات التصريح (Authorization)
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));
    options.AddPolicy("PractitionerOnly", policy => policy.RequireRole("Practitioner"));
    options.AddPolicy("AdminOrUser", policy => policy.RequireRole("Admin", "User"));
});

// ✅ 7️⃣ إضافة AutoMapper
builder.Services.AddAutoMapper(typeof(Mappings));

var app = builder.Build();

// ✅ 8️⃣ تشغيل Swagger فقط في بيئة التطوير
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ✅ 9️⃣ استخدام Middleware لمعالجة الأخطاء في جميع البيئات
app.UseMiddleware<ErrorHandlingMiddleware>();

// ✅ 1️⃣0️⃣ تمكين CORS (يجب أن يكون قبل المصادقة والتخويل)
app.UseCors("AllowSpecificOrigin");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
