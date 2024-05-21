using Pessoas.Domains;
using Pessoas.Repositories;
using rinhadotnet.Controllers;

namespace Pessoas.Services;

public class PessoaService
{
    private readonly PessoaRepository _pessoaRepository;

    public PessoaService(PessoaRepository pessoaRepository)
    {
        this._pessoaRepository = pessoaRepository;
    }
    public async Task<(Pessoa, bool)> InserirPessoaAsync(Pessoa pessoa, CancellationToken cancellationToken)
    {
        if (pessoa == null || !pessoa.PessoaValida())
            return (null, false);

        var apelidoJaCadastrado = await _pessoaRepository.VerificarApelidoCadastradoAsync(pessoa.Apelido, cancellationToken);
        if (apelidoJaCadastrado)
            return (null, false);

        pessoa.Id = Guid.NewGuid();
        await _pessoaRepository.InserirPessoaAsync(pessoa, cancellationToken);
        return (pessoa, true);
    }
}
