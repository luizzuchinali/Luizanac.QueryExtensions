var serializerOptions = new JsonSerializerOptions
{
    WriteIndented = true,
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
};

//api/users?sort=asc,name&filter=name@=lui,email!@=@gmail.com,age!=20,age>19"

//?sort=asc,name&filters=age>=16,name@=leffler,name_=h

// >, <, >=, <=, ==, !=  Comparison operators
// @= Contains / !@= NotContains = generate Like/ILike %value%
// _= StartsWith / !_= NotStartsWith = generate Like/ILike value%

var sort = "address.city,asc";
// var filters = "age>=16,email@=hotmail.com,name_=h";
var filters = "age>=16,email@=hotmail.com|gmail.com,name|email_=l,address.city_=lake,address.number==199";

using var dbContext = new AppDbContext();

if (!dbContext.Users.Any())
    dbContext.Seed();

var query = dbContext.Users
    .AsNoTrackingWithIdentityResolution()
    .Filter(filters)
    .Paginate(1, 3)
    .OrderBy(sort);

var sqlString = query.ToQueryString();

Console.WriteLine($"\n\n{sqlString}\n\n");

var data = query.ToListAsync();

Console.WriteLine(JsonSerializer.Serialize(data, serializerOptions));