using MyShopApi.Models;
using MyShopApi.Services;

public class ConsoleManager
{
    private readonly ApiService _api;

    public ConsoleManager(ApiService api)
    {
        _api = api;
    }

    public async Task Run()
    {
        while (true)
        {
            Console.WriteLine("\n1. Добавить товар");
            Console.WriteLine("2. Показать товары");
            Console.WriteLine("3. Обновить товар");
            Console.WriteLine("4. Удалить товар");
            Console.WriteLine("5. Выход");

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    await CreateProduct();
                    break;
                case "2":
                    await _api.GetAsync("/products");
                    break;
                case "3":
                    await UpdateProduct();
                    break;
                case "4":
                    await DeleteProduct();
                    break;
                case "5":
                    return;
            }
        }
    }

    private async Task CreateProduct()
    {
        Console.Write("Название: ");
        var name = Console.ReadLine();

        Console.Write("Цена: ");
        var price = decimal.Parse(Console.ReadLine()!);

        var product = new Product
        {
            Name = name!,
            Price = price,
            Stock = 10
        };

        await _api.PostAsync("/products", product);
    }

    private async Task UpdateProduct()
    {
        Console.Write("ID: ");
        Console.Write("ID: ");
        int id = int.Parse(Console.ReadLine()!);

        var product = new Product
        {
            Name = "Updated",
            Price = 999,
            Stock = 5
        };

        await _api.PutAsync($"/products/{id}", product);
    }

    private async Task DeleteProduct()
    {
        Console.Write("ID: ");
        int id = int.Parse(Console.ReadLine()!);

        await _api.DeleteAsync($"/products/{id}");
    }
}