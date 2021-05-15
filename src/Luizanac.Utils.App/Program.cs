using System;
using Luizanac.Utils.Extensions;
using System.Text.Json;
using Luizanac.Utils.Contexts.App;
using System.Linq;
using System.Diagnostics;

var serializerOptions = new JsonSerializerOptions
{
	WriteIndented = true,
	PropertyNamingPolicy =
	JsonNamingPolicy.CamelCase
};

//api/clients?sort=asc,name&filter=name,contains,lui&filter=email,notContains,@gmail&age,greatherThanOrEqualTo,20

//?sort=asc,name&filters=age>20,name@=luiz

var filters = "age!=20,age>19,name!=Harold johns";

var sort = "age,asc";

using var dbContext = new AppDbContext();
var timer = Stopwatch.StartNew();
timer.Start();

var paginatedData =
	await dbContext.Clients
		.AsQueryable()
		.Filter(filters)
		.OrderBy(sort)
		.Paginate(4, 3);

timer.Stop();

Console.WriteLine($"{timer.ElapsedMilliseconds} ms");
Console.WriteLine($"Pages: {paginatedData.TotalPages}");
Console.WriteLine($"CurrentPage: {paginatedData.CurrentPage}");
Console.WriteLine($"TotalDataCount: {paginatedData.TotalDataCount}");
Console.WriteLine(JsonSerializer.Serialize(paginatedData.Data, serializerOptions));
