using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// MySql
using System.Data.Common;
using MySql.Data.MySqlClient;

using System.Configuration; // для взятия строки подключения из App.config

// Telegram API
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.Enums; // для получения типа сообщения
using Telegram.Bot.Types.ReplyMarkups; // для создания клавиатуры
using System.Text.RegularExpressions;
using Telegram;
using System.IO;
using System.Net;

namespace TelegramShop
{
    class Program
    {
        static TelegramBotClient Bot;

        static string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

        static public List<string> GetAllCategories()
        {
            List<string> categories = new List<string>();

            try
            {
                using (MySqlConnection sqlConnection = new MySqlConnection())
                {
                    sqlConnection.ConnectionString = connectionString;

                    sqlConnection.Open();

                    MySqlCommand sqlCommand = sqlConnection.CreateCommand();
                    sqlCommand.CommandText = "SELECT name FROM category;";
                    sqlCommand.Connection = sqlConnection;

                    // Подсчитать кол-во строк в таблице
                    //object result = (new MySqlCommand("SELECT COUNT(*) FROM category", sqlConnection).ExecuteScalar());
                    //Console.WriteLine(((Int64)result));


                    using (DbDataReader reader = sqlCommand.ExecuteReader())
                    {
                        // вывести названия полей (столбцов) таблицы
                        //for (int i = 0; i < reader.FieldCount; i++)
                        //{
                        //    //Console.WriteLine((reader).GetName(i));
                        //}
                        //Console.WriteLine();

                        //Console.WriteLine("Field Count = " + reader.FieldCount);

                        //Console.WriteLine($"Id:\tName:\tSortOrder:\tStatus:");

                        while (reader.Read())
                        {
                            //Console.WriteLine($"{reader[0]}\t{reader["name"]}\t{reader["sort_order"]}\t{reader["status"]}");
                            categories.Add(reader["name"].ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.Message);
                categories.Add(ex.Message);
            }

            return categories;
        }

        static public int GetCategoryIdByName(string categoryName)
        {
            try
            {
                using (MySqlConnection sqlConnection = new MySqlConnection())
                {
                    sqlConnection.ConnectionString = connectionString;

                    sqlConnection.Open();

                    MySqlCommand sqlCommand = sqlConnection.CreateCommand();
                    sqlCommand.CommandText = $"SELECT id FROM category WHERE name=\"{categoryName}\" LIMIT 1;"; // ???
                    sqlCommand.Connection = sqlConnection;

                    using (DbDataReader reader = sqlCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            return Int32.Parse(reader["id"].ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return -1; // ошибка -- нет такой категории
        }

        static public List<Product> GetAllProductsByCategoryId(int categoryId)
        {
            List<Product> products = new List<Product>();

            try
            {
                using (MySqlConnection sqlConnection = new MySqlConnection())
                {
                    sqlConnection.ConnectionString = connectionString;

                    sqlConnection.Open();

                    MySqlCommand sqlCommand = sqlConnection.CreateCommand();
                    // Избавиться от возможного SQL_Injection
                    sqlCommand.CommandText = $"SELECT * FROM product WHERE category_id={categoryId};";
                    sqlCommand.Connection = sqlConnection;

                    using (DbDataReader reader = sqlCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            products.Add(new Product(Int32.Parse(reader["id"].ToString()), reader["name"].ToString(), Int32.Parse(reader["category_id"].ToString()), Int32.Parse(reader["code"].ToString()), double.Parse(reader["price"].ToString()), Int32.Parse(reader["availability"].ToString()), reader["brand"].ToString(), reader["description"].ToString(), Int32.Parse(reader["is_new"].ToString()), Int32.Parse(reader["is_recommended"].ToString()), Int32.Parse(reader["status"].ToString())));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return products;
        }

        static public List<Product> GetAllProductsByCategoryName(string categoryName)
        {
            int categoryId = GetCategoryIdByName(categoryName);

            if (categoryId <= 0) return null;

            return GetAllProductsByCategoryId(categoryId);
        }

        static public List<Product> GetAllProductsBetweenPrices(double minPrice = 0, double maxPrice = 9999999999999)
        {
            List<Product> products = new List<Product>();

            try
            {
                using (MySqlConnection sqlConnection = new MySqlConnection())
                {
                    sqlConnection.ConnectionString = connectionString;

                    sqlConnection.Open();

                    MySqlCommand sqlCommand = sqlConnection.CreateCommand();
                    // Избавиться от возможного SQL_Injection
                    sqlCommand.CommandText = $"SELECT * FROM product WHERE price BETWEEN {minPrice} AND {maxPrice};";
                    sqlCommand.Connection = sqlConnection;

                    using (DbDataReader reader = sqlCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            products.Add(new Product(Int32.Parse(reader["id"].ToString()), reader["name"].ToString(), Int32.Parse(reader["category_id"].ToString()), Int32.Parse(reader["code"].ToString()), double.Parse(reader["price"].ToString()), Int32.Parse(reader["availability"].ToString()), reader["brand"].ToString(), reader["description"].ToString(), Int32.Parse(reader["is_new"].ToString()), Int32.Parse(reader["is_recommended"].ToString()), Int32.Parse(reader["status"].ToString())));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return products;
        }

        static void Main(string[] args)
        {
            string API_token = "766708677:AAFO6OieruPegHjdTe0b7zaLAIoo2qJuJ10";
            // создать клиента бота на основе токена, который даёт Botfather
            // при создании бота
            Bot = new TelegramBotClient(API_token);

            // подписка на событие -- когда будет приходить сообщение,
            // будет вызываться метод BotOnMessageReceived
            Bot.OnMessage += BotOnMessageReceived;

            // а это для InlineKeyboardButton.WithCallbackData("Пункт 2")
            // при нажатии на кнопку "Пункт 2" будет срабатывать метод BonOnCallbackReceived
            Bot.OnCallbackQuery += BonOnCallbackReceived;

            // выдаст имя бота
            var me = Bot.GetMeAsync().Result;

            Console.WriteLine(me.FirstName); // название бота: "Чат-бот 08.05.19"
            // начать получение сообщений
            Bot.StartReceiving();

            Console.ReadLine();

            // остановить получение сообщений
            Bot.StopReceiving();
        }

        public static string MinifyHTML(string html)
        {
            string html2;
            while ((html2 = Regex.Replace(html, @"[ \t\r\n\f][ \t\r\n\f]", " ", RegexOptions.Singleline)).CompareTo(html) != 0)
            {
                html = string.Copy(html2);
            }

            while ((html2 = Regex.Replace(html, @"> <", "><", RegexOptions.Singleline)).CompareTo(html) != 0)
            {
                html = string.Copy(html2);
            }

            return html2;
        }

        private static async void BonOnCallbackReceived(object sender, Telegram.Bot.Args.CallbackQueryEventArgs e)
        {
            string buttonText = e.CallbackQuery.Data;

            // для отладки
            string name = $"{e.CallbackQuery.From.FirstName} {e.CallbackQuery.From.LastName}";
            Console.WriteLine($"{name} нажал кнопку {buttonText}");

            var products = GetAllProductsByCategoryName(buttonText);

            for (int i = 0; i < products?.Count; ++i)
            {
                SendImageAndDescriptionOfProduct(products[i], e.CallbackQuery.From.Id);
            }

            // значит нажата кнопка, не содержащая название категории товара
            if (products == null)
            {
                switch (buttonText)
                {
                    case "🔍 Поиск":
                        // отправка текста пользователю ( у каждого пользователя свой отдельный чат с ботом )
                        // message.From.Id -- Id чата
                        // await Bot.SendTextMessageAsync(e.CallbackQuery.From.Id, "Введите часть названия товара:");

                        InlineKeyboardButton nameButton = InlineKeyboardButton.WithCallbackData("Часть названия");
                        InlineKeyboardButton priceButton = InlineKeyboardButton.WithCallbackData("Цена");
                        InlineKeyboardButton codeButton = InlineKeyboardButton.WithCallbackData("Код");
                        List<List<InlineKeyboardButton>> categoriesGroupsOfButtons = new List<List<InlineKeyboardButton>>();
                        categoriesGroupsOfButtons.Add(new List<InlineKeyboardButton>(new[] { nameButton }));
                        categoriesGroupsOfButtons.Add(new List<InlineKeyboardButton>(new[] { priceButton }));
                        categoriesGroupsOfButtons.Add(new List<InlineKeyboardButton>(new[] { codeButton }));
                        var catalogInlineKeyboard = new InlineKeyboardMarkup(categoriesGroupsOfButtons);
                        try
                        {
                            // отправка клавиатуры в чат пользователю
                            await Bot.SendTextMessageAsync(e.CallbackQuery.From.Id, "Выберите критерий поиска товара:", replyMarkup: catalogInlineKeyboard);
                        }
                        catch
                        { }

                        break;
                    case "Цена":
                        // отправка клавиатуры в чат пользователю
                        await Bot.SendTextMessageAsync(e.CallbackQuery.From.Id, "Цена товара до (грн):");
                        break;
                    case "Часть названия":
                        // отправка клавиатуры в чат пользователю
                        await Bot.SendTextMessageAsync(e.CallbackQuery.From.Id, "Название товара содержит:");
                        break;
                    case "Код":
                        // отправка клавиатуры в чат пользователю
                        await Bot.SendTextMessageAsync(e.CallbackQuery.From.Id, "Код товара (начните с f):");
                        break;
                    default:
                        break;
                }
            }
        }

        private static string WrapTextByHtmlTemplate(string text)
        {
            return "<!DOCTYPE html><html lang='en'><head><meta charset='UTF-8'><title></title></head><body>" + text + "</body></html>";
        }

        public static async void SendImageAndDescriptionOfProduct(Product product, int chatId)
        {
            var FileUrl = $@"C:\\ospanel\\domains\\eshop\\upload\\images\\products\\{product.id}.jpg";
            using (var stream = System.IO.File.Open(FileUrl, FileMode.Open))
            {
                string fileName = FileUrl.Split('\\').Last();
                string text = $"{product.name}\nЦена: {product.price} грн.\nПодробнее: https://scehlov.000webhostapp.com/product/{product.id}";
                var test = await Bot.SendPhotoAsync(chatId, new InputOnlineFile(stream, fileName), text);
            }
        }

        //  async -- асинхронная обработка получаемых сообщений
        // можно одновременно получать и обрабатывать сообщения от разных пользователей
        private static async void BotOnMessageReceived(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            var message = e.Message;

            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            // если тип сообщения -- не текст, выходим из метода
            if (message == null || message.Type != MessageType.Text)
                return;

            // получить имя и фамилию пользовтеля, отправившего сообщение
            string name = $"{message.From.FirstName} {message.From.LastName}";

            //Console.WriteLine($"{name} отправил сообщение: {message.Text}");

            switch (message.Text)
            {
                case "/start":
                    string text =
    "Добро пожаловать в наш Telegram-магазин\nженского белья 'EShop'!\n" +
    @"Список команд:
/start - запуск бота
/callback - вывод меню
/keyboard - вывод клавиатуры
/photo";
                    // отправка текста пользователю ( у каждого пользователя свой отдельный чат с ботом )
                    // message.From.Id -- Id чата
                    await Bot.SendTextMessageAsync(message.From.Id, text);
                    break;

                // меню
                case "/callback":
                    // массив кнопок меню
                    var inlineKeyboard = new InlineKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            InlineKeyboardButton.WithUrl("VK", "https://vk.com"),
                            InlineKeyboardButton.WithUrl("Telegram", "https://t.me")
                        },
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("Пункт 1"),
                            InlineKeyboardButton.WithCallbackData("Пункт 2")
                        }
                    });

                    try
                    {
                        // отправка клавиатуры в чат пользователю
                        await Bot.SendTextMessageAsync(message.From.Id, "Выберите пункт меню", replyMarkup: inlineKeyboard);
                    }
                    catch
                    { }

                    break;
                // клавиатура
                case "/keyboard":
                    var replyKeyboard = new ReplyKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            new KeyboardButton("📁 Каталог"),
                            new KeyboardButton("🛒 Корзина")
                        },
                        new[]
                        {
                            new KeyboardButton("📦 Заказы"),
                            new KeyboardButton("📢 Новости")
                        },
                        new[]
                        {
                            new KeyboardButton("⚙ Настройки"),
                            new KeyboardButton("❓ Помощь")
                        },
                        new[]
                        {
                            new KeyboardButton("☎ Мой Контакт") { RequestContact = true },
                            new KeyboardButton("📌 Моя Геолокация") { RequestLocation = true }
                        }
                    });
                    await Bot.SendTextMessageAsync(message.Chat.Id, "Меню", replyMarkup: replyKeyboard);
                    break;
                case "📁 Каталог":
                    // отправка текста пользователю ( у каждого пользователя свой отдельный чат с ботом )
                    // message.From.Id -- Id чата
                    await Bot.SendTextMessageAsync(message.From.Id, "Каталог");

                    List<string> categories = GetAllCategories();

                    //List<InlineKeyboardButton> categoriesButtons = new List<InlineKeyboardButton>();

                    List<List<InlineKeyboardButton>> categoriesGroupsOfButtons = new List<List<InlineKeyboardButton>>();

                    InlineKeyboardButton searchButton = InlineKeyboardButton.WithCallbackData("🔍 Поиск");
                    categoriesGroupsOfButtons.Add(new List<InlineKeyboardButton>(new[] { searchButton }));

                    foreach (string category in categories)
                    {
                        InlineKeyboardButton button = InlineKeyboardButton.WithCallbackData(category);

                        // в каждой группе кнопок будет по одной кнопке -- чтоб по горизонтали была одна кнопка и
                        // текст в ней отображался полностью (название категории товаров)
                        categoriesGroupsOfButtons.Add(new List<InlineKeyboardButton>(new[] { button }));
                    }

                    // массив кнопок меню
                    var catalogInlineKeyboard = new InlineKeyboardMarkup(categoriesGroupsOfButtons);

                    try
                    {
                        // отправка клавиатуры в чат пользователю
                        await Bot.SendTextMessageAsync(message.From.Id, "Выберите раздел, чтобы вывести список товаров:", replyMarkup: catalogInlineKeyboard);
                    }
                    catch
                    { }

                    break;

                default:
                    int code;
                    double maxPrice;
                    // меняем точки на запятые
                    message.Text = Regex.Replace(message.Text, @"\.", ",");
                    //message.Text
                    bool isDouble = Double.TryParse(message.Text, out maxPrice);
                    if (isDouble)
                    {
                        // !!! получить введенное от пользователя число -- цену товара для фильтрации запроса
                        await Bot.SendTextMessageAsync(message.Chat.Id, $"Вы ввели цену: {message.Text}");

                        // отобразить все товары сценой до или равной введенной пользователем
                        List<Product> products = GetAllProductsBetweenPrices(0, maxPrice);

                        for (int i = 0; i < products?.Count; ++i)
                        {
                            SendImageAndDescriptionOfProduct(products[i], message.From.Id);
                        }
                    }
                    else if (message.Text[0] == 'f' && Int32.TryParse(message.Text.Substring(1), out code))
                    {
                        Product product = GetProductByCode(code);
                        SendImageAndDescriptionOfProduct(product, message.From.Id);
                    }
                    else
                    {
                        // отобразить все товары с именем, содержащим часть, введенную пользователем
                        List<Product> products = GetAllProductsByName(message.Text);

                        for (int i = 0; i < products?.Count; ++i)
                        {
                            SendImageAndDescriptionOfProduct(products[i], message.From.Id);
                        }
                    }

                    break;
            }
        }

        private static Product GetProductByCode(int code)
        {
            Product product = null;

            try
            {
                using (MySqlConnection sqlConnection = new MySqlConnection())
                {
                    sqlConnection.ConnectionString = connectionString;
                    sqlConnection.Open();

                    MySqlCommand sqlCommand = sqlConnection.CreateCommand();
                    // Избавиться от возможного SQL_Injection
                    sqlCommand.CommandText = $"SELECT * FROM product WHERE code={code} LIMIT 1;"; // ???
                    sqlCommand.Connection = sqlConnection;

                    using (DbDataReader reader = sqlCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            product = new Product(Int32.Parse(reader["id"].ToString()), reader["name"].ToString(), Int32.Parse(reader["category_id"].ToString()), Int32.Parse(reader["code"].ToString()), double.Parse(reader["price"].ToString()), Int32.Parse(reader["availability"].ToString()), reader["brand"].ToString(), reader["description"].ToString(), Int32.Parse(reader["is_new"].ToString()), Int32.Parse(reader["is_recommended"].ToString()), Int32.Parse(reader["status"].ToString()));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return product;
        }

        private static List<Product> GetAllProductsByName(string text)
        {
            List<Product> products = new List<Product>();

            try
            {
                using (MySqlConnection sqlConnection = new MySqlConnection())
                {
                    sqlConnection.ConnectionString = connectionString;

                    sqlConnection.Open();

                    MySqlCommand sqlCommand = sqlConnection.CreateCommand();
                    // Избавиться от возможного SQL_Injection
                    sqlCommand.CommandText = $"SELECT * FROM product WHERE name LIKE '%{text}%';";
                    sqlCommand.Connection = sqlConnection;

                    using (DbDataReader reader = sqlCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            products.Add(new Product(Int32.Parse(reader["id"].ToString()), reader["name"].ToString(), Int32.Parse(reader["category_id"].ToString()), Int32.Parse(reader["code"].ToString()), double.Parse(reader["price"].ToString()), Int32.Parse(reader["availability"].ToString()), reader["brand"].ToString(), reader["description"].ToString(), Int32.Parse(reader["is_new"].ToString()), Int32.Parse(reader["is_recommended"].ToString()), Int32.Parse(reader["status"].ToString())));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return products;
        }
    }
}