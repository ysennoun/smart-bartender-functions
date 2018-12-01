#r "Microsoft.Azure.Documents.Client"
#r "Microsoft.Azure.WebJobs.Extensions.Http"
#r "System.Web"
#r "System.Web.Extensions"
#load "..\library\model\command.csx"
#load "..\library\dao\cosmosClient.csx"
using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.Script.Serialization;
using System.Threading.Tasks;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;


private static DocumentClient client = GetCustomClient();
private static Uri collectionUri = GetCollectionUri();
private static string SERVED_STATUS = "served";

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Info($"[CONNECTED_BAR - GetCommand] Http Request to get all appending commands.");

    IDocumentQuery<Command> query = client.CreateDocumentQuery<Command>(collectionUri)
        .Where(p => p.status != SERVED_STATUS)
        .AsDocumentQuery();
    var commandsToReturn = new List<CommandToReturn>();
    while (query.HasMoreResults)
    {
        foreach (Command result in await query.ExecuteNextAsync())
        {
            log.Info(result.drink.ToString());
            commandsToReturn.Add(new CommandToReturn() {
                id = result.id.ToString(),
                drink = result.drink.ToString(),
                sendTime = convertTimestampToDate(result.sendTime),
                }
            );
        }
    }
    var serializer = new JavaScriptSerializer();
    var serializedResult = serializer.Serialize(commandsToReturn);

    log.Info($"[CONNECTED_BAR - GetCommand] Appending commands : {serializedResult.ToString()}");
    return req.CreateResponse(HttpStatusCode.OK, serializedResult.ToString());
}

private static String convertTimestampToDate(Int32 timestamp) {
    DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(timestamp).ToLocalTime();
    return dt.ToString("dd-MM-yyyy hh:mm");
}
