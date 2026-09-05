using MediatR;
using Microsoft.EntityFrameworkCore;
using AutenticationService.Application.Interfaces;

namespace AutenticationService.Application.Employees.Commands.UpdateEmployee;

public record UpdateEmployeeCommand(long Id, string Name) : IRequest;

public class UpdateEmployeeCommandHandler(
	IApplicationDbContext context)
	: IRequestHandler<UpdateEmployeeCommand>
{
	public async Task Handle(UpdateEmployeeCommand command, CancellationToken cancellationToken)
	{
		var entity = await context.Employees
			.AsTracking()
			.FirstAsync(e => e.Id == command.Id, cancellationToken);

		entity.Name = command.Name;

		await context.SaveChangesAsync(cancellationToken);
	}
}
