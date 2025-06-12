using Microsoft.OpenApi.Models;
using UserCrudService.Repositories;
using UserCrudService.Services;

var builder = WebApplication.CreateBuilder(args);

// Репозиторий и сервисы
builder.Services.AddSingleton<IUserRepository, InMemoryUserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Users API", Version = "v1" });

    // Описываем наш заголовок-ключ
    c.AddSecurityDefinition("X-Login", new OpenApiSecurityScheme
    {
        Name = "X-Login", // имя в HTTP-заголовке
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header,
        Description = "Login пользователя, от имени которого выполняется запрос"
    });

    // Говорим, что все операции могут (или должны) использовать этот ключ
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "X-Login"
                }
            },
            []
        }
    });
});

var app = builder.Build();

// Swagger
app.UseSwagger();
app.UseSwaggerUI();

// Маршрутизация
app.MapControllers();

// Создаём пользователя-админа при запуске
using (var scope = app.Services.CreateScope())
{
    var svc = scope.ServiceProvider.GetRequiredService<IUserService>();
    svc.EnsureAdmin();
}

app.Run();