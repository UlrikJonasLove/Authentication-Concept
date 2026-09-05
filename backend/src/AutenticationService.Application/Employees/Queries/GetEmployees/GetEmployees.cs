using MediatR;
using Microsoft.EntityFrameworkCore;
using AutenticationService.Application.Common.DTOs;
using AutenticationService.Application.Interfaces;

namespace AutenticationService.Application.Employees.Queries.GetEmployees;

public record GetEmployeesQuery() : IRequest<IEnumerable<EmployeeDTO>>;

public class GetEmployeesQueryHandler(
	IApplicationDbContext context)
	: IRequestHandler<GetEmployeesQuery, IEnumerable<EmployeeDTO>>
{
	public async Task<IEnumerable<EmployeeDTO>> Handle(GetEmployeesQuery request, CancellationToken cancellationToken)
	{
		//With existing database:
		//await context.Employees
		//	.AsNoTracking()
		//	.Select(employee => EmployeeDTO.FromEntity(employee))
		//	.ToListAsync(cancellationToken);

		//With mock data:
		var _ = context;
		await Task.Delay(1);
		return new List<EmployeeDTO>()
		{
			new(1, "Marcus"),
			new(2, "Ulrik")
		};
	}
}
