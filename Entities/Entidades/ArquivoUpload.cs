namespace Entities;

public class ArquivoUpload
{
    public ArquivoUpload(
        int id,
        byte[] conteudo)
    {
        Id = id;
        Conteudo = conteudo;
    }
    public int Id { get; }
    public byte[] Conteudo { get; }
}