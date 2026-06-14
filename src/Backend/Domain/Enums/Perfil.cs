namespace ControleDisciplinas.Domain.Enums;

/// <summary>Perfis de acesso do sistema (PDF §5 — FrmLogin).</summary>
public enum Perfil
{
    /// <summary>Gerencia os usuários.</summary>
    Administrador = 1,

    /// <summary>Cria/edita o álbum e as figurinhas.</summary>
    Autor = 2,

    /// <summary>Visualiza o álbum e adquire figurinhas.</summary>
    Colecionador = 3,
}
