using AutoMapper;
using CommandService.Data.Repositories;
using CommandService.Dtos;
using CommandService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommandService.Controllers;

[ApiController]
[Route("api/c/platforms/[controller]")]
public class CommandController : ControllerBase
{
    private readonly ICommandRepo _repo;
    private readonly IMapper _mapper;

    public CommandController(ICommandRepo repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    [HttpGet]
    public ActionResult<IEnumerable<CommandReadDto>> GetAllCommandsForPlatform([FromQuery] int platformId)
    {
        IEnumerable<CommandReadDto> commands;

        Console.WriteLine($"--> Hit GetAllCommandsForPlatform: {platformId}");

        if (!_repo.IsPlatformExists(platformId))
            return NotFound();

        commands = _mapper.Map<IEnumerable<CommandReadDto>>(_repo.GetCommandsForPlatform(platformId));

        return Ok(commands);
    }

    [HttpGet]
    [Route("{commandId}")]
    public ActionResult<CommandReadDto> GetCommandForPlatform(
        [FromQuery] int platformId,
        [FromQuery] int commandId)
    {
        CommandReadDto command;

        Console.WriteLine($"--> Hit GetCommandForPlatform: {platformId} / {commandId}");

        if (!_repo.IsPlatformExists(platformId))
            return NotFound();

        command = _mapper.Map<CommandReadDto>(
            _repo.GetCommand(platformId, commandId)
        );

        if (command is null)
            return NotFound();

        return Ok(command);
    }

    [HttpPost]
    public ActionResult<CommandReadDto> Create(
        [FromQuery] int platformId,
        CommandCreateDto commandCreateDto)
    {
        CommandModel command;
        CommandReadDto commandReadDto;

        if (!_repo.IsPlatformExists(platformId)) return NotFound();

        command = _mapper
            .Map<CommandModel>(commandCreateDto);

        _repo.CreateCommand(platformId, command);
        _repo.SaveChanges();

        commandReadDto = _mapper
            .Map<CommandReadDto>(command);

        return Created(
            nameof(Create),
            commandReadDto);
    }
}