// You can write to a bot and get a response while this program is running

using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

var botClient = new TelegramBotClient("6577043335:AAHUC35x7EtYlC8kIWc36lS4SUuS5P092CE");

using CancellationTokenSource cts = new();

// StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
ReceiverOptions receiverOptions = new()
{
    AllowedUpdates = Array.Empty<UpdateType>() // receive all update types except ChatMember related updates
};

botClient.StartReceiving(
    updateHandler: HandleUpdateAsync,
    pollingErrorHandler: HandlePollingErrorAsync,
    receiverOptions: receiverOptions,
    cancellationToken: cts.Token
);

var me = await botClient.GetMeAsync();

Console.WriteLine($"Start listening for @{me.Username}");
Console.ReadLine();

// Send cancellation request to stop bot
cts.Cancel();

async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    // Only process Message updates: https://core.telegram.org/bots/api#message
    if (update.Message is not { } message)
        return;
    // Only process text messages
    if (message.Text is not { } messageText)
        return;

    var chatId = message.Chat.Id;

    Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");

    // // Echo received message text
    // Message sentMessage = await botClient.SendTextMessageAsync(
    //    chatId: chatId,
    //    text: messageText,
    //    cancellationToken: cancellationToken);

    if (message.Text == "Hello")
    {
        Message newMessage = await botClient.SendTextMessageAsync(
            chatId: chatId,
            text: "Hello",
            cancellationToken: cancellationToken);
    }

    //Message messageTwo = await botClient.SendPhotoAsync(
    //chatId: chatId,
    //photo: InputFile.FromUri("https://commons.wikimedia.org/wiki/File:Киев._Привет_из_Киева._Почтовая_открытка_№1319.jpg"),
    ////caption: "<i>Source</i>: <a href=\"https://wikimedia.org\">Wikimedia</a>",
    //parseMode: ParseMode.Html,
    //cancellationToken: cancellationToken);

    if (message.Text == "Poll")
    {
        var pollMessage = await botClient.SendPollAsync(
            chatId: chatId,
            question: "Yes or no?",
            options: new[]
            {
                "Yes",
                "No"
            },
            cancellationToken: cancellationToken);
    }

    if (message.Text == "How to call my mother?")
    {
        Message message1 = await botClient.SendContactAsync(
    chatId: chatId,
    phoneNumber: "88005553535",
    firstName: "Your",
    lastName: "Mother",
    cancellationToken: cancellationToken);
    }

    if (message.Text == "Where I am?")
    {
        Message message2 = await botClient.SendVenueAsync(
    chatId: chatId,
    latitude: 59.589806f,
    longitude: 32.461641f,
    title: "Here",
    address: "Pchyovzha rural settlement, Kirishsky district, Russian Federation",
    cancellationToken: cancellationToken);
    }

    // ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
    // {
    // new KeyboardButton[] { "1" },
    // new KeyboardButton[] { "2" },
    // new KeyboardButton[] { "3" },
    // new KeyboardButton[] { "4" },
    // })
    // {
    //     ResizeKeyboard = true
    // };

    // Message sentMessage = await botClient.SendTextMessageAsync(
    //     chatId: chatId,
    //     text: "Choose the number",
    //     replyMarkup: replyKeyboardMarkup,
    //     cancellationToken: cancellationToken);

    if (message.Text == "Who are you?")
    {
        InlineKeyboardMarkup inlineKeyboard = new(new[]
        {
        InlineKeyboardButton.WithUrl(
        text: "Me",
        url: "https://github.com/t3mv-l")
        });
        Message sentMessage2 = await botClient.SendTextMessageAsync(
            chatId: chatId,
            text: "Yeah, that's me",
            replyMarkup: inlineKeyboard,
            cancellationToken: cancellationToken);
    }

}

Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
{
    var ErrorMessage = exception switch
    {
        ApiRequestException apiRequestException
            => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
        _ => exception.ToString()
    };

    Console.WriteLine(ErrorMessage);
    return Task.CompletedTask;
}