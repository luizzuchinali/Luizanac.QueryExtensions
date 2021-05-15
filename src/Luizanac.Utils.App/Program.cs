using System;
using Luizanac.Utils.Extensions;
using System.Text.Json;
using Luizanac.Utils.Contexts.App;
using System.Linq;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

var serializerOptions = new JsonSerializerOptions
{
	WriteIndented = true,
	PropertyNamingPolicy =
	JsonNamingPolicy.CamelCase
};

//api/clients?sort=asc,name&filter=name@=lui,email!@=@gmail.com,age!=20,age>19"

//?sort=asc,name&filters=age>=16,name@=leffler,name_=h

// >, <, >=, <=, ==, !=  Comparison operators
// @= Contains / !@= NotContains = generate Like/ILike %value%
// _= StartsWith / !_= NotStartsWith = generate Like/ILike value%

var sort = "age,asc";
var filters = "age>=16,email@=hotmail.com,name_=h";

using var dbContext = new AppDbContext();
var timer = Stopwatch.StartNew();

timer.Start();
var totalServerData = await dbContext.Clients.CountAsync();
var paginatedData =
	await dbContext.Clients
		.AsNoTracking()
		.Filter(filters)
		.OrderBy(sort)
		.Paginate(1, 3);

timer.Stop();

Console.WriteLine($"{timer.ElapsedMilliseconds} ms");
Console.WriteLine($"TotalServerData: {totalServerData}");
Console.WriteLine($"Pages: {paginatedData.TotalPages}");
Console.WriteLine($"CurrentPage: {paginatedData.CurrentPage}");
Console.WriteLine($"TotalDataCount: {paginatedData.TotalDataCount}");
Console.WriteLine(JsonSerializer.Serialize(paginatedData.Data, serializerOptions));
