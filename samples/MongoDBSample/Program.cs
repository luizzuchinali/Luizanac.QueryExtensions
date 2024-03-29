﻿var client = new MongoClient("mongodb://root:root@localhost:27017/");
var collection = client.GetDatabase("luizanac-query-extensions").GetCollection<User>("Users");

var serializerOptions = new JsonSerializerOptions
{
    WriteIndented = true,
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
};

if (collection.CountDocuments(FilterDefinition<User>.Empty) == 0)
{
    var users = DataGenerator.CreateUsers();
    collection.InsertMany(users);
}

var watcher = new Stopwatch();
//api/clients?sort=asc,name&filter=email@=@gmail.com"

//SORT
//?sort=adress.number,asc will orderBy address.number ascending
//?sort=address.number,asc|name,asc will orderBy address.number ascending then by name ascending
// | is "then by"
const string sort = "age,asc|address.number,desc";

//Filter
// >, <, >=, <=, ==, !=  Comparison operators
// @= Contains / !@= NotContains
// _= StartsWith / !_= NotStartsWith 
// $= Regex 
// | in context of filter is OR
const string filters = "email@=hotmail.com|gmail.com,name|email_=Felix";
watcher.Start();

// var options = new FindOptions<User>()
// {
//     Limit = 10
// };
// var query = collection.Query(options);
// await foreach (var entity in query.GetAsyncEnumerable())
// {
// }
var data = await ((IMongoQueryable<User>)collection.AsQueryable().Filter(filters).OrderBy(sort)).ToListAsync();

watcher.Stop();
WriteLine(JsonSerializer.Serialize(data, serializerOptions));
WriteLine($"elapsed {watcher.ElapsedMilliseconds}ms");
ReadLine();