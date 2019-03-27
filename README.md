# TelegramBotFramework
Simple framework for easily building Telegram bots using modular system inspired by [Mr. GreyWolf](https://github.com/GreyWolfDev/CSChatBot)
## Installation
![Nuget version](https://img.shields.io/nuget/v/TelegramBotFramework.svg) ![Nuget downloads](https://img.shields.io/nuget/dt/TelegramBotFramework.svg)

Available on [NuGet](https://www.nuget.org/packages/TelegramBotFramework/):
```
PM> Install-Package TelegramBotFramework
```
To get started with TelegramBotFramework first you will need to get the library itself. The easiest way to do this is to install the package into your project using [NuGet](https://www.nuget.org/packages/TelegramBotFramework/). Using Visual Studio this can be done in two ways.

### Using the package manager
In Visual Studio right click on your solution and select 'Manage NuGet Packages for solution...'. A screen will appear which initially shows the currently installed packages. In the top bit select 'Browse'. This will let you download net package from the NuGet server. In the search box type 'TelegramBotFramework' and hit enter. The TelegramBotFramework package should come up in the results. After selecting the package you can then on the right hand side select in which projects in your solution the package should install. After you've selected all project you wish to install and use TelegramBotFramework in hit 'Install' and the package will be downloaded and added to you projects.

### Using the package manager console
In Visual Studio in the top menu select 'Tools' -> 'NuGet Package Manager' -> 'Package Manager Console'. This should open up a command line interface. On top of the interface there is a dropdown menu where you can select the Default Project. This is the project that TelegramBotFramework will be installed in. After selecting the correct project type  `Install-Package TelegramBotFramework`  in the command line interface. This should install the latest version of the package in your project.

After doing either of above steps you should now be ready to actually start using TelegramBotFramework.

## Getting started
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
            return new CommandResponse($"*Hello world*", parseMode: ParseMode.Markdown);
        }
    }
````
And instatiate `TelegramBotWrapper` with your key from [BotFather](tg://@BotFather) and your telegram id from @useridbot
````C#
  var wrapper = new TelegramBotWrapper("12345:AAAAAAAAAAAAAAAbbbbxccccccc", 424242, "NewBotConfigAlias");
````
Also you can use it through dependency injection in your `Startup.cs`:
````C#
using TelegramBotFramework.Core.Extensions;
....

public void ConfigureServices(IServiceCollection services)
{
    services.AddTelegramBot("12345:AAAAAAAAAAAAAAAbbbbxccccccc", 424242, "NewBotConfigAlias");
}
````
It will be intstatiate wrapper as Singleton and this instance take care to run and use all your modules written in cuurent running solution
## Donations
Donations are greatly appreciated and a motivation to keep improving.

**Btc**:  1KhYc4yQUHpj6tjpjh64KQ9Ki77N4srCxj  
**Eth**:  0x84f892FaBCE99e3429a4318948B9bBe1434Bbe4A 
