using ControleDisciplinas.Shared.Kernel;

namespace ControleDisciplinas.Domain.Entities;

/// <summary>
/// Figurinha adquirida por um colecionador (PDF §12 — FrmNovaFigurinha).
/// Liga o usuário colecionador à figurinha do catálogo do álbum.
/// </summary>
public class FigurinhaAdquirida : EntityBase
{
    public int UsuarioId { get; private set; }
    public int FigurinhaId { get; private set; }
    public DateTime AdquiridaEmUtc { get; private set; }

    public Usuario? Usuario { get; private set; }
    public Figurinha? Figurinha { get; private set; }

    private FigurinhaAdquirida() { } // EF Core

    public FigurinhaAdquirida(int usuarioId, int figurinhaId)
    {
        UsuarioId = usuarioId;
        FigurinhaId = figurinhaId;
        AdquiridaEmUtc = DateTime.UtcNow;
    }
}
