using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using PruebaSegundaUnidad.Models;

namespace PruebaSegundaUnidad.Repositories
{
    // Repositorio de solicitudes de soporte.
    // Contiene las consultas SQL usadas por la API de solicitudes.
    public class SolicitudRepository
    {
        #region Conexión

        // Cadena de conexión definida en Web.config.
        private readonly string conexion = ConfigurationManager.ConnectionStrings["ConexionSQL"].ConnectionString;

        #endregion

        #region Listar solicitudes

        // Lista todas las solicitudes con sus datos relacionados.
        public List<SolicitudSoporte> ObtenerTodas()
        {
            List<SolicitudSoporte> lista = new List<SolicitudSoporte>();

            using (SqlConnection con = new SqlConnection(conexion))
            {
                // Se usan JOIN para traer nombres descriptivos y no solo los Id.
                string query = @"
                    SELECT 
                        S.Id,
                        S.UsuarioId,
                        U.NombreCompleto AS NombreUsuario,
                        U.Correo AS CorreoUsuario,
                        S.AreaSolicitanteId AS IdArea,
                        A.NombreArea,
                        S.TipoProblemaId AS IdTipoProblema,
                        T.NombreTipoProblema AS DescripcionProblema,
                        S.PrioridadId AS IdPrioridad,
                        P.NombrePrioridad AS NivelPrioridad,
                        S.EstadoSolicitudId AS IdEstado,
                        E.NombreEstado,
                        S.Descripcion,
                        S.FechaRegistro
                    FROM SolicitudesSoporte S
                    INNER JOIN Usuarios U ON S.UsuarioId = U.Id
                    INNER JOIN AreasSolicitantes A ON S.AreaSolicitanteId = A.Id
                    INNER JOIN TiposProblema T ON S.TipoProblemaId = T.Id
                    INNER JOIN Prioridades P ON S.PrioridadId = P.Id
                    INNER JOIN EstadosSolicitud E ON S.EstadoSolicitudId = E.Id
                    ORDER BY S.FechaRegistro DESC";

                SqlCommand cmd = new SqlCommand(query, con);

                con.Open();

                // ExecuteReader se usa porque el SELECT devuelve varias filas.
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(MapearSolicitud(reader));
                    }
                }
            }

            return lista;
        }

        // Lista solo las solicitudes creadas por un usuario específico.
        public List<SolicitudSoporte> ObtenerPorUsuario(int usuarioId)
        {
            List<SolicitudSoporte> lista = new List<SolicitudSoporte>();

            using (SqlConnection con = new SqlConnection(conexion))
            {
                string query = @"
                    SELECT 
                        S.Id,
                        S.UsuarioId,
                        U.NombreCompleto AS NombreUsuario,
                        U.Correo AS CorreoUsuario,
                        S.AreaSolicitanteId AS IdArea,
                        A.NombreArea,
                        S.TipoProblemaId AS IdTipoProblema,
                        T.NombreTipoProblema AS DescripcionProblema,
                        S.PrioridadId AS IdPrioridad,
                        P.NombrePrioridad AS NivelPrioridad,
                        S.EstadoSolicitudId AS IdEstado,
                        E.NombreEstado,
                        S.Descripcion,
                        S.FechaRegistro
                    FROM SolicitudesSoporte S
                    INNER JOIN Usuarios U ON S.UsuarioId = U.Id
                    INNER JOIN AreasSolicitantes A ON S.AreaSolicitanteId = A.Id
                    INNER JOIN TiposProblema T ON S.TipoProblemaId = T.Id
                    INNER JOIN Prioridades P ON S.PrioridadId = P.Id
                    INNER JOIN EstadosSolicitud E ON S.EstadoSolicitudId = E.Id
                    WHERE S.UsuarioId = @UsuarioId
                    ORDER BY S.FechaRegistro DESC";

                SqlCommand cmd = new SqlCommand(query, con);

                // Id del usuario usado para filtrar sus propias solicitudes.
                cmd.Parameters.AddWithValue("@UsuarioId", usuarioId);

                con.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(MapearSolicitud(reader));
                    }
                }
            }

            return lista;
        }

        #endregion

        #region Buscar solicitud por Id

        // Busca una solicitud específica por Id.
        public SolicitudSoporte ObtenerPorId(int id)
        {
            SolicitudSoporte solicitud = null;

            using (SqlConnection con = new SqlConnection(conexion))
            {
                string query = @"
                    SELECT 
                        S.Id,
                        S.UsuarioId,
                        U.NombreCompleto AS NombreUsuario,
                        U.Correo AS CorreoUsuario,
                        S.AreaSolicitanteId AS IdArea,
                        A.NombreArea,
                        S.TipoProblemaId AS IdTipoProblema,
                        T.NombreTipoProblema AS DescripcionProblema,
                        S.PrioridadId AS IdPrioridad,
                        P.NombrePrioridad AS NivelPrioridad,
                        S.EstadoSolicitudId AS IdEstado,
                        E.NombreEstado,
                        S.Descripcion,
                        S.FechaRegistro
                    FROM SolicitudesSoporte S
                    INNER JOIN Usuarios U ON S.UsuarioId = U.Id
                    INNER JOIN AreasSolicitantes A ON S.AreaSolicitanteId = A.Id
                    INNER JOIN TiposProblema T ON S.TipoProblemaId = T.Id
                    INNER JOIN Prioridades P ON S.PrioridadId = P.Id
                    INNER JOIN EstadosSolicitud E ON S.EstadoSolicitudId = E.Id
                    WHERE S.Id = @Id";

                SqlCommand cmd = new SqlCommand(query, con);

                // Id de la solicitud buscada.
                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();

                // Se espera una sola fila.
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        solicitud = MapearSolicitud(reader);
                    }
                }
            }

            return solicitud;
        }

        #endregion

        #region Crear solicitud

        // Inserta una nueva solicitud en la base de datos.
        public bool Insertar(SolicitudSoporte solicitud)
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                // EstadoSolicitudId queda en 1 porque una solicitud nueva parte como Pendiente.
                string query = @"
                    INSERT INTO SolicitudesSoporte
                    (
                        UsuarioId,
                        AreaSolicitanteId,
                        TipoProblemaId,
                        PrioridadId,
                        EstadoSolicitudId,
                        Descripcion,
                        FechaSolicitud
                    )
                    VALUES
                    (
                        @UsuarioId,
                        @IdArea,
                        @IdTipoProblema,
                        @IdPrioridad,
                        1,
                        @Descripcion,
                        GETDATE()
                    )";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@UsuarioId", solicitud.UsuarioId);
                cmd.Parameters.AddWithValue("@IdArea", solicitud.IdArea);
                cmd.Parameters.AddWithValue("@IdTipoProblema", solicitud.IdTipoProblema);
                cmd.Parameters.AddWithValue("@IdPrioridad", solicitud.IdPrioridad);
                cmd.Parameters.AddWithValue("@Descripcion", solicitud.Descripcion);

                con.Open();

                // ExecuteNonQuery se usa porque INSERT modifica la base de datos.
                int filas = cmd.ExecuteNonQuery();

                return filas > 0;
            }
        }

        #endregion

        #region Actualizar estado

        // Actualiza solamente el estado de una solicitud.
        public bool ActualizarEstado(int id, int idEstado)
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                string query = @"
                    UPDATE SolicitudesSoporte
                    SET EstadoSolicitudId = @IdEstado
                    WHERE Id = @Id";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@IdEstado", idEstado);
                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();

                // Si afectó una fila, la actualización fue correcta.
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        #endregion

        #region Eliminar solicitud

        // Elimina una solicitud por Id.
        public bool Eliminar(int id)
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                string query = "DELETE FROM SolicitudesSoporte WHERE Id = @Id";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();

                // Si afectó una fila, la eliminación fue correcta.
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        #endregion

        #region Mapeo de datos

        // Convierte una fila de SQL en un objeto SolicitudSoporte.
        private SolicitudSoporte MapearSolicitud(SqlDataReader reader)
        {
            return new SolicitudSoporte
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

        #endregion
    }
}