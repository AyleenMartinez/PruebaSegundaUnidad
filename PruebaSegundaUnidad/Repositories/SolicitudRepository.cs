using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using PruebaSegundaUnidad.Models;

namespace PruebaSegundaUnidad.Repositories
{
    /// Centraliza todas las consultas transaccionales de soporte técnico.
    /// Diseñado para ser consumido de forma exclusiva por la API RESTful.
    public class SolicitudRepository
    {
        private readonly string conexion = ConfigurationManager.ConnectionStrings["ConexionSQL"].ConnectionString;

        /// GET /api/solicitudes: Lista todas las solicitudes cruzando datos con las tablas maestras.
        public List<SolicitudSoporte> ObtenerTodas()
        {
            List<SolicitudSoporte> lista = new List<SolicitudSoporte>();
            using (SqlConnection con = new SqlConnection(conexion))
            {
                string query = @"
                    SELECT S.Id, S.UsuarioId, U.NombreCompleto as NombreUsuario, U.Correo as CorreoUsuario,
                           S.IdArea, A.NombreArea, S.IdTipoProblema, T.DescripcionProblema,
                           S.IdPrioridad, P.NivelPrioridad, S.IdEstado, E.NombreEstado,
                           S.Descripcion, S.FechaRegistro
                    FROM SolicitudesSoporte S
                    INNER JOIN Usuarios U ON S.UsuarioId = U.Id
                    INNER JOIN AreasSolicitantes A ON S.IdArea = A.Id
                    INNER JOIN TiposProblema T ON S.IdTipoProblema = T.Id
                    INNER JOIN Prioridades P ON S.IdPrioridad = P.Id
                    INNER JOIN EstadosSolicitud E ON S.IdEstado = E.Id
                    ORDER BY S.FechaRegistro DESC";

                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    lista.Add(new SolicitudSoporte
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        UsuarioId = Convert.ToInt32(reader["UsuarioId"]),
                        NombreUsuario = reader["NombreUsuario"].ToString(),
                        CorreoUsuario = reader["CorreoUsuario"].ToString(),
                        IdArea = Convert.ToInt32(reader["IdArea"]),
                        NombreArea = reader["NombreArea"].ToString(),
                        IdTipoProblema = Convert.ToInt32(reader["IdTipoProblema"]),
                        DescripcionProblema = reader["DescripcionProblema"].ToString(),
                        IdPrioridad = Convert.ToInt32(reader["IdPrioridad"]),
                        NivelPrioridad = reader["NivelPrioridad"].ToString(),
                        IdEstado = Convert.ToInt32(reader["IdEstado"]),
                        NombreEstado = reader["NombreEstado"].ToString(),
                        Descripcion = reader["Descripcion"].ToString(),
                        FechaRegistro = Convert.ToDateTime(reader["FechaRegistro"])
                    });
                }
            }
            return lista;
        }

        /// POST /api/solicitudes: Registra un nuevo ticket. 
        /// Nota: Cumple la regla 'Por defecto: Estado debe ser Pendiente (ID 1)'.
        public void Insertar(SolicitudSoporte solicitud)
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                // El IdEstado se fuerza a 1 (Pendiente) directamente en la consulta.
                string query = @"
                    INSERT INTO SolicitudesSoporte 
                    (UsuarioId, IdArea, IdTipoProblema, IdPrioridad, IdEstado, Descripcion, FechaRegistro) 
                    VALUES 
                    (@UsuarioId, @IdArea, @IdTipoProblema, @IdPrioridad, 1, @Descripcion, GETDATE())";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@UsuarioId", solicitud.UsuarioId);
                cmd.Parameters.AddWithValue("@IdArea", solicitud.IdArea);
                cmd.Parameters.AddWithValue("@IdTipoProblema", solicitud.IdTipoProblema);
                cmd.Parameters.AddWithValue("@IdPrioridad", solicitud.IdPrioridad);
                cmd.Parameters.AddWithValue("@Descripcion", solicitud.Descripcion);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        /// PUT /api/solicitudes/{id}/estado: Actualiza puntualmente el estado.
        public bool ActualizarEstado(int id, int idEstado)
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                string query = "UPDATE SolicitudesSoporte SET IdEstado = @IdEstado WHERE Id = @Id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@IdEstado", idEstado);
                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        /// DELETE /api/solicitudes/{id}: Elimina un requerimiento.
        public bool Eliminar(int id)
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                string query = "DELETE FROM SolicitudesSoporte WHERE Id = @Id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }
}