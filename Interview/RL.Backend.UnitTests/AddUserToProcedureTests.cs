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
    public class AddUserToProcedureTests
    {
        [TestMethod]
        [DataRow(-1, 1, 1)]
        [DataRow(0, 1, 1)]
        [DataRow(int.MinValue, 1, 1)]
        public async Task AddUserToProcedureTests_InvalidPlanId_ReturnsBadRequest(int planId, int procedureId, int userId)
        {
            //Given
            var context = new Mock<RLContext>();
            var sut = new AddUserToProcedureCommandHandler(context.Object);
            var request = new AddUserToProcedureCommand()
            {
                PlanId = planId,
                ProcedureId = procedureId,
                UserId = userId
            };

            //When
            var result = await sut.Handle(request, new CancellationToken());

            //Then
            result.Exception.Should().BeOfType(typeof(BadRequestException));
            result.Succeeded.Should().BeFalse();
        }

        [TestMethod]
        [DataRow(1, -1, 1)]
        [DataRow(1, 0, 1)]
        [DataRow(1, int.MinValue, 1)]
        public async Task AddUserToProcedureTests_InvalidProcedureId_ReturnsBadRequest(int planId, int procedureId, int userId)
        {
            //Given
            var context = new Mock<RLContext>();
            var sut = new AddUserToProcedureCommandHandler(context.Object);
            var request = new AddUserToProcedureCommand()
            {
                PlanId = planId,
                ProcedureId = procedureId,
                UserId = userId
            };

            //When
            var result = await sut.Handle(request, new CancellationToken());

            //Then
            result.Exception.Should().BeOfType(typeof(BadRequestException));
            result.Succeeded.Should().BeFalse();
        }

        [TestMethod]
        [DataRow(1, 1, -1)]
        [DataRow(1, 1, 0)]
        [DataRow(1, 1, int.MinValue)]
        public async Task AddUserToProcedureTests_InvalidUserId_ReturnsBadRequest(int planId, int procedureId, int userId)
        {
            //Given
            var context = new Mock<RLContext>();
            var sut = new AddUserToProcedureCommandHandler(context.Object);
            var request = new AddUserToProcedureCommand()
            {
                PlanId = planId,
                ProcedureId = procedureId,
                UserId = userId
            };

            //When
            var result = await sut.Handle(request, new CancellationToken());

            //Then
            result.Exception.Should().BeOfType(typeof(BadRequestException));
            result.Succeeded.Should().BeFalse();
        }

        [TestMethod]
        [DataRow(1, 1, 1)]
        public async Task AddUserToProcedureTests_PlanProcedureNotFound_ReturnsNotFound(int planId, int procedureId, int userId)
        {
            //Given
            var context = DbContextHelper.CreateContext();
            var sut = new AddUserToProcedureCommandHandler(context);
            var request = new AddUserToProcedureCommand()
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

            //When
            var result = await sut.Handle(request, new CancellationToken());

            //Then
            result.Exception.Should().BeOfType(typeof(NotFoundException));
            result.Succeeded.Should().BeFalse();
        }

        [TestMethod]
        [DataRow(1, 1, 1)]
        public async Task AddUserToProcedureTests_UserIdNotFound_ReturnsNotFound(int planId, int procedureId, int userId)
        {
            //Given
            var context = DbContextHelper.CreateContext();
            var sut = new AddUserToProcedureCommandHandler(context);
            var request = new AddUserToProcedureCommand()
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

            //When
            var result = await sut.Handle(request, new CancellationToken());

            //Then
            result.Exception.Should().BeOfType(typeof(NotFoundException));
            result.Succeeded.Should().BeFalse();
        }

        [TestMethod]
        [DataRow(1, 1, 1)]
        [DataRow(19, 1010, 2)]
        [DataRow(35, 69, 3)]
        public async Task AddUserToProcedureTests_AlreadyContainsUser_ReturnsSuccess(int planId, int procedureId, int userId)
        {
            //Given
            var context = DbContextHelper.CreateContext();
            var sut = new AddUserToProcedureCommandHandler(context);
            var request = new AddUserToProcedureCommand()
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

            //When
            var result = await sut.Handle(request, new CancellationToken());

            //Then
            result.Value.Should().BeOfType(typeof(Unit));
            result.Succeeded.Should().BeTrue();
        }

        [TestMethod]
        [DataRow(1, 1, 1)]
        [DataRow(19, 1010, 2)]
        [DataRow(35, 69, 3)]
        public async Task AddUserToProcedureTests_DoesntContainsUser_ReturnsSuccess(int planId, int procedureId, int userId)
        {
            //Given
            var context = DbContextHelper.CreateContext();
            var sut = new AddUserToProcedureCommandHandler(context);
            var request = new AddUserToProcedureCommand()
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

            //When
            var result = await sut.Handle(request, new CancellationToken());

            //Then
            var dbPlanProcedureUser = await context.PlanProcedureUsers.FirstOrDefaultAsync(pu => pu.PlanId == planId && pu.ProcedureId == procedureId && pu.UserId == userId);

            dbPlanProcedureUser.Should().NotBeNull();

            result.Value.Should().BeOfType(typeof(Unit));
            result.Succeeded.Should().BeTrue();
        }
    }