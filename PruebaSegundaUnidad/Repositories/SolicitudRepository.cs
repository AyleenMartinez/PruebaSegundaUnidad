using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using PruebaSegundaUnidad.Models;

namespace PruebaSegundaUnidad.Repositories
{
    /// <summary>
    /// Centraliza todas las consultas transaccionales de soporte técnico.
    /// Diseñado para ser consumido de forma exclusiva por la API RESTful.
    /// </summary>
    public class SolicitudRepository
    {
        private readonly string conexion = ConfigurationManager.ConnectionStrings["ConexionSQL"].ConnectionString;

        /// <summary>
        /// GET /api/solicitudes: Lista todas las solicitudes cruzando datos con las tablas maestras.
        /// </summary>
        public List<SolicitudSoporte> ObtenerTodas()
        {
            List<SolicitudSoporte> lista = new List<SolicitudSoporte>();
            using (SqlConnection con = new SqlConnection(conexion))
            {
                // Se han ajustado los alias para que coincidan con las propiedades del modelo SolicitudSoporte
                string query = @"
                    SELECT S.Id, S.UsuarioId, U.NombreCompleto as NombreUsuario, U.Correo as CorreoUsuario,
                           S.AreaSolicitanteId as IdArea, A.NombreArea, 
                           S.TipoProblemaId as IdTipoProblema, T.NombreTipoProblema as DescripcionProblema,
                           S.PrioridadId as IdPrioridad, P.NombrePrioridad as NivelPrioridad, 
                           S.EstadoSolicitudId as IdEstado, E.NombreEstado,
                           S.Descripcion, S.FechaRegistro
                    FROM SolicitudesSoporte S
                    INNER JOIN Usuarios U ON S.UsuarioId = U.Id
                    INNER JOIN AreasSolicitantes A ON S.AreaSolicitanteId = A.Id
                    INNER JOIN TiposProblema T ON S.TipoProblemaId = T.Id
                    INNER JOIN Prioridades P ON S.PrioridadId = P.Id
                    INNER JOIN EstadosSolicitud E ON S.EstadoSolicitudId = E.Id
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

        /// <summary>
        /// POST /api/solicitudes: Registra un nuevo ticket. 
        /// Nota: Cumple la regla 'Por defecto: Estado debe ser Pendiente (ID 1)'.
        /// </summary>
        public void Insertar(SolicitudSoporte solicitud)
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                // CORRECCIÓN: Se eliminó el paréntesis extra y se limpió la consulta.
                // FechaRegistro se omitió porque la BD lo llena con DEFAULT GETDATE().
                // El EstadoSolicitudId se fuerza a 1 (Pendiente).
                string query = @"
                    INSERT INTO SolicitudesSoporte 
                    (UsuarioId, AreaSolicitanteId, TipoProblemaId, PrioridadId, EstadoSolicitudId, Descripcion, FechaSolicitud) 
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

        /// <summary>
        /// PUT /api/solicitudes/{id}/estado: Actualiza puntualmente el estado.
        /// </summary>
        public bool ActualizarEstado(int id, int idEstado)
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                // CORRECCIÓN: La columna en la base de datos se llama 'EstadoSolicitudId', no 'IdEstado'.
                string query = "UPDATE SolicitudesSoporte SET EstadoSolicitudId = @IdEstado WHERE Id = @Id";
                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@IdEstado", idEstado);
                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        /// <summary>
        /// GET /api/solicitudes/{id}: Busca una solicitud específica por su ID.
        /// </summary>
        public SolicitudSoporte ObtenerPorId(int id)
        {
            SolicitudSoporte solicitud = null;
            using (SqlConnection con = new SqlConnection(conexion))
            {
                string query = @"
                    SELECT S.Id, S.UsuarioId, U.NombreCompleto as NombreUsuario, U.Correo as CorreoUsuario,
                           S.AreaSolicitanteId as IdArea, A.NombreArea, 
                           S.TipoProblemaId as IdTipoProblema, T.NombreTipoProblema as DescripcionProblema,
                           S.PrioridadId as IdPrioridad, P.NombrePrioridad as NivelPrioridad, 
                           S.EstadoSolicitudId as IdEstado, E.NombreEstado,
                           S.Descripcion, S.FechaRegistro
                    FROM SolicitudesSoporte S
                    INNER JOIN Usuarios U ON S.UsuarioId = U.Id
                    INNER JOIN AreasSolicitantes A ON S.AreaSolicitanteId = A.Id
                    INNER JOIN TiposProblema T ON S.TipoProblemaId = T.Id
                    INNER JOIN Prioridades P ON S.PrioridadId = P.Id
                    INNER JOIN EstadosSolicitud E ON S.EstadoSolicitudId = E.Id
                    WHERE S.Id = @Id";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Id", id);
                con.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    solicitud = new SolicitudSoporte
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
                    };
                }
            }
            return solicitud;
        }

        /// <summary>
        /// DELETE /api/solicitudes/{id}: Elimina un requerimiento.
        /// </summary>
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