namespace AutenticationService.Domain.Entities;

public class Employee
{
	public long Id { get; init; }
	public required string Name { get; set; }
}
