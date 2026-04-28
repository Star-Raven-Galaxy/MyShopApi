// Program.cs

/*
var builder = WebApplication.CreateBuilder(args);

// ----------------- База данных -----------------
var connStr = Environment.GetEnvironmentVariable("POSTGRES_CONNECTION")
    ?? "Host=127.0.0.1;Port=5432;Database=myshop;Username=postgres;Password=postgres";
builder.Services.AddDbContext<ShopDbContext>(o => o.UseNpgsql(connStr));

// ----------------- Redis -----------------
var redisConn = Environment.GetEnvironmentVariable("REDIS_CONNECTION") ?? "localhost:6379,abortConnect=false";
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConn));

// ----------------- Swagger -----------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ----------------- ApiService -----------------
builder.Services.AddHttpClient<ApiService>();

var app = builder.Build();
app.UseDefaultFiles(); // ищет index.html в wwwroot
app.UseStaticFiles();  // отдаёт статику

app.MapGet("/", () => Results.Redirect("/index.html"));
app.UseHttpMetrics();
if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }
app.UseHttpsRedirection();

// ----------------- Redis кэш -----------------
var cache = app.Services.GetRequiredService<IConnectionMultiplexer>().GetDatabase();

var ProductsCreated =
Metrics.CreateCounter(
    "products_created_total",
    "Количество созданных товаров"
);



*/
/*
using Microsoft.AspNetCore.Mvc;
using MyShopApi.Models;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers()
    .AddJsonOptions(opts => opts.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Простые in-memory хранилища
var products = new List<Product>();
var customers = new List<Customer>();
var orders = new List<Order>();
var deliveries = new List<Delivery>();
int nextProductId = 1, nextCustomerId = 1, nextOrderId = 1, nextDeliveryId = 1;

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/api/products", () => Results.Ok(products));
app.MapGet("/api/products/{id}", (int id) =>
{
    var product = products.FirstOrDefault(p => p.Id == id);
    return product is null ? Results.NotFound() : Results.Ok(product);
});
app.MapPost("/api/products", (Product product) =>
{
    product.Id = nextProductId++;
    products.Add(product);
    return Results.Created($"/api/products/{product.Id}", product);
});
app.MapPut("/api/products/{id}", (int id, Product updated) =>
{
    var product = products.FirstOrDefault(p => p.Id == id);
    if (product is null) return Results.NotFound();
    product.Name = updated.Name;
    product.Price = updated.Price;
    product.Stock = updated.Stock;
    return Results.Ok(product);
});
app.MapDelete("/api/products/{id}", (int id) =>
{
    var product = products.FirstOrDefault(p => p.Id == id);
    if (product is null) return Results.NotFound();
    products.Remove(product);
    return Results.NoContent();
});

// Аналогично для Customers (минимум)
app.MapGet("/api/customers", () => Results.Ok(customers));
app.MapPost("/api/customers", (Customer customer) =>
{
    customer.Id = nextCustomerId++;
    customers.Add(customer);
    return Results.Created($"/api/customers/{customer.Id}", customer);
});
// Добавьте PUT, DELETE, GET по id по аналогии (или просто для примера)

app.MapGet("/", () => Results.Redirect("/swagger"));
app.Run();
*/
using Microsoft.AspNetCore.Mvc;
using MyShopApi.Models;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers()
    .AddJsonOptions(opts => opts.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Простые in-memory хранилища
var products = new List<Product>();
var customers = new List<Customer>();
var orders = new List<Order>();
var deliveries = new List<Delivery>();
int nextProductId = 1, nextCustomerId = 1, nextOrderId = 1, nextDeliveryId = 1;

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/api/products", () => Results.Ok(products));
app.MapGet("/api/products/{id}", (int id) =>
{
    var product = products.FirstOrDefault(p => p.Id == id);
    return product is null ? Results.NotFound() : Results.Ok(product);
});
app.MapPost("/api/products", (Product product) =>
{
    product.Id = nextProductId++;
    products.Add(product);
    return Results.Created($"/api/products/{product.Id}", product);
});
app.MapPut("/api/products/{id}", (int id, Product updated) =>
{
    var product = products.FirstOrDefault(p => p.Id == id);
    if (product is null) return Results.NotFound();
    product.Name = updated.Name;
    product.Price = updated.Price;
    product.Stock = updated.Stock;
    return Results.Ok(product);
});
app.MapDelete("/api/products/{id}", (int id) =>
{
    var product = products.FirstOrDefault(p => p.Id == id);
    if (product is null) return Results.NotFound();
    products.Remove(product);
    return Results.NoContent();
});

// Аналогично для Customers (минимум)
app.MapGet("/api/customers", () => Results.Ok(customers));
app.MapPost("/api/customers", (Customer customer) =>
{
    customer.Id = nextCustomerId++;
    customers.Add(customer);
    return Results.Created($"/api/customers/{customer.Id}", customer);
});
// Добавьте PUT, DELETE, GET по id по аналогии (или просто для примера)

app.MapGet("/", () => Results.Redirect("/swagger"));
app.Run();

/*
using Microsoft.EntityFrameworkCore;
using MyShopApi.Data;
using MyShopApi.Services;
using Prometheus;
using StackExchange.Redis;
using System.Net.Sockets;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connStr = Environment.GetEnvironmentVariable("POSTGRES_CONNECTION")
              ?? builder.Configuration.GetConnectionString("Postgres")
              ?? "Host=localhost;Port=5432;Database=myshop;Username=postgres;Password=postgres";
builder.Services.AddDbContext<ShopDbContext>(options => options.UseNpgsql(connStr));

var redisConn = Environment.GetEnvironmentVariable("REDIS_CONNECTION")
                ?? builder.Configuration.GetConnectionString("Redis")
                ?? "localhost:6379,abortConnect=false";
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConn));
builder.Services.AddScoped<ICacheService, RedisCacheService>();

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IDeliveryService, DeliveryService>();
builder.Services.AddHealthChecks();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ShopDbContext>();
    await dbContext.Database.MigrateAsync();
    await SeedData.InitializeAsync(app.Services);
}

// Статические файлы (нужны для CSS/JS Swagger UI)
app.UseStaticFiles();

// Swagger с явным указанием конечной точки
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MyShop API V1"));

app.UseHttpsRedirection();
app.UseRouting();
app.UseHttpMetrics();
app.MapControllers();
app.MapMetrics();

app.MapGet("/", () => Results.Redirect("/swagger"));

app.Run();

*/











/*

using Microsoft.EntityFrameworkCore;
using MyShopApi.Data;
using MyShopApi.Models;
using MyShopApi.Services;
using MyShopApi.Delegates;
using StackExchange.Redis;
using Redis = StackExchange.Redis;
using MyModels = MyShopApi.Models;
using Prometheus;
// Program.cs



var builder = WebApplication.CreateBuilder(args);

// База данных 
var connStr = Environment.GetEnvironmentVariable("POSTGRES_CONNECTION")
    ?? "Host=127.0.0.1;Port=5432;Database=myshop;Username=postgres;Password=postgres";
builder.Services.AddDbContext<ShopDbContext>(o => o.UseNpgsql(connStr));

// ----------------- Redis
var redisConn = Environment.GetEnvironmentVariable("REDIS_CONNECTION") ?? "localhost:6379,abortConnect=false";
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConn));

//  Swagger 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//  ApiService
builder.Services.AddHttpClient<ApiService>();

var app = builder.Build();
app.UseDefaultFiles(); // ищет index.html в wwwroot
app.UseStaticFiles();  // отдаёт статику

app.MapGet("/", () => Results.Redirect("/index.html"));
app.UseHttpMetrics();
if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }
app.UseHttpsRedirection();

// Redis кэш 
var cache = app.Services.GetRequiredService<IConnectionMultiplexer>().GetDatabase();

var ProductsCreated =
Metrics.CreateCounter(
    "products_created_total",
    "Количество созданных товаров"
);



app.MapGet("/api/products", async (ShopDbContext db) =>
{
    return await db.Products.ToListAsync();
});





// Получение по ID
app.MapGet("/products/{id}", async (int id, ShopDbContext db) =>
{
    var product = await db.Products.FindAsync(id);
    return product is null ? Results.NotFound() : Results.Ok(product);
});

// Обновление
app.MapPut("/products/{id}", async (int id, Product updated, ShopDbContext db) =>
{
    var product = await db.Products.FindAsync(id);
    if (product == null) return Results.NotFound();

    product.Name = updated.Name;
    product.Price = updated.Price;
    product.Stock = updated.Stock;

    await db.SaveChangesAsync();
    return Results.Ok(product);
});

// Удаление
app.MapDelete("/products/{id}", async (int id, ShopDbContext db) =>
{
    var product = await db.Products.FindAsync(id);
    if (product == null) return Results.NotFound();

    db.Products.Remove(product);
    await db.SaveChangesAsync();

    return Results.Ok();
});





Action<string> logAction = msg => Console.WriteLine($"LOG: {msg}");

Func<Product, bool> isExpensive = p => p.Price > 100;

logAction("Запуск приложения");

var testProduct = new Product { Name = "Phone", Price = 200 };

Console.WriteLine($"Дорогой товар? {isExpensive(testProduct)}");









app.MapGet("/shop", async (ShopDbContext db) =>
{
    var products = await db.Products.ToListAsync();

    var html = "<h1>Магазин</h1><ul>";

    foreach (var p in products)
    {
        html += $"<li>{p.Name} - {p.Price} $" +
                $"<a href='/buy/{p.Id}'>Купить</a></li>";
    }

    html += "</ul>";
    return Results.Content(html, "text/html");
});

// Покупка
app.MapGet("/buy/{id}", async (int id, ShopDbContext db) =>
{
    var product = await db.Products.FindAsync(id);
    if (product == null) return Results.NotFound();

    if (product.Stock <= 0)
        return Results.Content("Нет в наличии");

    product.Stock -= 1;
    await db.SaveChangesAsync();

    return Results.Content($"Вы купили: {product.Name}");
});








// CRUD для Product 
app.MapGet("/products", async (ShopDbContext db) =>
{
    string key = "products_all";
    var cached = await cache.StringGetAsync(key);
    if (cached.HasValue)
        return Results.Ok(System.Text.Json.JsonSerializer.Deserialize<List<Product>>(cached)!);

    var list = await db.Products.ToListAsync();
    await cache.StringSetAsync(key, System.Text.Json.JsonSerializer.Serialize(list), TimeSpan.FromMinutes(5));
    return Results.Ok(list);
});

app.MapPost("/products", async (Product product, ShopDbContext db) =>
{
    db.Products.Add(product);
    await db.SaveChangesAsync();
    await cache.KeyDeleteAsync("products_all");
    return Results.Created($"/products/{product.Id}", product);
});

//Customers, Orders, Delivery 
app.MapPost("/customers", async (Customer customer, ShopDbContext db) =>
{
    db.Customers.Add(customer);
    await db.SaveChangesAsync();
    return Results.Created($"/customers/{customer.Id}", customer);
});

app.MapPost("/orders", async (MyModels.Order order, ShopDbContext db) =>
{
    db.Orders.Add(order);
    await db.SaveChangesAsync();
    return Results.Created($"/orders/{order.Id}", order);
});

app.MapPost("/deliveries", async (Delivery delivery, ShopDbContext db) =>
{
    db.Deliveries.Add(delivery);
    await db.SaveChangesAsync();
    return Results.Created($"/deliveries/{delivery.Id}", delivery);
});

// ----------------- Демонстрация ApiService с делегатами -----------------
var httpClient = new HttpClient();
var apiService = new ApiService(httpClient);

OnRequestCompleted handler = RequestHandlers.LogToFile;
handler += RequestHandlers.LogToConsole;
apiService.RequestCompleted += handler;

_ = RunApiTests(apiService, handler);

async Task RunApiTests(ApiService service, OnRequestCompleted handler)
{
    string[] endpoints = new[]
    {
        "https://jsonplaceholder.typicode.com/posts/1",
        "https://jsonplaceholder.typicode.com/posts/2",
        "https://jsonplaceholder.typicode.com/posts/3",
        "https://jsonplaceholder.typicode.com/posts/4",
        "https://jsonplaceholder.typicode.com/posts/5"
    };

    for (int i = 0; i < 3; i++) await service.GetAsync(endpoints[i]);

    handler -= RequestHandlers.LogToFile;
    service.RequestCompleted -= RequestHandlers.LogToFile;
    service.RequestCompleted += RequestHandlers.LogToConsole;

    for (int i = 3; i < 5; i++) await service.GetAsync(endpoints[i]);
}

_ = RunTests(apiService, handler);

var consoleManager = new ConsoleManager(apiService);
_ = consoleManager.Run();



async Task RunTests(ApiService apiService, OnRequestCompleted handler)
{
    string baseUrl = "https://localhost:5001";

    var product = new Product
    {
        Name = "TestProduct",
        Price = 100
    };

    // 1 создание
    await apiService.PostAsync($"{baseUrl}/products", product);

    // 2 список
    await apiService.GetAsync($"{baseUrl}/products");

    // 3 получение по ID
    await apiService.GetAsync($"{baseUrl}/products/1");

    // отключаем логирование в файл
    handler -= RequestHandlers.LogToFile;

    apiService.RequestCompleted -= RequestHandlers.LogToFile;
    apiService.RequestCompleted += RequestHandlers.LogToConsole;

    // 4 обновление
    product.Price = 200;
    await apiService.PutAsync($"{baseUrl}/products/1", product);

    // 5 удаление
    await apiService.DeleteAsync($"{baseUrl}/products/1");
}


app.UseHttpMetrics();
app.MapGet("/status", () => "API работает");

app.MapMetrics();


handler += RequestHandlers.LogToConsole;

apiService.RequestCompleted += handler;
app.MapMetrics();
app.Run();*/
/*
using Microsoft.EntityFrameworkCore;
using MyShopApi.Data;
using MyShopApi.Models;
using MyShopApi.Services;
using MyShopApi.Delegates;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// -------------------- Настройка сервисов --------------------

// PostgreSQL
var connStr = Environment.GetEnvironmentVariable("POSTGRES_CONNECTION")
    ?? "Host=127.0.0.1;Port=5432;Database=myshop;Username=postgres;Password=postgres";
builder.Services.AddDbContext<ShopDbContext>(o => o.UseNpgsql(connStr));

// Redis
var redisConn = "localhost:6379,abortConnect=false";
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConn));

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// HttpClient и ApiService
builder.Services.AddHttpClient<ApiService>();

var app = builder.Build();

// -------------------- Middleware --------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();

// -------------------- Redis --------------------
var cache = app.Services.GetRequiredService<IConnectionMultiplexer>().GetDatabase();

// -------------------- Product CRUD --------------------

// GET /products с кэшем Redis
app.MapGet("/products", async (ShopDbContext db) =>
{
    string key = "products_all";
    var cached = await cache.StringGetAsync(key);
    if (cached.HasValue)
        return Results.Ok(System.Text.Json.JsonSerializer.Deserialize<List<Product>>(cached)!);

    var list = await db.Products.ToListAsync();
    await cache.StringSetAsync(key, System.Text.Json.JsonSerializer.Serialize(list), TimeSpan.FromMinutes(5));
    return Results.Ok(list);
});

// GET /products/{id}
app.MapGet("/products/{id:int}", async (int id, ShopDbContext db) =>
{
    string key = $"product_{id}";
    var cached = await cache.StringGetAsync(key);
    if (cached.HasValue)
        return Results.Ok(System.Text.Json.JsonSerializer.Deserialize<Product>(cached)!);

    var product = await db.Products.FindAsync(id);
    if (product == null) return Results.NotFound();

    await cache.StringSetAsync(key, System.Text.Json.JsonSerializer.Serialize(product), TimeSpan.FromMinutes(5));
    return Results.Ok(product);
});

// POST /products
app.MapPost("/products", async (Product product, ShopDbContext db) =>
{
    db.Products.Add(product);
    await db.SaveChangesAsync();
    await cache.KeyDeleteAsync("products_all"); // сбросить кэш
    return Results.Created($"/products/{product.Id}", product);
});

// PUT /products/{id}
app.MapPut("/products/{id:int}", async (int id, Product updatedProduct, ShopDbContext db) =>
{
    var product = await db.Products.FindAsync(id);
    if (product == null) return Results.NotFound();

    product.Name = updatedProduct.Name;
    product.Price = updatedProduct.Price;
    await db.SaveChangesAsync();

    await cache.KeyDeleteAsync("products_all");
    await cache.KeyDeleteAsync($"product_{id}");
    return Results.Ok(product);
});

// DELETE /products/{id}
app.MapDelete("/products/{id:int}", async (int id, ShopDbContext db) =>
{
    var product = await db.Products.FindAsync(id);
    if (product == null) return Results.NotFound();

    db.Products.Remove(product);
    await db.SaveChangesAsync();

    await cache.KeyDeleteAsync("products_all");
    await cache.KeyDeleteAsync($"product_{id}");
    return Results.NoContent();
});

// -------------------- Демонстрация ApiService с делегатами --------------------
var httpClient = new HttpClient();
var apiService = new ApiService(httpClient);

// Создаём делегат и подписываем два метода
OnRequestCompleted handler = RequestHandlers.LogToFile;
handler += RequestHandlers.LogToConsole;
apiService.RequestCompleted += handler;

// Асинхронная задача тестирования API
_ = RunApiTests(apiService, handler);

// -------------------- Метод RunApiTests --------------------
async Task RunApiTests(ApiService service, OnRequestCompleted handler)
{
    string[] testEndpoints = new[]
    {
        "https://jsonplaceholder.typicode.com/posts/1",
        "https://jsonplaceholder.typicode.com/posts/2",
        "https://jsonplaceholder.typicode.com/posts/3",
        "https://jsonplaceholder.typicode.com/posts/4",
        "https://jsonplaceholder.typicode.com/posts/5"
    };

    // Первые три запроса с логированием в файл + консоль
    for (int i = 0; i < 3; i++)
        await service.GetAsync(testEndpoints[i]);

    // Отписка логирования в файл
    handler -= RequestHandlers.LogToFile;
    service.RequestCompleted -= null;   // снимаем старую подписку
    service.RequestCompleted += handler; // подписываем обновлённый делегат

    // Последние два запроса только в консоль
    for (int i = 3; i < 5; i++)
        await service.GetAsync(testEndpoints[i]);
}

// -------------------- Запуск приложения --------------------
app.Run();
*/








/*var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
var connectionString = Environment.GetEnvironmentVariable("POSTGRES_CONNECTION") 
                       ?? "Host=127.0.0.1;Port=5432;Database=myshop;Username=postgres;Password=ТВОЙ_ПАРОЛЬ";
app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
*/