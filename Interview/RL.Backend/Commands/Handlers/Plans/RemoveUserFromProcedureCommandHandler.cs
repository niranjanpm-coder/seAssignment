using MediatR;
using Microsoft.EntityFrameworkCore;
using RL.Backend.Exceptions;
using RL.Backend.Models;
using RL.Data;
using RL.Data.DataModels;

namespace RL.Backend.Commands.Handlers.Plans;

public class RemoveUserFromProcedureCommandHandler : IRequestHandler<RemoveUserFromProcedureCommand, ApiResponse<Unit>>
{
    private readonly RLContext _context;

    public RemoveUserFromProcedureCommandHandler(RLContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<Unit>> Handle(RemoveUserFromProcedureCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validate request
            if (request.PlanId < 1)
                return ApiResponse<Unit>.Fail(new BadRequestException("Invalid PlanId"));
            if (request.ProcedureId < 1)
                return ApiResponse<Unit>.Fail(new BadRequestException("Invalid ProcedureId"));
            if (request.UserId < 1)
                return ApiResponse<Unit>.Fail(new BadRequestException("Invalid UserId"));

            var user = await _context.Users.FindAsync(request.UserId);

            if (user == null)
                return ApiResponse<Unit>.Fail(new NotFoundException($"UserId: {request.UserId} not found"));

            var planProcedure = await _context.PlanProcedures
                .Include(pp => pp.PlanProcedureUsers)
                .FirstOrDefaultAsync(pp => pp.PlanId == request.PlanId && pp.ProcedureId == request.ProcedureId);

            if (planProcedure == null)
                return ApiResponse<Unit>.Fail(new NotFoundException($"PlanProcedure not found for PlanId: {request.PlanId}, ProcedureId: {request.ProcedureId}"));
        
            if (request.ClearAll)
            {
                _context.PlanProcedureUsers.RemoveRange(planProcedure.PlanProcedureUsers);
                await _context.SaveChangesAsync(cancellationToken);

                var dbPlanProcedureUsers = await _context.PlanProcedureUsers
                .Where(pu => pu.PlanId == 1 && pu.ProcedureId == 1)
                .ToListAsync();

                return ApiResponse<Unit>.Succeed(Unit.Value);
            }

            var planProcedureUser = planProcedure.PlanProcedureUsers.FirstOrDefault(x => x.UserId == request.UserId);

            if (planProcedureUser == null)
                return ApiResponse<Unit>.Succeed(Unit.Value);

            _context.PlanProcedureUsers.Remove(planProcedureUser);
            await _context.SaveChangesAsync(cancellationToken);

            return ApiResponse<Unit>.Succeed(Unit.Value);
        }
        catch (Exception e)
        {
            return ApiResponse<Unit>.Fail(e);
        }
    }
}