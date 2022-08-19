namespace Entities;

public class Usuario
{
    public Usuario(int id, string nome, int idade, Endereco endereco)
    {
        Id = id;
        Nome = nome;
        Idade = idade;
        Endereco = endereco;
    }

    public int Id { get; }
    public string Nome { get; }
    public int Idade { get; }
    public Endereco Endereco { get; }
}

public class Endereco
{
    public Endereco(string rua, string bairro, int numero)
    {
        Rua = rua;
        Bairro = bairro;
        Numero = numero;
    }
    
    public string Rua { get; }
    public string Bairro { get; }
    public int Numero { get; }
}