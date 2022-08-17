namespace Producer;

public class CorrelationId
{
    public CorrelationId()
    {
        Id = Guid.NewGuid().ToString();
    }

    public CorrelationId(string correlationId)
    {
        Id = correlationId;
    }

    public string Id { get; }

    public static implicit operator CorrelationId(string id)
    {
        return new CorrelationId(id);
    }

    public static implicit operator string(CorrelationId id) => id.Id;

    public override string ToString()
    {
        return Id.ToString();
    }
}