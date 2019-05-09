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
using Telegram.Bot.Types.Enums; // для получения типа сообщения
using Telegram.Bot.Types.ReplyMarkups; // для создания клавиатуры

namespace TelegramShop
{
    class Program
    {
        static TelegramBotClient Bot;
        static void Main(string[] args)
        {
            #region DatabaseOperations

            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            // "Database=eshop;Datasource=localhost;User=root";

            try
            {
                using (MySqlConnection sqlConnection = new MySqlConnection())
                {
                    sqlConnection.ConnectionString = connectionString;

                    sqlConnection.Open();

                    MySqlCommand sqlCommand = sqlConnection.CreateCommand();
                    sqlCommand.CommandText = "SELECT * FROM category;";
                    sqlCommand.Connection = sqlConnection;

                    // Подсчитать кол-во строк в таблице
                    //object result = (new MySqlCommand("SELECT COUNT(*) FROM category", sqlConnection).ExecuteScalar());
                    //Console.WriteLine(((Int64)result));


                    using (DbDataReader reader = sqlCommand.ExecuteReader())
                    {
                        // вывести названия полей (столбцов) таблицы
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            Console.WriteLine((reader).GetName(i));
                        }
                        Console.WriteLine();

                        Console.WriteLine("Field Count = " + reader.FieldCount);

                        Console.WriteLine($"Id:\tName:\tSortOrder:\tStatus:");

                        while (reader.Read())
                        {
                            Console.WriteLine($"{reader[0]}\t{reader["name"]}\t{reader["sort_order"]}\t{reader["status"]}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            #endregion DatabaseOperations

            #region TelegramShop

            // создать клиента бота на основе токена, который даёт Botfather
            // при создании бота
            Bot = new TelegramBotClient("766708677:AAFO6OieruPegHjdTe0b7zaLAIoo2qJuJ10");

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

            #endregion TelegramShop
        }

        private static async void BonOnCallbackReceived(object sender, Telegram.Bot.Args.CallbackQueryEventArgs e)
        {
            string buttonText = e.CallbackQuery.Data;
            string name = $"{e.CallbackQuery.From.FirstName} {e.CallbackQuery.From.LastName}";
            Console.WriteLine($"{name} нажал кнопку {buttonText}");

            await Bot.AnswerCallbackQueryAsync(e.CallbackQuery.Id, $"Вы нажали кнопку {buttonText}");
        }

        //  async -- асинхронная обработка получаемых сообщений
        // можно одновременно получать и обрабатывать сообщения от разных пользователей
        private static async void BotOnMessageReceived(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            var message = e.Message;

            // если тип сообщения -- не текст, выходим из метода
            if (message == null || message.Type != MessageType.Text)
                return;

            // получить имя и фамилию пользовтеля, отправившего сообщение
            string name = $"{message.From.FirstName} {message.From.LastName}";

            Console.WriteLine($"{name} отправил сообщение: {message.Text}");

            switch (message.Text)
            {
                case "/start":
                    string text =
@"Список команд:
/start - запуск бота
/callback - вывод меню
/keyboard - вывод клавиатуры";
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
                            new KeyboardButton("Привет"),
                            new KeyboardButton("Как дела?")
                        },
                        new[]
                        {
                            new KeyboardButton("Контакт") { RequestContact = true },
                            new KeyboardButton("Геолокация") { RequestLocation = true }
                        }
                    });
                    await Bot.SendTextMessageAsync(message.Chat.Id, "Сообщение", replyMarkup: replyKeyboard);
                    break;
                default:
                    break;
            }
        }
    }
}
