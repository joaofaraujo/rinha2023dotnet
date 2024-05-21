using Npgsql;
using Pessoas.Domains;

namespace Pessoas.Repositories;
public class PessoaRepository
{
    private readonly NpgsqlConnection _connection;
    private readonly ILogger<PessoaRepository> _logger;
    public PessoaRepository(NpgsqlConnection connection, ILogger<PessoaRepository> logger)
    {
        this._connection = connection;
        _logger = logger;
    }

    public async Task InserirPessoaAsync(Pessoa pessoa, CancellationToken cancellation)
    {
        try
        {
            _connection.Open();

            string query = """
                        insert into pessoas
                        (id, apelido, nome, nascimento, stack)
                        values ($1, $2, $3, $4, $5);
                    """;

            using (var command = new NpgsqlCommand(query, _connection))
            {
                command.Parameters.AddWithValue(pessoa.Id);
                command.Parameters.AddWithValue(pessoa.Apelido);
                command.Parameters.AddWithValue(pessoa.Nome);
                command.Parameters.AddWithValue(pessoa.Nascimento);
                command.Parameters.AddWithValue(pessoa.Stack == null ? DBNull.Value : string.Join(",", pessoa.Stack));

                await command.ExecuteNonQueryAsync(cancellation);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "erro na inserção :)");
            throw;
        }
        finally
        {
            if(_connection.State != System.Data.ConnectionState.Closed)
                _connection.Close();
        }
    }

    public async Task<Pessoa?> ConsultarPessoaPorIdAsync(Guid id, CancellationToken cancellationToken)
    {
        Pessoa? pessoa = null;
        try
        {
            _connection.Open();

            string query = """
                        select id, apelido, nome, nascimento, stack 
                        from pessoas
                        where id = $1;
                    """;

            using (var command = new NpgsqlCommand(query, _connection))
            {
                command.Parameters.AddWithValue(id);

                using (var reader = await command.ExecuteReaderAsync(cancellationToken))
                {
                    while (reader.Read())
                    {
                        var stack = reader.IsDBNull(4) ? null : reader.GetString(4);
                        pessoa = new Pessoa(reader.GetGuid(0), reader.GetString(1), reader.GetString(2),
                            reader.GetDateTime(3), stack?.Split(',').ToArray());
                    }
                }

            }

            return pessoa;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "erro na inserção :)");
            throw;
        }
        finally
        {
            if (_connection.State != System.Data.ConnectionState.Closed)
                _connection.Close();
        }
    }

    public async Task<bool> VerificarApelidoCadastradoAsync(string apelido, CancellationToken cancellationToken)
    {
        bool cadastrado = false;
        try
        {
            _connection.Open();
            string query = """
                        select id 
                        from pessoas
                        where apelido = $1;
                    """;
            using (var command = new NpgsqlCommand(query, _connection))
            {
                command.Parameters.AddWithValue(apelido);

                using (var reader = await command.ExecuteReaderAsync(cancellationToken))
                {
                    while (reader.Read())
                    {
                        cadastrado = true;
                    }
                }
            }

            return cadastrado;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "erro na inserção :)");
            throw;
        }
        finally
        {
            if (_connection.State != System.Data.ConnectionState.Closed)
                _connection.Close();
        }
    }

    public async Task<int> ConsultarTotalPessoaAsync(CancellationToken cancellationToken)
    {
        int totalRegistros = 0;
        try
        {
            _connection.Open();
            string query = """
                        select COUNT(id) 
                        from pessoas
                    """;

            using (var command = new NpgsqlCommand(query, _connection))
            {
                using (var reader = await command.ExecuteReaderAsync(cancellationToken))
                {
                    while (reader.Read())
                    {
                        totalRegistros = reader.GetInt32(0);
                    }
                }
            }

            return totalRegistros;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "erro na inserção :)");
            throw;
        }
        finally
        {
            if (_connection.State != System.Data.ConnectionState.Closed)
                _connection.Close();
        }
    }

    public async Task<List<Pessoa>> ListarPessoasPorTermoAsync(string termo, CancellationToken cancellationToken)
    {
        List<Pessoa> pessoas = new List<Pessoa>();
        try
        {
            _connection.Open();

            string query = """
                        select id, apelido, nome, nascimento, stack 
                        from pessoas
                        where (apelido LIKE @termo OR nome LIKE @termo OR stack LIKE @termo)
                        limit 50;
                    """;
            using (var command = new NpgsqlCommand(query, _connection))
            {
                command.Parameters.AddWithValue("termo", $"%{termo}%");

                using (var reader = await command.ExecuteReaderAsync(cancellationToken))
                {
                    while (reader.Read())
                    {
                        var stack = reader.IsDBNull(4) ? null : reader.GetString(4);
                        var pessoa = new Pessoa(reader.GetGuid(0), reader.GetString(1), reader.GetString(2),
                            reader.GetDateTime(3), stack?.Split(',').ToArray());

                        pessoas.Add(pessoa);
                    }
                }
            }

            return pessoas;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "erro na inserção :)");
            throw;
        }
        finally
        {
            if (_connection.State != System.Data.ConnectionState.Closed)
                _connection.Close();
        }
    }
}