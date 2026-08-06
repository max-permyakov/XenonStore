// Не используется


using Bogus;
using Xenon.Domain.Models;

namespace Xenon.Infrastructure.Services
{
    public static class DataGenerator
    {
      
        public static List<Product> GenerateProducts(int count = 100)
        {

            var productFaker = new Faker<Product>()
                .RuleFor(p => p.Name, f => f.Commerce.ProductName())
                .RuleFor(p => p.Description, f => f.Commerce.ProductDescription())
                .RuleFor(p => p.Price, f => decimal.Parse(f.Commerce.Price(min: 1, max: 1000)));
                //.RuleFor(p => p.Category,  f => f.PickRandom(CategoryData.AllCategories));

            return productFaker.Generate(count);
        }
        public static List<Order> GenerateOrders(IEnumerable<Product> products, int orderCount = 20)
        {
            var productList = products.ToList();
            var orderFaker = new Faker<Order>()
                .RuleFor(o => o.Name, f => f.Person.FullName)
                .RuleFor(o => o.Line1, f => f.Address.StreetAddress())
                .RuleFor(o => o.Line2, f => f.Address.SecondaryAddress().OrNull(f))
                .RuleFor(o => o.City, f => f.Address.City())
                .RuleFor(o => o.State, f => f.Address.State())
                .RuleFor(o => o.Zip, f => f.Address.ZipCode())
                .RuleFor(o => o.Country, f => f.Address.Country())
                .RuleFor(o => o.GiftWrap, f => f.Random.Bool(0.3f))
                .RuleFor(o => o.Shipped, f => f.Random.Bool(0.7f))
                .RuleFor(o => o.Lines, (f, o) =>
                {
                
                    int linesCount = f.Random.Int(1, 5);
                    var lines = new List<CartLine>();
                    for (int i = 0; i < linesCount; i++)
                    {
                        var product = f.PickRandom(productList);
                        var quantity = f.Random.Int(1, 10);
                        lines.Add(new CartLine { Product = product, Quantity = quantity });
                    }
                    return lines;
                });

            return orderFaker.Generate(orderCount);
        }
        public static class CategoryData
        {
            public static readonly string[] AllCategories = new[]
            {
        // Электроника
        "Смартфоны", "Ноутбуки", "Планшеты", "Наушники", "Колонки", "Телевизоры",
        "Фотоаппараты", "Видеокамеры", "Умные часы", "Фитнес-браслеты", "Игровые приставки",
        "Компьютерные комплектующие", "Мониторы", "Клавиатуры", "Мыши", "Принтеры",
        // Одежда
        "Мужская одежда", "Женская одежда", "Детская одежда", "Обувь", "Аксессуары",
        "Спортивная одежда", "Пляжная одежда", "Верхняя одежда", "Джинсы", "Футболки",
        "Рубашки", "Платья", "Костюмы", "Головные уборы", "Шарфы и перчатки",
        // Дом и сад
        "Мебель", "Кухонная утварь", "Посуда", "Декор", "Садовая мебель", "Грили",
        "Освещение", "Ковры", "Шторы", "Постельное бельё", "Полотенца",
        // Детские товары
        "Игрушки", "Детское питание", "Коляски", "Автокресла", "Детская мебель",
        // Книги
        "Художественная литература", "Научная литература", "Учебники", "Детские книги",
        "Компьютерная литература", "Бизнес-литература",
        // Спорт и отдых
        "Спортивные тренажёры", "Велосипеды", "Беговые дорожки", "Мячи", "Туризм",
        "Рыболовные снасти", "Охота",
        // Автотовары
        "Автомобильные шины", "Масла и жидкости", "Автозапчасти", "Автоэлектроника",
        // Красота и здоровье
        "Косметика", "Парфюмерия", "Уход за волосами", "Уход за кожей", "Зубные пасты",
        "Витамины", "БАДы",
        // Продукты питания
        "Бакалея", "Напитки", "Молочные продукты", "Мясо", "Рыба", "Фрукты и овощи",
        "Кондитерские изделия", "Замороженные продукты",
        // Бытовая химия
        "Стиральные порошки", "Чистящие средства", "Средства для мытья посуды",
        // Ювелирные изделия
        "Серьги", "Кольца", "Цепи", "Браслеты",
        // Товары для животных
        "Корм для собак", "Корм для кошек", "Аксессуары для животных",
        // Канцтовары
        "Бумага", "Ручки", "Карандаши", "Папки", "Ежедневники"
        
    };
        }
    }
}