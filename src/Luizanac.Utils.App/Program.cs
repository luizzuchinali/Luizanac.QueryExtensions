using System;
using Luizanac.Utils.App;
using Luizanac.Utils.Extensions;
using System.Text.Json;
using Luizanac.Utils.Contexts.App;
using System.Linq;

var serializerOptions = new JsonSerializerOptions
{
    WriteIndented = true,
    PropertyNamingPolicy =
    JsonNamingPolicy.CamelCase
};

using var dbContext = new AppDbContext();
var paginatedData = await dbContext.Clients.AsQueryable()
    .Select(x => new { x.Name, x.Email })
    .OrderByString("asc,name")
    .Paginate(1, 2);

Console.WriteLine(JsonSerializer.Serialize(paginatedData.Data, serializerOptions));
