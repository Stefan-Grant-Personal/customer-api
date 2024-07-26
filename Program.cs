using Microsoft.EntityFrameworkCore;
using CustomerApi.SQLiteUtils;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Use SQLite for a file-based database (persistent storage)
const string DatabaseFile = "Customers.db";
if (!File.Exists(DatabaseFile))
{
    // Create a DB file if it doesn't already exist
    const string CreateTableQuery = @"CREATE TABLE [Customers] (
                                            [ID] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                                            [FirstName] NVARCHAR(25) NOT NULL,
                                            [Surname] NVARCHAR(25) NOT NULL,
                                            [Age] INTEGER,
                                            [Secret] NVARCHAR(500)
                                            )";
    SQLiteDbBuilder.CreateDb(DatabaseFile, CreateTableQuery);
}
builder.Services.AddDbContext<CustomerDb>(opt => opt.UseSqlite($"Data Source={DatabaseFile}"));

// Enable displaying database-related exceptions
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Enable Swagger OpenAPI documentation generation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(config =>
{
    config.DocumentName = "CustomerAPI";
    config.Title = "CustomerAPI v1";
    config.Version = "v1";
});

WebApplication app = builder.Build();

app.UseOpenApi();
app.UseSwaggerUi(config =>
{
    config.DocumentTitle = "CustomerAPI";
    config.Path = "/swagger";
    config.DocumentPath = "/swagger/{documentName}/swagger.json";
    config.DocExpansion = "list";
});


// Define the API endpoints

// Make the root endpoint display a useful message instead of returning a 404
app.MapGet("/", () =>
    "Hello! This is not a valid endpoint. Please navigate to http://localhost:<port>/swagger instead.");


// Create a Map Group to group endpoints
RouteGroupBuilder customerEndpoint = app.MapGroup("/customers");

customerEndpoint.MapGet("/", GetAllCustomers);
if (app.Environment.IsDevelopment())
{
    // This method should never be exposed, it's purely for testing
    customerEndpoint.MapGet("/withsecrets", GetAllCustomersWithSecrets);
}
customerEndpoint.MapGet("/{id}", GetCustomer);
customerEndpoint.MapPost("/", CreateCustomer);
customerEndpoint.MapPut("/{id}", UpdateCustomer);
customerEndpoint.MapDelete("/{id}", DeleteCustomer);

app.Run();

static async Task<IResult> GetAllCustomers(CustomerDb db)
{
    return TypedResults.Ok(await db.Customers.Select(x => new CustomerDTO(x)).ToListAsync());
}

static async Task<IResult> GetAllCustomersWithSecrets(CustomerDb db)
{
    return TypedResults.Ok(await db.Customers.ToListAsync());
}

static async Task<IResult> GetCustomer(int id, CustomerDb db)
{
    Customer? customer = await db.Customers.FindAsync(id);

    if (customer is null)
        return TypedResults.NotFound();

    return TypedResults.Ok(new CustomerDTO(customer));
}

static async Task<IResult> CreateCustomer(Customer inputCustomer, CustomerDb db)
{
    // The Add() method here overwrites the value of Id
    db.Customers.Add(inputCustomer);
    await db.SaveChangesAsync();

    // Now create a new DTO from the created Customer object, so that we show what has been created
    // in the response, without the secret
    var customerDTO = new CustomerDTO(inputCustomer);

    return TypedResults.Created($"/customers/{inputCustomer.Id}", customerDTO);
}

static async Task<IResult> UpdateCustomer(int id, Customer inputCustomer, CustomerDb db)
{
    Customer? customer = await db.Customers.FindAsync(id);

    if (customer is null)
        return TypedResults.NotFound();

    customer.FirstName = inputCustomer.FirstName;
    customer.Surname = inputCustomer.Surname;
    customer.Age = inputCustomer.Age;
    customer.Secret = inputCustomer.Secret;

    await db.SaveChangesAsync();

    return TypedResults.NoContent();
}

static async Task<IResult> DeleteCustomer(int id, CustomerDb db)
{
    Customer? customer = await db.Customers.FindAsync(id);

    if (customer is null)
        return TypedResults.NotFound();

    db.Customers.Remove(customer);
    await db.SaveChangesAsync();

    return TypedResults.NoContent();
}
