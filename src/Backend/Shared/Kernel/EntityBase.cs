namespace AlbumFigurinhas.Shared.Kernel;

/// <summary>Base para entidades com identidade inteira.</summary>
public abstract class EntityBase
{
    public int Id { get; protected set; }
}
