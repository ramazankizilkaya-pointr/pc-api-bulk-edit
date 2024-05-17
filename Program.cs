// See https://aka.ms/new-console-template for more information
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

var token = await GetToken();
Console.WriteLine(token);
var siteIdList = new List<int>();

await GetSites();


async Task<string> GetToken()
{
    using var client = new HttpClient();
    client.BaseAddress = new Uri("https://qa-xlarge.gateway.pointr.cloud");
    var paylod = new
    {
        client_id = "cda76359-0a4e-4b19-82fb-629c7954d349",
        client_secret = "0b2b8d9d-a1a6-4fe2-9b49-0461fbdf1a4c",
        grant_type = "client_credentials"
    };

    var jsonBody = JsonSerializer.Serialize(paylod);
    var postData = new StringContent(jsonBody, System.Text.Encoding.UTF8, "application/json");
    var response = await client.PostAsync("https://qa-xlarge-gateway.pointr.cloud/api/v8/auth/token", postData);
    var result = await response.Content.ReadAsStringAsync();
    var JsonObj = JsonObject.Parse(result);
    var aa = JsonObj["result"]["access_token"];
    var bb = aa.ToString();
    return JsonObj["result"]["access_token"].ToString();
}


async Task GetSites()
{
    using var client = new HttpClient();
    string token = await GetToken();
    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    var response = await client.GetAsync("https://qa-xlarge-gateway.pointr.cloud/api/v8/clients/cda76359-0a4e-4b19-82fb-629c7954d349/sites/draft");
    var responsePayload = await response.Content.ReadAsStringAsync();
    var jsonResult = JsonObject.Parse(responsePayload);
    var sitesArr = (JsonArray)jsonResult["result"]["sites"];
    foreach(var site in sitesArr)
    {
        
        if ((string)site["siteTitle"] != "Big Site")
        {
            siteIdList.Add((int)site["siteInternalIdentifier"]);
        }
    }
}


async Task DeleteSite(int siteId)
{
    using var client = new HttpClient();
    string token = await GetToken();
    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    var response = await client.DeleteAsync($"https://qa-xlarge-gateway.pointr.cloud/api/v8/sites/{siteId}");
    if (response.IsSuccessStatusCode)
    {
        Console.WriteLine($"Site with siteId {siteId} deleted");
    }
}



siteIdList.Sort();

foreach (var siteId in siteIdList)
{
    await DeleteSite(siteId);
}

Console.ReadLine();