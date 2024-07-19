using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using RL.Backend.Commands;
using RL.Backend.Commands.Handlers.Plans;
using RL.Backend.Exceptions;
using RL.Data;
using RL.Data.DataModels;

namespace RL.Backend.UnitTests;

[TestClass]
    public class RemoveUserFromProcedureTests
    {
        [TestMethod]
        [DataRow(-1, 1, 1)]
        [DataRow(0, 1, 1)]
        [DataRow(int.MinValue, 1, 1)]
        public async Task RemoveUserFromProcedureTests_InvalidPlanId_ReturnsBadRequest(int planId, int procedureId, int userId)
        {
            // Given
            var context = new Mock<RLContext>();
            var sut = new RemoveUserFromProcedureCommandHandler(context.Object);
            var request = new RemoveUserFromProcedureCommand()
            {
                PlanId = planId,
                ProcedureId = procedureId,
                UserId = userId
            };

            // When
            var result = await sut.Handle(request, new CancellationToken());

            // Then
            result.Exception.Should().BeOfType(typeof(BadRequestException));
            result.Succeeded.Should().BeFalse();
        }

        [TestMethod]
        [DataRow(1, -1, 1)]
        [DataRow(1, 0, 1)]
        [DataRow(1, int.MinValue, 1)]
        public async Task RemoveUserFromProcedureTests_InvalidProcedureId_ReturnsBadRequest(int planId, int procedureId, int userId)
        {
            // Given
            var context = new Mock<RLContext>();
            var sut = new RemoveUserFromProcedureCommandHandler(context.Object);
            var request = new RemoveUserFromProcedureCommand()
            {
                PlanId = planId,
                ProcedureId = procedureId,
                UserId = userId
            };

            // When
            var result = await sut.Handle(request, new CancellationToken());

            // Then
            result.Exception.Should().BeOfType(typeof(BadRequestException));
            result.Succeeded.Should().BeFalse();
        }

        [TestMethod]
        [DataRow(1, 1, 1)]
        public async Task RemoveUserFromProcedureTests_PlanProcedureNotFound_ReturnsNotFound(int planId, int procedureId, int userId)
        {
            // Given
            var context = DbContextHelper.CreateContext();
            var sut = new RemoveUserFromProcedureCommandHandler(context);
            var request = new RemoveUserFromProcedureCommand()
            {
                PlanId = planId,
                ProcedureId = procedureId,
                UserId = userId
            };

            context.PlanProcedures.Add(new PlanProcedure
            {
                PlanId = planId + 1,
                ProcedureId = procedureId + 1
            });
            await context.SaveChangesAsync();

            // When
            var result = await sut.Handle(request, new CancellationToken());

            // Then
            result.Exception.Should().BeOfType(typeof(NotFoundException));
            result.Succeeded.Should().BeFalse();
        }

        [TestMethod]
        [DataRow(1, 1, 1)]
        public async Task RemoveUserFromProcedureTests_ClearAllUsers_Success(int planId, int procedureId, int userId)
        {
            // Given
            var context = DbContextHelper.CreateContext();
            var sut = new RemoveUserFromProcedureCommandHandler(context);
            var request = new RemoveUserFromProcedureCommand()
            {
                PlanId = planId,
                ProcedureId = procedureId,
                UserId = userId,
                ClearAll = true
            };

            context.PlanProcedures.Add(new PlanProcedure
            {
                PlanId = planId,
                ProcedureId = procedureId
            });
            context.Users.Add(new User
            {
                UserId = userId
            });
            context.PlanProcedureUsers.Add(new PlanProcedureUser
            {
                PlanId = planId,
                ProcedureId = procedureId,
                UserId = userId
            });
            await context.SaveChangesAsync();

            // When
            var result = await sut.Handle(request, new CancellationToken());

            // Then
            // Then
            var dbPlanProcedureUsers = await context.PlanProcedureUsers
                .Where(pu => pu.PlanId == planId && pu.ProcedureId == procedureId)
                .ToListAsync();

            dbPlanProcedureUsers.Should().BeEmpty();

            result.Value.Should().BeOfType(typeof(Unit));
            result.Succeeded.Should().BeTrue();
        }

        [TestMethod]
        [DataRow(1, 1, -1)]
        [DataRow(1, 1, 0)]
        [DataRow(1, 1, int.MinValue)]
        public async Task RemoveUserFromProcedureTests_InvalidUserId_ReturnsBadRequest(int planId, int procedureId, int userId)
        {
            // Given
            var context = new Mock<RLContext>();
            var sut = new RemoveUserFromProcedureCommandHandler(context.Object);
            var request = new RemoveUserFromProcedureCommand()
            {
                PlanId = planId,
                ProcedureId = procedureId,
                UserId = userId
            };

            // When
            var result = await sut.Handle(request, new CancellationToken());

            // Then
            result.Exception.Should().BeOfType<BadRequestException>();
            result.Succeeded.Should().BeFalse();
        }

        [TestMethod]
        [DataRow(1, 1, 1)]
        public async Task RemoveUserFromProcedureTests_UserNotFound_ReturnsNotFound(int planId, int procedureId, int userId)
        {
            // Given
            var context = DbContextHelper.CreateContext();
            var sut = new RemoveUserFromProcedureCommandHandler(context);
            var request = new RemoveUserFromProcedureCommand()
            {
                PlanId = planId,
                ProcedureId = procedureId,
                UserId = userId
            };

            context.PlanProcedures.Add(new PlanProcedure
            {
                PlanId = planId,
                ProcedureId = procedureId
            });
            await context.SaveChangesAsync();

            // When
            var result = await sut.Handle(request, new CancellationToken());

            // Then
            result.Exception.Should().BeOfType(typeof(NotFoundException));
            result.Succeeded.Should().BeFalse();
        }

        [TestMethod]
        [DataRow(1, 1, 1)]
        [DataRow(19, 1010, 2)]
        [DataRow(35, 69, 3)]
        public async Task RemoveUserFromProcedureTests_UserNotAssociated_ReturnsSuccess(int planId, int procedureId, int userId)
        {
            // Given
            var context = DbContextHelper.CreateContext();
            var sut = new RemoveUserFromProcedureCommandHandler(context);
            var request = new RemoveUserFromProcedureCommand()
            {
                PlanId = planId,
                ProcedureId = procedureId,
                UserId = userId
            };

            context.PlanProcedures.Add(new PlanProcedure
            {
                PlanId = planId,
                ProcedureId = procedureId
            });
            context.Users.Add(new User
            {
                UserId = userId
            });
            await context.SaveChangesAsync();

            // When
            var result = await sut.Handle(request, new CancellationToken());

            // Then
            result.Value.Should().BeOfType(typeof(Unit));
            result.Succeeded.Should().BeTrue();
        }

        [TestMethod]
        [DataRow(1, 1, 1)]
        [DataRow(19, 1010, 2)]
        [DataRow(35, 69, 3)]
        public async Task RemoveUserFromProcedureTests_UserAssociated_Success(int planId, int procedureId, int userId)
        {
            // Given
            var context = DbContextHelper.CreateContext();
            var sut = new RemoveUserFromProcedureCommandHandler(context);
            var request = new RemoveUserFromProcedureCommand()
            {
                PlanId = planId,
                ProcedureId = procedureId,
                UserId = userId
            };

            context.PlanProcedures.Add(new PlanProcedure
            {
                PlanId = planId,
                ProcedureId = procedureId
            });
            context.Users.Add(new User
            {
                UserId = userId
            });
            context.PlanProcedureUsers.Add(new PlanProcedureUser
            {
                PlanId = planId,
                ProcedureId = procedureId,
                UserId = userId
            });
            await context.SaveChangesAsync();

            // When
            var result = await sut.Handle(request, new CancellationToken());

            // Then
            var dbPlanProcedureUser = await context.PlanProcedureUsers.FirstOrDefaultAsync(pu => pu.PlanId == planId && pu.ProcedureId == procedureId && pu.UserId == userId);

            dbPlanProcedureUser.Should().BeNull();

            result.Value.Should().BeOfType(typeof(Unit));
            result.Succeeded.Should().BeTrue();
        }
    }