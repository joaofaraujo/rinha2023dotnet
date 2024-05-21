using Pessoas.Domains;
using System.Text.Json.Serialization;

namespace rinhadotnet.ViewModel;

public class PessoaViewModel
{
    public PessoaViewModel()
    {
    }

    public PessoaViewModel(Pessoa pessoa)
    {
        Id = pessoa.Id;
        Apelido = pessoa.Apelido;
        Nome = pessoa.Nome;
        Nascimento = pessoa.Nascimento;
        Stack = pessoa.Stack; ;
    }

    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("apelido")]
    public string Apelido { get; set; }

    [JsonPropertyName("nome")]
    public string Nome { get; set; }

    [JsonPropertyName("nascimento")]
    public DateTime? Nascimento { get; set; }

    [JsonPropertyName("stack")]
    public string[]? Stack { get; set; }

    [JsonIgnore]
    public string? Pesquisa { get; set; }

    public Pessoa ConverterDominio()
    {
        var pessoa = new Pessoa(this.Apelido, this.Nome, this.Nascimento.Value, this.Stack);
        return pessoa;
    }

    public bool ValidaCamposNulos()
    {
        return string.IsNullOrEmpty(this.Nome) || string.IsNullOrEmpty(this.Apelido) || this.Nascimento == null;
    }
}