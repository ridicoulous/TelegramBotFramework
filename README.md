# TL;DR

See [Example here](https://github.com/ridicoulous/TelegramBotFramework/tree/master/TelegramBotFramework.Example)

# TelegramBotFramework
Simple framework for easily building Telegram bots using modular system inspired by [Mr. GreyWolf](https://github.com/GreyWolfDev/CSChatBot)
It uses reflection to automatically find all your modules and gets it's configuration from attributes. All the routine work will be done without your intervention. Just write logic and choose some magic word that will be trigger it's execution
## Installation
![Nuget version](https://img.shields.io/nuget/v/TelegramBotFramework.svg) ![Nuget downloads](https://img.shields.io/nuget/dt/TelegramBotFramework.svg)

Available on [NuGet](https://www.nuget.org/packages/TelegramBotFramework/):
```
PM> Install-Package TelegramBotFramework
```
To get started with TelegramBotFramework first you will need to get the library itself. The easiest way to do this is to install the package into your project using [NuGet](https://www.nuget.org/packages/TelegramBotFramework/). 

## Getting started
First of all you must get your own token to communicate with telegram bot api. Open your Telegram application and type in search BotFather. Send him `/newbot` command and follow up instructions

After  it's time to actually use it. To get started we have to add the TelegramBotFramework namespace:  `using TelegramBotFramework;`.

TelegramBotFramework uses wonderful [library](https://github.com/TelegramBots/Telegram.Bot) to  interact with the [Telegram](https://telegram.org) API. 
All what you have to do:
### Create your own module inherited from `TelegramBotModuleBase`
````C#
    [TelegramBotModule(Author = "mr. fourty two", Name = "HelloWorld", Version = "1.0")]
    public class TestModule : TelegramBotModuleBase
    {
        public TestModule(TelegramBotWrapper wrapper) : base(wrapper)
        {

        }    
        [ChatCommand(Triggers = new[] { "start" }, HideFromInline = true, DontSearchInline = true)]
        public CommandResponse Start(CommandEventArgs args)
        {
            return new CommandResponse($"*Hello world:*", parseMode: ParseMode.Markdown);
        }
    }
````
And instatiate `TelegramBotWrapper` with your key from [BotFather](tg://@BotFather) and your telegram id from @useridbot
````C#
  var wrapper = new TelegramBotWrapper("12345:AAAAAAAAAAAAAAAbbbbxccccccc", 424242, "NewBotConfigAlias");
  wrapper.Run();
````
Also you can use it through dependency injection in your `Startup.cs`:
````C#
using TelegramBotFramework.Core.Extensions;
....
//Add instance to DI
public void ConfigureServices(IServiceCollection services)
{
    services.AddTelegramBot("12345:AAAAAAAAAAAAAAAbbbbxccccccc", 424242, "NewBotConfigAlias");
}

....
//Instatiate this
 public void Configure(IApplicationBuilder app)
        {
            app.UseTelegramBot();
        }

````
It will be intstatiate wrapper as Singleton and this instance take care to run and use all your modules written in cuurent running solution. By default, if you use dependency injection methods, `TelegramBotWrapper` class has injected `IServiceProvider` to have ability to use your services, added to your DI in your own modules
## Donations
Donations are greatly appreciated and a motivation to keep improving.

**Btc**:  1KhYc4yQUHpj6tjpjh64KQ9Ki77N4srCxj  
**Eth**:  0x84f892FaBCE99e3429a4318948B9bBe1434Bbe4A 
