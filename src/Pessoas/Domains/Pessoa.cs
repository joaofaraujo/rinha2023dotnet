namespace Pessoas.Domains;
public class Pessoa
{
    public Pessoa(string apelido, string nome, DateTime nascimento, string[]? stack)
    {
        Apelido = apelido;
        Nome = nome;
        Nascimento = nascimento;
        Stack = stack;
    }

    public Pessoa(Guid id, string apelido, string nome, DateTime nascimento, string[]? stack) : this(apelido, nome, nascimento, stack)
    {
        this.Id = id;
    }

    public Guid Id { get; set; }
    public string Apelido { get; set; }
    public string Nome { get; set; }
    public DateTime Nascimento { get; set; }
    public string[]? Stack { get; set; }
    
    private bool ApelidoValido()
    {
        return !string.IsNullOrEmpty(this.Apelido) && this.Apelido.Length <= 32;
    }

    private bool NomeValido()
    {
        return !string.IsNullOrEmpty(this.Nome) && this.Nome.Length <= 100;
    }

    private bool StackValida()
    {
        return this.Stack?.All(x => x.Length <= 32) ?? true;
    }

    public bool PessoaValida()
    {
        return this.ApelidoValido() && this.NomeValido() && this.StackValida();
    }
}
