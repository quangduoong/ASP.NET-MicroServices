using AutoMapper;
using CommandService.Models;
using Grpc.Net.Client;
using PlatformService;

namespace CommandService.SyncDataServices.Grpc;

public class PlatformDataClient : IPlatformDataClient
{
    private readonly IConfiguration _config;
    private readonly IMapper _mapper;

    public PlatformDataClient(IConfiguration config, IMapper mapper)
    {
        _config = config;
        _mapper = mapper;
    }

    public IEnumerable<PlatformModel> ReturnAllPlatforms()
    {
        var address = _config["GrpcPlatform"] ?? default!;
        GrpcChannel channel;
        GrpcPlatform.GrpcPlatformClient client;
        GetAllRequest request;

        Console.WriteLine($"--> Calling GRPC Service {address}");

        channel = GrpcChannel.ForAddress(address);
        client = new(channel);
        request = new();

        try
        {
            var reply = client.GetAllPlatforms(request);
            return _mapper.Map<IEnumerable<PlatformModel>>(reply.Platform);
        }
        catch (System.Exception ex)
        {
            Console.WriteLine($"--> Could not call GRPC Server.");
            throw ex;
        }
    }
}