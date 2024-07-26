/*
This file defines the Data Transfer Object (DTO), in other words, the subset of the Customer class that is exposed by the
API.
*/

public class CustomerDTO
{
    // Define the same fields as Customer, except for `Secret`
    public int Id { get; set; }
    public string? FirstName { get; set; }
    public string? Surname { get; set; }
    public int Age { get; set; }

    // Constructors
    public CustomerDTO(Customer customer) =>
    (Id, FirstName, Surname, Age) = (customer.Id, customer.FirstName, customer.Surname, customer.Age);
}