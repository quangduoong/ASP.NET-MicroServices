using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.AsyncDataServices;
using PlatformService.Dtos;
using PlatformService.Interfaces;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;

namespace PlatformService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlatformController : ControllerBase
{
    private readonly IPlatformRepo _repo;
    private readonly IMapper _mapper;
    private readonly ICommandDataClient _commandDataClient;
    private readonly IMessageBusClient _messageBusClient;

    public PlatformController(
        IPlatformRepo repo,
        IMapper mapper,
        ICommandDataClient commandDataClient,
        IMessageBusClient messageBusClient)
    {
        _repo = repo;
        _mapper = mapper;
        _commandDataClient = commandDataClient;
        _messageBusClient = messageBusClient;
    }

    [HttpGet]
    public ActionResult<IEnumerable<PlatformReadDto>> GetAll()
    {
        IEnumerable<PlatformModel> items;

        Console.WriteLine("--> Getting All Platforms...");

        items = _repo.GetAll();

        if (items is null || items.Count() == 0) return StatusCode(204);

        return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(items));
    }

    [HttpGet]
    [Route("{id}", Name = "GetById")]
    public ActionResult<PlatformReadDto> GetById(int id)
    {
        var item = _repo.GetById(id);

        if (item is null) return NotFound();

        return Ok(_mapper.Map<PlatformReadDto>(item));
    }

    [HttpPost]
    public async Task<ActionResult<PlatformReadDto>> Create(PlatformCreateDto platformCreateDto)
    {
        var platformModel = _mapper.Map<PlatformModel>(platformCreateDto);
        PlatformReadDto platformReadDto;

        _repo.Create(platformModel);
        _repo.SaveChanges();
        platformReadDto = _mapper.Map<PlatformReadDto>(platformModel);
        // * Send Sync Message
        try
        {
            await _commandDataClient.SendPlatformToCommand(platformReadDto);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> Cannot send command synchronously: {ex}");
        }
        // * Send Async Message
        try
        {
            var platformPublishDto = _mapper.Map<PlatformPublishDto>(platformReadDto);

            platformPublishDto.Event = "Platform_Publish";
            _messageBusClient.PublishNewPlatform(platformPublishDto);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> Send async message failed: {ex.Message}");
        }

        return Ok(platformReadDto);
    }
}