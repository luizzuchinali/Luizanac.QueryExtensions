# Luizanac.QueryExtensions
Simple and clean LINQ extensions library for .NET that **adds sorting, filtering, and pagination functionality to IQueryable**.

### Luizanac.QueryExtensions [![NuGet](https://img.shields.io/nuget/v/Luizanac.QueryExtensions.svg)](https://www.nuget.org/packages/Luizanac.QueryExtensions)

> Install-Package Luizanac.QueryExtensions

OR

> dotnet add package Luizanac.QueryExtensions

### Examples

All following examples consider a **Client** class.

```C#
public class Client
{
    public Guid Id {get; private set; }
    public string Name { get; private set; }
    public string Email { get; private set; }
    public int Age { get; private set; }
}
```

## OrderBy

To use **OrderBy** extension, you will need to pass **"property, asc/desc"** (You can use first level navigation properties to **"address.number, asc/desc"**, this will access **Address** property on **Client** and then **Number** on **Address**).

```C#
var sort = "age,asc";
var clients = await dbContext.Clients.AsNoTracking().OrderBy(sort).ToListAsync();
//this example will return all cleints classified by their ascending age
```
```C#
var sort = "name,desc";
var clients = await dbContext.Clients.AsNoTracking().OrderBy(sort).ToListAsync();
//this example will return all cleints classified by their descending name
```

## Filter by properties

To use filter extension, you will need to pass a string **"{property}{operator}{data}"** with your conditions separated by commas (Like order by extension, you can use first level navigation properties too).

below you can see all supported operators

| Operator   |                          |
|------------|--------------------------|
| `==`       | Equals                   |
| `!=`       | Not equals               |
| `>`        | Greater than             |
| `>=`       | Greater than or equal to |
| `<`        | Less than                |
| `<=`       | Less than or equal to    |
| `@=`       | Contains                 |
| `!@=`      | Does not Contains        |
| `_=`       | Starts with              |
| `!_=`      | Does not Starts with     |

and here you can see an using example

```C#
var filters = "age>=16,email@=hotmail.com,name_=h";
var clients = await dbContext.Clients.AsNoTracking().Filter(filters).ToListAsync();
//This will filter all clients by age greather than or equal to 16, email contains hotmail.com and name starts with h.
```

you can use "|" to make OR. **"{property}|{property}{operator}{data}"** or **"{property}{operator}{data}|{data}"**

```C#
var sort = "age,asc";
var filters = "age>=16,email@=hotmail.com|gmail.com,name|email_=luiz";
var clients = await dbContext.Clients.AsNoTracking().Filter(filters).OrderBy(sort).ToListAsync();
//This will filter all clients by age greather than or equal to 16, email contains hotmail.com and name starts with h.
```

The previous code will generate this SQL

```SQL
SELECT [c].[Id], [c].[Age], [c].[Email], [c].[Name]
    FROM [Clients] AS [c]
        WHERE 
            (([c].[Age] >= 16) AND 
            (([c].[Email] LIKE '%hotmail.com%') OR ([c].[Email] LIKE '%gmail.com%'))) AND 
            (([c].[Name] LIKE 'luiz%') OR ([c].[Email] LIKE 'luiz%'))
ORDER BY [c].[Age]
```

you can use all extensions together

```C#
var sort = "age,asc";
var filters = "age>=16,email@=hotmail.com,name_=h";
var paginatedData = await dbContext.Clients.AsNoTracking()
            .Filter(filters)
            .OrderBy(sort)
            .Paginate(1, 3);
```
