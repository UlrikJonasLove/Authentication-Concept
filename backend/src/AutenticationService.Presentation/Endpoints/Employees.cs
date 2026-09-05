using MediatR;
using Microsoft.AspNetCore.Mvc;
using AutenticationService.Application.Common.DTOs;
using AutenticationService.Application.Employees.Commands.UpdateEmployee;
using AutenticationService.Presentation.Infrastructure;
using AutenticationService.Application.Employees.Queries.GetEmployees;

namespace AutenticationService.Presentation.Endpoints;

public class Employees : EndpointGroupBase
{
	public override void Map(WebApplication app)
	{
		var employeeGroup = app.MapGroup(this);

		employeeGroup
			.MapGet(GetAllAsync);

		employeeGroup
			.MapPut(UpdateAsync);
	}

	public async Task<IEnumerable<EmployeeDTO>> GetAllAsync(ISender sender) =>
		await sender.Send(new GetEmployeesQuery());

	public async Task UpdateAsync(
		ISender sender,
		[FromBody] UpdateEmployeeCommand command) =>
		await sender.Send(command);
}
