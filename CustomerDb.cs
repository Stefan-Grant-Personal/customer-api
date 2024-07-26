/*
The code in this file defines the database context.
*/

using Microsoft.EntityFrameworkCore;

class CustomerDb : DbContext
{
    public CustomerDb(DbContextOptions<CustomerDb> options)
        : base(options) { }

    // The name of the variable "Customers" will be the name of the database table we're accessing
    public DbSet<Customer> Customers => Set<Customer>();
}