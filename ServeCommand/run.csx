#r "Microsoft.Azure.Documents.Client"
#r "Microsoft.Azure.WebJobs.Extensions.Http"
#load "..\library\model\command.csx"
#load "..\library\dao\cosmosClient.csx"
using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
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
    log.Info("C# HTTP trigger function processed a request.");

    string idCommand = req.GetQueryNameValuePairs()
        .FirstOrDefault(q => string.Compare(q.Key, "idCommand", true) == 0)
        .Value;
    string typeItem = req.GetQueryNameValuePairs()
        .FirstOrDefault(q => string.Compare(q.Key, "type", true) == 0)
        .Value;
    if(idCommand == null || typeItem == null)
         return req.CreateResponse(HttpStatusCode.BadRequest, "Please pass a idCommand and a type on the query string or in the request body");

    Microsoft.Azure.Documents.Document document = client.CreateDocumentQuery<Microsoft.Azure.Documents.Document>(collectionUri)
            .Where(p => p.Id == idCommand)
            .AsEnumerable()
            .SingleOrDefault();

    Command commandToUpdate = (dynamic)document;
    commandToUpdate.status = SERVED_STATUS;
    Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
    commandToUpdate.servedTime = unixTimestamp;
    commandToUpdate.durationToServe = commandToUpdate.servedTime - commandToUpdate.sendTime;

    Microsoft.Azure.Documents.Document documentUpdated = (dynamic) await client.ReplaceDocumentAsync(document.SelfLink, commandToUpdate);
    Command commandUpdated = (dynamic)documentUpdated;

    return commandUpdated.status != SERVED_STATUS
        ? req.CreateResponse(HttpStatusCode.BadRequest, "Could not replace")
        : req.CreateResponse(HttpStatusCode.OK, "idCommand is " + idCommand + ", and type is " + typeItem + "; drink is " + commandUpdated.drink);
}
