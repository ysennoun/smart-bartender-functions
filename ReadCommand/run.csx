#r "Newtonsoft.Json"
#load "..\library\model\command.csx"
using System.Net;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

public static void Run(string myQueueItem, out object outputDocument, TraceWriter log)
{
    log.Info($"[CONNECTED_BAR - ReadCommand] new command received : {myQueueItem}");

    var command = JsonConvert.DeserializeObject<Command>(myQueueItem);
    Guid uuidCommand = Guid.NewGuid();
    Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
    outputDocument = new {
        id = uuidCommand.ToString(),
        sendTime = command.sendTime,
        receptionTime = unixTimestamp,
        table = command.table,
        drink = command.drink,
        status = "not_served"
    };

    log.Info($"[CONNECTED_BAR - ReadCommand] command to store : {outputDocument}");
}