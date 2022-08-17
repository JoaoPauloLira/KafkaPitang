using Producer;
using Microsoft.AspNetCore.Mvc;

namespace Api.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class KafkaPitangController : ControllerBase
{
    private readonly ILogger<KafkaPitangController> _logger;
    private readonly IKafkaDispatcher _dispatcher;

    public KafkaPitangController(ILogger<KafkaPitangController> logger, IKafkaDispatcher dispatcher)
    {
        _logger = logger;
        _dispatcher = dispatcher;
    }

    [HttpGet]
    public async Task<OkObjectResult> GetKafkaTeste()
    {
        for (var i = 0; i < 100; i++)
        {
            //Thread.Sleep(TimeSpan.FromSeconds(2));
            await _dispatcher.Dispatch($"{i}", "TesteJP",$"teste - {i}");
        }
        return Ok("teste");
    }
}