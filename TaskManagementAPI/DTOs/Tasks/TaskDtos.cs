using Microsoft.AspNetCore.Antiforgery;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using TaskManagementAPI.Models;

namespace TaskManagementAPI.DTOs.Tasks;

public record CreateTaskRequestDto(
    [Required, MaxLength(100)] string Title,
    [MaxLength(1000)] string Description,
    TaskPriority Priority,
    DateTime? DueDate,
    [Required] Guid ProjectId,
    Guid? AssignedToUserId);

public record UpdateTaskRequestDto(
    //[Required] Guid TaskId,
    [Required, MaxLength(100)] string Title,
    [MaxLength(1000)] string Description,
    TaskPriority Priority,
    DateTime? DueDate,
    Guid? AssignedToUserId );

public record UpdateTaskStatusRequestDto(
    [Required] TaskItemStatus Status);

public record TaskQueryParametersDto(
    Guid? ProjectId,
    TaskItemStatus? Status,
    TaskPriority? Priority,
    Guid? AssignedToUserId,
    string? SearchItem);

public record TaskResponseDto(
    Guid Id,
    string Title,
    string Description,
    string Status,
    string Priority,
    DateTime? DueDate,
    Guid ProjectId,
    string ProjectName,
    Guid? AssignedtoUserId,
    string? AssignedToUser,
    Guid CreatedByUserId,
    DateTime CreatedAt,
    DateTime? UpdatedAt);