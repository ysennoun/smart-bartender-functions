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
    string drinkFromPrediction = "";
    float scoreFromPrediction = 0;
    foreach (var pred in command.predictions) {
        if (pred.prediction.Contains("water") && pred.score > scoreFromPrediction) {
            drinkFromPrediction = "water";
            scoreFromPrediction = pred.score;
        }
        else if (pred.prediction.Contains("coke") && pred.score > scoreFromPrediction) {
            drinkFromPrediction = "coke";
            scoreFromPrediction = pred.score;
        }
        else if (pred.prediction.Contains("beer") && pred.score > scoreFromPrediction) {
            drinkFromPrediction = "beer";
            scoreFromPrediction = pred.score;
        }
        else if (pred.prediction.Contains("wine") && pred.score > scoreFromPrediction) {
            drinkFromPrediction = "wine";
            scoreFromPrediction = pred.score;
        }
        else if (pred.prediction.Contains("jus") && pred.score > scoreFromPrediction) {
            drinkFromPrediction = "jus";
            scoreFromPrediction = pred.score;
        }
    }
    outputDocument = new {};
    if(!string.IsNullOrEmpty(drinkFromPrediction)){
        Guid uuidCommand = Guid.NewGuid();
        Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        outputDocument = new {
            id = uuidCommand.ToString(),
            sendTime = command.sendTime,
            receptionTime = unixTimestamp,
            table = command.table,
            drink = drinkFromPrediction,
            status = "not_served"
        };
        log.Info($"[CONNECTED_BAR - ReadCommand] command to store : {outputDocument}");
    }else
        log.Info($"[CONNECTED_BAR - ReadCommand] this is not a command to store.");
}