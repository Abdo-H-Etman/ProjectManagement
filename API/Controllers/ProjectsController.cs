using System;
using Application.Common.Models.Interfaces;
using Application.DTOs.Project;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;
    public ProjectsController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAllProjects(CancellationToken cancellationToken)
    {
        var result = await _projectService.GetAllProjectsAsync(cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("{projectId:guid}")]
    [Authorize]
    public async Task<IActionResult> GetProjectById(Guid projectId, CancellationToken cancellationToken)
    {
        var result = await _projectService.GetProjectByIdAsync(projectId, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("user/{userId:guid}")]
    [Authorize]
    public async Task<IActionResult> GetUserProjects(Guid userId, CancellationToken cancellationToken)
    {
        var result = await _projectService.GetUserProjectsAsync(userId, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("archived/{userId:guid}")]
    [Authorize]
    public async Task<IActionResult> GetArchivedProjects(Guid userId, CancellationToken cancellationToken)
    {
        var result = await _projectService.GetArchivedProjectsAsync(userId, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("details/{projectId:guid}")]
    [Authorize]
    public async Task<IActionResult> GetProjectDetailsById(Guid projectId, CancellationToken cancellationToken)
    {
        var result = await _projectService.GetProjectDetailsByIdAsync(projectId, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("statistics/{projectId:guid}")]
    [Authorize]
    public async Task<IActionResult> GetProjectStatistics(Guid projectId, CancellationToken cancellationToken)
    {
        var result = await _projectService.GetProjectStatisticsAsync(projectId, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateProject([FromBody] CreateProjectDto createProjectDto, CancellationToken cancellationToken)
    {
        var result = await _projectService.CreateProjectAsync(createProjectDto, cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(result);
        return Ok(result);
    }

    [HttpPatch("{projectId:guid}")]
    [Authorize]
    public async Task<IActionResult> UpdateProject(
        Guid projectId,
        [FromBody] UpdateProjectDto updateProjectDto,
        CancellationToken cancellationToken)
    {
        var result = await _projectService.UpdateProjectAsync(
            projectId,
            updateProjectDto,
            cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpDelete("{projectId:guid}")]
    [Authorize]
    public async Task<IActionResult> DeleteProject(Guid projectId, CancellationToken cancellationToken)
    {
        var result = await _projectService.DeleteProjectAsync(projectId, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("search")]
    [Authorize]
    public async Task<IActionResult> SearchProjects([FromQuery] string searchTerm, CancellationToken cancellationToken)
    {
        var result = await _projectService.SearchProjectsAsync(searchTerm, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("{projectId:guid}/invite")]
    [Authorize]
    public async Task<IActionResult> SendProjectInvitation(
        Guid projectId,
        [FromBody] InvitationDto invitationDto,
        CancellationToken cancellationToken)
    {
        var result = await _projectService.SendProjectInvitationAsync(
            projectId,
            invitationDto,
            cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("invitations/accept")]
    [Authorize]
    public async Task<IActionResult> AcceptProjectInvitation(
        [FromQuery] string token,
        [FromQuery] Guid inviterId,
        CancellationToken cancellationToken)
    {
        var result = await _projectService.AcceptProjectInvitationAsync(
            token,
            inviterId,
            cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("{projectId:guid}/archive")]
    [Authorize]
    public async Task<IActionResult> ArchiveProject(Guid projectId, CancellationToken cancellationToken)
    {
        var result = await _projectService.ArchiveProjectAsync(projectId, cancellationToken);

        if( !result.IsSuccess)
            return BadRequest(result);

        return Ok(result);    
    }

    [HttpPost("archive")]
    [Authorize]
    public async Task<IActionResult> ArchiveMultipleProjects(
        [FromBody] List<Guid> projectIds,
        CancellationToken cancellationToken)
    {
        var result = await _projectService.ArchiveMultipleProjectsAsync(projectIds, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("{projectId:guid}/restore")]
    [Authorize]
    public async Task<IActionResult> RestoreProject(Guid projectId, CancellationToken cancellationToken)
    {
        var result = await _projectService.RestoreProjectAsync(projectId, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }
}