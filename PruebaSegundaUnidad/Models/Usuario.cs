using System;

namespace PruebaSegundaUnidad.Models
{
    // Datos principales de un usuario del sistema.
    // Se usa en login, sesión, rol y después en perfil.
    public class Usuario
    {
        public int Id { get; set; }

        public int RolId { get; set; }

        public string NombreRol { get; set; }

        public string NombreCompleto { get; set; }

        public string NombreUsuario { get; set; }

        public string Correo { get; set; }

        public string ClaveHash { get; set; }

        public bool Estado { get; set; }

        public DateTime FechaRegistro { get; set; }
    }
}