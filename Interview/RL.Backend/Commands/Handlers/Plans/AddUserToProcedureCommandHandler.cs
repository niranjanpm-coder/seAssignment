using MediatR;
using Microsoft.EntityFrameworkCore;
using RL.Backend.Exceptions;
using RL.Backend.Models;
using RL.Data;
using RL.Data.DataModels;

namespace RL.Backend.Commands.Handlers.Plans;

public class AddUserToProcedureCommandHandler : IRequestHandler<AddUserToProcedureCommand, ApiResponse<Unit>>
{
    private readonly RLContext _context;

    public AddUserToProcedureCommandHandler(RLContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<Unit>> Handle(AddUserToProcedureCommand request, CancellationToken cancellationToken)
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

            var planProcedure = await _context.PlanProcedures
                .Include(pp => pp.PlanProcedureUsers)
                .FirstOrDefaultAsync(pp => pp.PlanId == request.PlanId && pp.ProcedureId == request.ProcedureId);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == request.UserId);

            if (planProcedure == null)
                return ApiResponse<Unit>.Fail(new NotFoundException($"PlanProcedure not found for PlanId: {request.PlanId}, ProcedureId: {request.ProcedureId}"));
            if (user == null)
                return ApiResponse<Unit>.Fail(new NotFoundException($"UserId: {request.UserId} not found"));

            // Already has the user, so just succeed
            if (planProcedure.PlanProcedureUsers.Any(pu => pu.UserId == user.UserId))
                return ApiResponse<Unit>.Succeed(new Unit());

            planProcedure.PlanProcedureUsers.Add(new PlanProcedureUser
            {
                PlanId = request.PlanId,
                ProcedureId = request.ProcedureId,
                UserId = request.UserId
            });

            await _context.SaveChangesAsync(cancellationToken);

            return ApiResponse<Unit>.Succeed(new Unit());
        }
        catch (Exception e)
        {
            return ApiResponse<Unit>.Fail(e);
        }
    }
}