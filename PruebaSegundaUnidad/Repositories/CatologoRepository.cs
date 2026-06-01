using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using PruebaSegundaUnidad.Models;

namespace PruebaSegundaUnidad.Repositories
{
    /// Repositorio exclusivo para consultar las tablas maestras de solo lectura.
    /// Utilizado para poblar los elementos genéricos en las vistas Razor y formularios
    /// (como las listas desplegables <select>).
    public class CatalogoRepository
    {
        // Se obtiene la cadena de conexión definida en el archivo Web.config
        private readonly string conexion = ConfigurationManager.ConnectionStrings["ConexionSQL"].ConnectionString;

        /// Obtiene la lista completa de roles disponibles en el sistema.
        /// Útil para el panel de administración al momento de crear o editar un usuario.
        /// <returns>Lista de objetos Rol.</returns>
        public List<Rol> ObtenerRoles()
        {
            List<Rol> lista = new List<Rol>();
            using (SqlConnection con = new SqlConnection(conexion))
            {
                SqlCommand cmd = new SqlCommand("SELECT Id, NombreRol FROM Roles", con);
                con.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    lista.Add(new Rol
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        NombreRol = reader["NombreRol"].ToString()
                    });
                }
            }
            return lista;
        }

        /// Recupera todos los departamentos o áreas de la institución.
        /// Necesario para el formulario de registro de nuevas solicitudes de soporte.
        /// <returns>Lista de objetos AreaSolicitante.</returns>
        public List<AreaSolicitante> ObtenerAreas()
        {
            List<AreaSolicitante> lista = new List<AreaSolicitante>();
            using (SqlConnection con = new SqlConnection(conexion))
            {
                SqlCommand cmd = new SqlCommand("SELECT Id, NombreArea FROM AreasSolicitantes", con);
                con.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    lista.Add(new AreaSolicitante
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        NombreArea = reader["NombreArea"].ToString()
                    });
                }
            }
            return lista;
        }

        /// Extrae el catálogo de problemas predefinidos que los usuarios pueden reportar.
        /// Se utiliza en la vista de creación de requerimientos (Fase 4).
        /// <returns>Lista de objetos TipoProblema.</returns>
        public List<TipoProblema> ObtenerTiposProblema()
        {
            List<TipoProblema> lista = new List<TipoProblema>();
            using (SqlConnection con = new SqlConnection(conexion))
            {
                SqlCommand cmd = new SqlCommand("SELECT Id, NombreTipoProblema FROM TiposProblema", con);
                con.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    lista.Add(new TipoProblema
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        NombreTipoProblema = reader["NombreTipoProblema"].ToString()
                    });
                }
            }
            return lista;
        }
    }
}