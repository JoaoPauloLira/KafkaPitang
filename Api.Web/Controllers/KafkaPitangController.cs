using Api.Web.Extensions;
using Entities;
using Entities.Enum;
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
    
    [HttpGet("GetKafkaTeste")]
    public async Task<OkObjectResult> GetKafkaTeste()
    {
        for (var i = 0; i < 100; i++)
        {
            await _dispatcher.Dispatch($"{i}", TopicosKafka.LOJA_NOVO_PEDIDO.ToString(),$"teste - {i}");
        }
        return Ok("Teste Kafka - String");
    }

    [HttpGet("GetKafkaTesteString")]
    public async Task<OkObjectResult> GetKafkaTesteString()
    {
        for (var i = 0; i < 100; i++)
        {
            await _dispatcher.Dispatch($"{i}", TopicosKafka.PITANG_STRING.ToString(),$"testeString - {i}");
        }
        return Ok("Teste Kafka - String");
    }
    
    [HttpGet("GetKafkaTesteUsuario")]
    public async Task<OkObjectResult> GetKafkaTesteUsuario()
    {
        for (var i = 0; i < 10; i++)
        {
            var usuario = new Usuario(i + 1, $"Usuario Teste-{i}", 55, new Endereco("Rua teste", "Bairro teste", i));
            
            await _dispatcher.Dispatch($"{i}", TopicosKafka.PITANG_USUARIO.ToString(), usuario);
        }
        return Ok("Teste Kafka - Usuário");
    }
    
    [HttpPatch("GetKafkaTesteArquivo")]
    public async Task<OkObjectResult> GetKafkaTesteArquivo()
    {
        var arquivoUpload = await ArquivoUpload();

        await _dispatcher.Dispatch(arquivoUpload.Id.ToString(), TopicosKafka.PITANG_ARQUIVO.ToString(), arquivoUpload);
        
        return Ok("Teste Kafka - Arquivo Bits");
    }

    private async Task<ArquivoUpload> ArquivoUpload()
    {
        var stream = Request.Body;
        var conteudo = await stream.ToByteArrayAsync();

        //var id = 145;
        var id = Random.Shared.Next(144);

        var arquivoUpload = new ArquivoUpload(id, conteudo);
        return arquivoUpload;
    }
}