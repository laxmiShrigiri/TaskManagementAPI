using FluentAssertions;
using TaskManagementAPI.Exceptions;
using TaskManagementAPI.Models;
using Xunit;

namespace TaskManagementAPI.Tests;

public class TaskManagementTests
{
    [Fact]
    public void NewTaskItem_ShouldDefaultStatusToTodo()
    {
        // Arrange & Act
        var task = new TaskItem { Title = "Configure CI/CD" };

        // Assert
        task.Status.Should().Be(TaskItemStatus.Todo);
        task.CreatedAt.Should().BeBefore(DateTime.UtcNow.AddSeconds(1));
    }

    [Fact]
    public void TaskItem_ShouldInitializeEmptyCommentList()
    {
        // Arrange & Act
        var task = new TaskItem();

        // Assert
        task.Comments.Should().NotBeNull();
        task.Comments.Should().BeEmpty();
    }

    [Fact]
    public void NotFoundException_ShouldSetCorrectStatusCodeAndMessage()
    {
        // Arrange & Act
        var ex = new NotFoundException("Task not found");

        // Assert
        ex.Message.Should().Be("Task not found");
        ((int)ex.StatusCode).Should().Be(404);
    }
}