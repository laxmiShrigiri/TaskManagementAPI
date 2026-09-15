using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Data;
using TaskManagementAPI.DTOs.Projects;
using TaskManagementAPI.Exceptions;
using TaskManagementAPI.Models;
using TaskManagementAPI.Services;
using Xunit;

namespace TaskManagementAPI.Tests;

public class ProjectServiceTests
{
    private AppDbContext CreateInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task AddMember_WhenUserIsAlreadyMember_ShouldThrowConflictException()
    {
        // Arrange
        using var db = CreateInMemoryDbContext();
        var pmId = Guid.NewGuid();
        var existingMemberId = Guid.NewGuid();
        var projectId = Guid.NewGuid();

        // Seed project with owner/PM
        var project = new Project
        {
            Id = projectId,
            Name = "Migration Project",
            Description = "Backend migration",
            CreatedByUserId = pmId
        };
        db.Projects.Add(project);

        // Seed existing member
        db.ProjectMembers.Add(new ProjectMember
        {
            ProjectId = projectId,
            userId = existingMemberId
        });

        // Seed target user
        db.Users.Add(new User
        {
            Id = existingMemberId,
            Name = "John Developer",
            Email = "john@example.com",
            PasswordHash = "hash123",
            Role = UserRole.Member
        });

        await db.SaveChangesAsync();

        var projectService = new ProjectService(db);
        var addMemberDto = new AddMemberRequestDto(projectId, existingMemberId);

        // Act & Assert
        var act = async () => await projectService.AddMember(pmId, nameof(UserRole.ProjectManager), addMemberDto);

        await act.Should().ThrowAsync<ConflictException>();
    }

    [Fact]
    public async Task DeleteProject_WhenUserIsNotAdminOrCreator_ShouldThrowUnauthorizedOrForbidden()
    {
        // Arrange
        using var db = CreateInMemoryDbContext();
        var ownerId = Guid.NewGuid();
        var nonAuthorizedMemberId = Guid.NewGuid();
        var projectId = Guid.NewGuid();

        db.Projects.Add(new Project
        {
            Id = projectId,
            Name = "Confidential System",
            Description = "Internal operations",
            CreatedByUserId = ownerId
        });
        await db.SaveChangesAsync();

        var projectService = new ProjectService(db);

        // Act & Assert
        var act = async () => await projectService.DeleteProject(projectId, nonAuthorizedMemberId, nameof(UserRole.Member));

        // Assert specifically for UnauthorizedAccessException
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("You do not have permission to modify this project.");
    }

    [Fact]
    public async Task CreateProject_ShouldCreateProjectAndAddCreatorAsMember()
    {
        // Arrange
        using var db = CreateInMemoryDbContext();

        var userId = Guid.NewGuid();

        db.Users.Add(new User
        {
            Id = userId,
            Name = "Project Manager",
            Email = "pm@example.com",
            PasswordHash = "hash123",
            Role = UserRole.ProjectManager
        });

        await db.SaveChangesAsync();

        var projectService = new ProjectService(db);

        var dto = new CreateProjectDto(
            "Task Management Project",
            "Backend API project"
        );

        // Act
        var result = await projectService.CreateProject(userId, dto);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Task Management Project");
        result.Description.Should().Be("Backend API project");
        result.CreatedByUserId.Should().Be(userId);

        var project = await db.Projects
            .Include(p => p.Members)
            .FirstOrDefaultAsync(p => p.Id == result.Id);

        project.Should().NotBeNull();
        project!.Members.Should().ContainSingle();

        var member = project.Members.First();

        member.userId.Should().Be(userId);
        member.ProjectId.Should().Be(project.Id);
    }
}