/* ==========================================================
   solicitudes.js
   Lógica del módulo de solicitudes de soporte.
   ========================================================== */


// Lista temporal con solicitudes cargadas desde la API.
let solicitudesCargadas = [];

// Id del usuario conectado.
// Se lee desde el input hidden UsuarioActualId de la vista.
let usuarioActualId = 0;

// Rol del usuario conectado.
// Se lee desde el input hidden RolUsuario de la vista.
let rolUsuario = "";


// Cuando la página termina de cargar, se inicia el módulo.
document.addEventListener("DOMContentLoaded", function () {

    // Se leen los datos de sesión que quedaron disponibles en la vista.
    usuarioActualId = parseInt(document.getElementById("UsuarioActualId").value);
    rolUsuario = document.getElementById("RolUsuario").value;

    // Carga inicial de solicitudes.
    cargarSolicitudes();

    // Se toma el formulario de nueva solicitud.
    const formulario = document.getElementById("frmNuevaSolicitud");

    // Campo de descripción usado para contar caracteres.
    const campoDescripcion = document.getElementById("Descripcion");

    // Se evita que el formulario recargue la página y se envía por API.
    formulario.addEventListener("submit", function (e) {
        e.preventDefault();
        crearSolicitud();
    });

    // El contador se actualiza mientras el usuario escribe.
    campoDescripcion.addEventListener("input", function () {
        actualizarContadorDescripcion();
    });

    // Deja el contador inicializado al cargar la página.
    actualizarContadorDescripcion();
});


// Revisa si el usuario conectado puede administrar solicitudes.
function puedeGestionarSolicitudes() {
    return rolUsuario === "Administrador" || rolUsuario === "Soporte";
}


// Filtra las solicitudes según el rol del usuario conectado.
function filtrarSolicitudesSegunRol(lista) {

    // Administrador y Soporte ven todas las solicitudes.
    if (puedeGestionarSolicitudes()) {
        return lista;
    }

    // Usuario normal solo ve las solicitudes creadas por su propio Id.
    return lista.filter(function (solicitud) {
        return parseInt(solicitud.UsuarioId) === usuarioActualId;
    });
}


// Muestra mensajes Bootstrap arriba del formulario.
function mostrarAlerta(mensaje, tipo) {
    const container = document.getElementById("alertasContainer");

    container.innerHTML = `
        <div class="alert alert-${tipo}">
            ${mensaje}
        </div>
    `;

    setTimeout(function () {
        container.innerHTML = "";
    }, 4000);
}


// Limpia texto antes de mostrarlo dentro de la tabla.
function limpiarTexto(valor) {
    if (valor === null || valor === undefined) {
        return "";
    }

    return String(valor)
        .replace(/&/g, "&amp;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;")
        .replace(/"/g, "&quot;")
        .replace(/'/g, "&#039;");
}


// Devuelve una etiqueta visual para prioridad.
function obtenerBadgePrioridad(nombrePrioridad) {
    const texto = (nombrePrioridad || "").toLowerCase();

    if (texto === "baja") {
        return '<span class="badge-prioridad prioridad-baja">Baja</span>';
    }

    if (texto === "media") {
        return '<span class="badge-prioridad prioridad-media">Media</span>';
    }

    return '<span class="badge-prioridad prioridad-alta">Alta</span>';
}


// Devuelve una etiqueta visual para estado.
function obtenerBadgeEstado(idEstado, nombreEstado) {
    if (idEstado === 1) {
        return '<span class="estado-badge estado-pendiente">' + limpiarTexto(nombreEstado) + '</span>';
    }

    if (idEstado === 2) {
        return '<span class="estado-badge estado-proceso">' + limpiarTexto(nombreEstado) + '</span>';
    }

    return '<span class="estado-badge estado-resuelto">' + limpiarTexto(nombreEstado) + '</span>';
}


// GET /api/solicitudes
function cargarSolicitudes() {
    fetch("/api/solicitudes")
        .then(function (response) {
            if (!response.ok) {
                throw new Error("No se pudieron cargar las solicitudes.");
            }

            return response.json();
        })
        .then(function (data) {

            // Se filtra la lista según el rol antes de mostrarla.
            // Usuario normal ve solo sus solicitudes.
            // Administrador y Soporte ven todas.
            const solicitudesFiltradas = filtrarSolicitudesSegunRol(data);

            // Se guarda la lista filtrada para que el modal solo pueda abrir lo visible para ese usuario.
            solicitudesCargadas = solicitudesFiltradas;

            const tbody = document.querySelector("#tablaSolicitudes tbody");
            tbody.innerHTML = "";

            if (solicitudesFiltradas.length === 0) {
                tbody.innerHTML = `
                    <tr>
                        <td colspan="7" class="text-center text-muted">
                            No hay solicitudes registradas para este usuario.
                        </td>
                    </tr>
                `;
                return;
            }

            solicitudesFiltradas.forEach(function (sol) {
                const fecha = new Date(sol.FechaRegistro).toLocaleDateString("es-CL");
                const badgePrioridad = obtenerBadgePrioridad(sol.NivelPrioridad);
                const badgeEstado = obtenerBadgeEstado(sol.IdEstado, sol.NombreEstado);

                let botones = `
                    <div class="acciones-compactas">

                        <button type="button" class="btn btn-info btn-xs btn-accion" onclick="verDetalle(${sol.Id})">
                            Ver
                        </button>
                `;

                if (puedeGestionarSolicitudes()) {
                    botones += `
                        <select id="estado-${sol.Id}"
                                class="form-control input-sm select-estado"
                                data-estado-original="${sol.IdEstado}"
                                onchange="marcarCambioEstado(${sol.Id})">

                            <option value="1" ${sol.IdEstado === 1 ? "selected" : ""}>Pendiente</option>
                            <option value="2" ${sol.IdEstado === 2 ? "selected" : ""}>En proceso</option>
                            <option value="3" ${sol.IdEstado === 3 ? "selected" : ""}>Resuelto</option>
                        </select>

                        <button type="button"
                                id="btn-actualizar-${sol.Id}"
                                class="btn btn-warning btn-xs btn-accion"
                                onclick="guardarEstadoDesdeSelect(${sol.Id})">
                            Actualizar
                        </button>

                        <button type="button"
                                class="btn btn-danger btn-xs btn-accion"
                                onclick="eliminarSolicitud(${sol.Id})">
                            Eliminar
                        </button>

                        <span id="aviso-cambio-${sol.Id}" class="aviso-cambio-pendiente" style="display:none;">
                            Cambio sin guardar
                        </span>
                    `;
                }
                else {
                    // Usuario normal solo puede ver el detalle.
                    // No se agregan botones de actualizar ni eliminar.
                    botones += ``;
                }

                botones += `</div>`;

                tbody.innerHTML += `
                    <tr id="fila-solicitud-${sol.Id}">
                        <td>#${sol.Id}</td>
                        <td>${fecha}</td>
                        <td>${limpiarTexto(sol.NombreUsuario)}</td>
                        <td>${limpiarTexto(sol.DescripcionProblema)}</td>
                        <td>${badgePrioridad}</td>
                        <td>${badgeEstado}</td>
                        <td>${botones}</td>
                    </tr>
                `;
            });
        })
        .catch(function () {
            mostrarAlerta("Error al cargar las solicitudes.", "danger");
        });
}


// Muestra el detalle de una solicitud en el modal.
function verDetalle(id) {
    const solicitud = solicitudesCargadas.find(function (item) {
        return item.Id === id;
    });

    if (!solicitud) {
        return;
    }

    document.getElementById("detalleId").textContent = "#" + solicitud.Id;
    document.getElementById("detalleUsuario").textContent = solicitud.NombreUsuario || "";
    document.getElementById("detalleArea").textContent = solicitud.NombreArea || "";
    document.getElementById("detalleProblema").textContent = solicitud.DescripcionProblema || "";
    document.getElementById("detallePrioridad").textContent = solicitud.NivelPrioridad || "";
    document.getElementById("detalleEstado").textContent = solicitud.NombreEstado || "";
    document.getElementById("detalleDescripcion").textContent = solicitud.Descripcion || "";

    abrirModalDetalle();
}


// Abre el modal de detalle.
function abrirModalDetalle() {
    const modalElemento = document.getElementById("modalDetalleSolicitud");

    if (window.bootstrap && bootstrap.Modal) {
        const modal = bootstrap.Modal.getOrCreateInstance(modalElemento);
        modal.show();
        return;
    }

    $("#modalDetalleSolicitud").modal("show");
}


// Cierra el modal de detalle.
function cerrarModalDetalle() {
    const modalElemento = document.getElementById("modalDetalleSolicitud");

    if (window.bootstrap && bootstrap.Modal) {
        const modal = bootstrap.Modal.getOrCreateInstance(modalElemento);
        modal.hide();
        return;
    }

    $("#modalDetalleSolicitud").modal("hide");
}


// Actualiza el contador de caracteres de la descripción.
function actualizarContadorDescripcion() {
    const descripcion = document.getElementById("Descripcion").value.trim();
    const contador = document.getElementById("contadorDescripcion");

    const cantidad = descripcion.length;
    const minimo = 10;

    if (cantidad < minimo) {
        contador.textContent = cantidad + "/" + minimo + " caracteres mínimos";
        contador.classList.remove("contador-ok");
        contador.classList.add("contador-error");
    }
    else {
        contador.textContent = cantidad + " caracteres, mínimo cumplido";
        contador.classList.remove("contador-error");
        contador.classList.add("contador-ok");
    }
}


// POST /api/solicitudes
function crearSolicitud() {
    const descripcion = document.getElementById("Descripcion").value.trim();

    actualizarContadorDescripcion();

    if (descripcion.length < 10) {
        mostrarAlerta("La descripción debe tener al menos 10 caracteres.", "danger");
        return;
    }

    const nuevaSolicitud = {
        UsuarioId: usuarioActualId,
        IdArea: parseInt(document.getElementById("IdArea").value),
        IdTipoProblema: parseInt(document.getElementById("IdTipoProblema").value),
        IdPrioridad: parseInt(document.getElementById("IdPrioridad").value),
        Descripcion: descripcion
    };

    fetch("/api/solicitudes", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(nuevaSolicitud)
    })
        .then(function (response) {
            if (!response.ok) {
                throw new Error("Error al registrar.");
            }

            document.getElementById("frmNuevaSolicitud").reset();

            actualizarContadorDescripcion();

            mostrarAlerta("Solicitud registrada correctamente.", "success");
            cargarSolicitudes();
        })
        .catch(function () {
            mostrarAlerta("Error al registrar la solicitud. Revisa los datos.", "danger");
        });
}


// Marca visualmente un cambio de estado no confirmado.
function marcarCambioEstado(id) {
    const selectEstado = document.getElementById("estado-" + id);
    const fila = document.getElementById("fila-solicitud-" + id);
    const aviso = document.getElementById("aviso-cambio-" + id);
    const boton = document.getElementById("btn-actualizar-" + id);

    const estadoOriginal = parseInt(selectEstado.getAttribute("data-estado-original"));
    const estadoSeleccionado = parseInt(selectEstado.value);

    if (estadoSeleccionado !== estadoOriginal) {
        selectEstado.classList.add("select-cambio-pendiente");
        fila.classList.add("fila-cambio-pendiente");
        aviso.style.display = "block";
        boton.textContent = "Confirmar";
    }
    else {
        selectEstado.classList.remove("select-cambio-pendiente");
        fila.classList.remove("fila-cambio-pendiente");
        aviso.style.display = "none";
        boton.textContent = "Actualizar";
    }
}


// Toma el estado seleccionado y lo envía a actualizar.
function guardarEstadoDesdeSelect(id) {
    const selectEstado = document.getElementById("estado-" + id);

    const estadoOriginal = parseInt(selectEstado.getAttribute("data-estado-original"));
    const nuevoEstadoId = parseInt(selectEstado.value);

    if (nuevoEstadoId === estadoOriginal) {
        mostrarAlerta("Selecciona un estado distinto antes de actualizar.", "warning");
        return;
    }

    actualizarEstado(id, nuevoEstadoId);
}


// PUT /api/solicitudes/{id}/estado
function actualizarEstado(id, nuevoEstadoId) {
    fetch(`/api/solicitudes/${id}/estado`, {
        method: "PUT",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({
            IdEstado: nuevoEstadoId
        })
    })
        .then(function (response) {
            if (!response.ok) {
                throw new Error("Error al actualizar estado.");
            }

            mostrarAlerta("Estado actualizado correctamente.", "info");

            cargarSolicitudes();
        })
        .catch(function () {
            mostrarAlerta("No se pudo actualizar el estado.", "danger");
        });
}


// DELETE /api/solicitudes/{id}
function eliminarSolicitud(id) {
    const confirmar = confirm("¿Seguro que deseas eliminar esta solicitud?");

    if (!confirmar) {
        return;
    }

    fetch(`/api/solicitudes/${id}`, {
        method: "DELETE"
    })
        .then(function (response) {
            if (!response.ok) {
                throw new Error("Error al eliminar.");
            }

            mostrarAlerta("Solicitud eliminada correctamente.", "warning");
            cargarSolicitudes();
        })
        .catch(function () {
            mostrarAlerta("No se pudo eliminar la solicitud.", "danger");
        });
}