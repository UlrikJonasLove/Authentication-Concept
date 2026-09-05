using AutenticationService.Domain.Entities;

namespace AutenticationService.Application.Common.DTOs;

public record EmployeeDTO(
	long Id,
	string Name
)
{
	public static EmployeeDTO FromEntity(Employee employee) =>
		new(employee.Id, employee.Name);
}


