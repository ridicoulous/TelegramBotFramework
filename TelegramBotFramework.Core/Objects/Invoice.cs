using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Payments;

namespace TelegramBotFramework.Core.Objects
{
    public class InvoiceDto
    {
      
        public InvoiceDto(long userId, string title, string description,  string currency, string product, int amount)
        {
            UserId = userId;
            Goods = new List<LabeledPrice>() { new LabeledPrice(product,amount)};
            Title = title;
            Description = description;
            Currency = currency;
        }
        public InvoiceDto(Message message, string title, string description, List<LabeledPrice> goods, string currency, string imageUrl, int height, int width)
        {
            UserId = message.Chat.Id;
            Goods = goods;
            Title = title;
            Description = description;
            Currency = currency;
        }
        public string TelegramId { get; set; }
        public string PaymentProviderId { get; set; }
        public long UserId { get; set; }
        public string PayloadId { get; set; } = Guid.NewGuid().ToString();
        public string ImageUrl { get; set; }
        public string Title { get; set; } = "Payment title was not setted";
        public string Description { get; set; } = "Payment description was not setted";
        public  List<LabeledPrice> Goods { get; set; }
        public string Currency { get; set; }
        /// <summary>
        /// This is used for when your user forwards the invoice to another chat. When that invoice is forwarded, 
        ///  it will have an "Order for ${amount}" button attached to it, however the button will go to your bot
        ///  and start it with the parameter there. So your bot will receive /start {start_parameter}. 
        ///  From there you can generate a similar invoice to the one the original user sent.
        /// </summary>
        public string StartParameter { get; set; }


    }
}
