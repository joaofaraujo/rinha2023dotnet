using Microsoft.AspNetCore.Mvc;
using Pessoas.Repositories;
using Pessoas.Services;
using rinhadotnet.ViewModel;

namespace rinhadotnet.Controllers;

[ApiController]
[Route("pessoas")]
public class PessoaController : ControllerBase
{
    private readonly ILogger<PessoaController> _logger;
    private readonly PessoaService _pessoaService;
    private readonly PessoaRepository _pessoaRepository;

    public PessoaController(ILogger<PessoaController> logger, PessoaService pessoaService, PessoaRepository pessoaRepository)
    {
        _logger = logger;
        this._pessoaService = pessoaService;
        this._pessoaRepository = pessoaRepository;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PessoaViewModel>> GetPessoaPorId(Guid id, CancellationToken cancellationToken)
    {
        var pessoa = await _pessoaRepository.ConsultarPessoaPorIdAsync(id, cancellationToken);
        if (pessoa == null)
            return NotFound();

        var pessoaDTO = new PessoaViewModel(pessoa);

        return Ok(pessoaDTO);
    }

    [HttpPost]
    public async Task<IActionResult> Post(PessoaViewModel pessoaDTO, CancellationToken cancellationToken)
    {
        if (pessoaDTO?.ValidaCamposNulos() ?? true)
            return StatusCode(422);

        var pessoa = pessoaDTO.ConverterDominio();

        var retorno = await _pessoaService.InserirPessoaAsync(pessoa, cancellationToken);

        if (!retorno.Item2)
            return StatusCode(422);

        pessoaDTO.Id = retorno.Item1.Id;

        return CreatedAtAction(nameof(GetPessoaPorId), new { id = pessoaDTO.Id }, pessoaDTO);
    }


    [HttpGet]
    public async Task<ActionResult<List<PessoaViewModel>>> GetPessoasPorTermo(string t, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(t) || string.IsNullOrWhiteSpace(t))
            return BadRequest();

        var pessoas = await _pessoaRepository.ListarPessoasPorTermoAsync(t, cancellationToken);

        var retorno = pessoas.Select(x => new PessoaViewModel(x)).ToList();

        return Ok(retorno);
    }
}

[ApiController]
[Route("contagem-pessoas")]
public class ContagemPessoaController : ControllerBase
{
    private readonly PessoaRepository _pessoaRepository;

    public ContagemPessoaController(PessoaRepository pessoaRepository)
    {
        _pessoaRepository = pessoaRepository;
    }

    [HttpGet]
    public async Task<ActionResult<int>> GetContagemPessoas(CancellationToken cancellationToken)
    {
        return Ok(await _pessoaRepository.ConsultarTotalPessoaAsync(cancellationToken));
    }
}