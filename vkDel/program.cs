using System;
using System.Collections;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using VkNet;
using System.Threading;
using VkNet.Enums.Filters;
using VkNet.Model;
using VkNet.Model.RequestParams;
using VkNet.Utils;

var api = new VkApi();

while (true)
{
    int pier_id, id_user;
    int KolVo = 0;
    try
    {
        Console.Write("Введите Access Token: ");
        string AT = Console.ReadLine();

        Console.Write("Введите pier_id: ");
        pier_id = Convert.ToInt32(Console.ReadLine());

        Console.Write("Введите id юзера: ");
        id_user = Convert.ToInt32(Console.ReadLine());

        api.Authorize(new ApiAuthParams
        {
            AccessToken = AT
        });
        var res = api.Groups.Get(new GroupsGetParams());

        if (res.TotalCount == 2)
            Console.WriteLine("User Auth: access");
        Console.Write("Нажмите Enter чтобы продолжить>>>");
        Console.ReadLine();
    }
    catch
    {
        continue;
    }
    while (true)
    {
        try
        {
            Thread.Sleep(1000);
            KolVo++;
            var getHistory = api.Messages.GetHistory(new MessagesGetHistoryParams
            {
                PeerId = pier_id,
                Count = 5
            });
            foreach (var message in getHistory.Messages)
            {
                if (message.FromId == id_user)
                {
                    api.Call(methodName: "messages.delete", parameters: new
                            VkParameters
                          {
                {"cmids", (ulong)message.ConversationMessageId },
                {"delete_for_all", true },
                    {"peer_id", (ulong)message.PeerId}
                          });
                    Console.WriteLine($"DELETED Message:{message.FromId}: {message.Text}");
                    using (var file = new StreamWriter("log.txt", true))
                    {
                        file.WriteLine($"{DateTime.Now}{message.FromId}:{message.Text}");
                        file.Close();
                    }
                }
            }
        }
        catch
        {
            Console.WriteLine("Произошла ошибка, возможно у юзера права администратора, либо у вас нет прав!!!");
            continue;
        }
    }
}
