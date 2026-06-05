using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using PruebaSegundaUnidad.Models;

namespace PruebaSegundaUnidad.Repositories
{
    // Repositorio encargado de consultar tablas maestras o catálogos.
    // Estos datos se usan para llenar listas desplegables en formularios.
    public class CatalogoRepository
    {
        // Cadena de conexión definida en Web.config.
        private readonly string conexion = ConfigurationManager.ConnectionStrings["ConexionSQL"].ConnectionString;

        #region Roles

        // Obtiene todos los roles disponibles en el sistema.
        public List<Rol> ObtenerRoles()
        {
            List<Rol> lista = new List<Rol>();

            using (SqlConnection con = new SqlConnection(conexion))
            {
                string query = "SELECT Id, NombreRol FROM Roles";

                SqlCommand cmd = new SqlCommand(query, con);

                con.Open();

                // ExecuteReader se usa porque esta consulta devuelve varias filas.
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(new Rol
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            NombreRol = reader["NombreRol"].ToString()
                        });
                    }
                }
            }

            return lista;
        }

        #endregion

        #region Áreas solicitantes

        // Obtiene las áreas disponibles para registrar solicitudes de soporte.
        public List<AreaSolicitante> ObtenerAreas()
        {
            List<AreaSolicitante> lista = new List<AreaSolicitante>();

            using (SqlConnection con = new SqlConnection(conexion))
            {
                string query = "SELECT Id, NombreArea FROM AreasSolicitantes";

                SqlCommand cmd = new SqlCommand(query, con);

                con.Open();

                // Se leen todas las áreas devueltas por la consulta.
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(new AreaSolicitante
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            NombreArea = reader["NombreArea"].ToString()
                        });
                    }
                }
            }

            return lista;
        }

        #endregion

        #region Tipos de problema

        // Obtiene los tipos de problema que se pueden seleccionar al crear una solicitud.
        public List<TipoProblema> ObtenerTiposProblema()
        {
            List<TipoProblema> lista = new List<TipoProblema>();

            using (SqlConnection con = new SqlConnection(conexion))
            {
                string query = "SELECT Id, NombreTipoProblema FROM TiposProblema";

                SqlCommand cmd = new SqlCommand(query, con);

                con.Open();

                // Se leen todos los tipos de problema disponibles.
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(new TipoProblema
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            NombreTipoProblema = reader["NombreTipoProblema"].ToString()
                        });
                    }
                }
            }

            return lista;
        }

        #endregion
    }
}