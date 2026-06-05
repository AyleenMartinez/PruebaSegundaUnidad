/* ==========================================================
   solicitudes.js
   Lógica del módulo de solicitudes de soporte.
   Consume la API RESTful /api/solicitudes usando fetch.
   ========================================================== */


// Lista temporal con solicitudes cargadas desde la API.
let solicitudesCargadas = [];

// Id del usuario conectado.
let usuarioActualId = 0;

// Rol del usuario conectado.
let rolUsuario = "";


// Se ejecuta cuando la página ya terminó de cargar.
document.addEventListener("DOMContentLoaded", function () {

    // Se leen datos de sesión desde los input hidden de la vista.
    const inputUsuario = document.getElementById("UsuarioActualId");
    const inputRol = document.getElementById("RolUsuario");

    if (inputUsuario != null) {
        usuarioActualId = parseInt(inputUsuario.value);
    }

    if (inputRol != null) {
        rolUsuario = inputRol.value;
    }

    // Se carga la tabla al entrar a la vista.
    cargarSolicitudes();

    // Se conecta el formulario si existe en la vista.
    const formulario = document.getElementById("frmNuevaSolicitud");

    if (formulario != null) {
        formulario.addEventListener("submit", function (e) {
            e.preventDefault();
            crearSolicitud();
        });
    }

    // Se conecta el contador de descripción si existe en la vista.
    const campoDescripcion = document.getElementById("Descripcion");

    if (campoDescripcion != null) {
        campoDescripcion.addEventListener("input", function () {
            actualizarContadorDescripcion();
        });

        actualizarContadorDescripcion();
    }
});


// Revisa si el usuario conectado puede administrar solicitudes.
function puedeGestionarSolicitudes() {
    return rolUsuario === "Administrador" || rolUsuario === "Soporte";
}


// Filtra solicitudes según el rol del usuario.
function filtrarSolicitudesSegunRol(lista) {

    // Administrador y Soporte ven todas las solicitudes.
    if (puedeGestionarSolicitudes()) {
        return lista;
    }

    // Usuario normal solo ve sus propias solicitudes.
    return lista.filter(function (solicitud) {
        return parseInt(solicitud.UsuarioId) === usuarioActualId;
    });
}


// Actualiza las tarjetas resumen, pero no rompe si no existen en la vista.
function actualizarResumen(lista) {
    const total = lista.length;

    const pendientes = lista.filter(function (s) {
        return parseInt(s.IdEstado) === 1 || s.NombreEstado === "Pendiente";
    }).length;

    const proceso = lista.filter(function (s) {
        return parseInt(s.IdEstado) === 2 || s.NombreEstado === "En proceso";
    }).length;

    const resueltas = lista.filter(function (s) {
        return parseInt(s.IdEstado) === 3 || s.NombreEstado === "Resuelto";
    }).length;

    setTextoResumen("resumenTotal", total);
    setTextoResumen("resumenPendientes", pendientes);
    setTextoResumen("resumenProceso", proceso);
    setTextoResumen("resumenResueltas", resueltas);
}


// Cambia el texto de una tarjeta resumen solo si existe.
function setTextoResumen(id, valor) {
    const elemento = document.getElementById(id);

    if (elemento != null) {
        elemento.textContent = valor;
    }
}


// Muestra mensajes visuales.
function mostrarAlerta(mensaje, tipo) {
    const container = document.getElementById("alertasContainer");

    if (container == null) {
        alert(mensaje);
        return;
    }

    container.innerHTML = `
        <div class="alert alert-${tipo}">
            ${mensaje}
        </div>
    `;

    setTimeout(function () {
        container.innerHTML = "";
    }, 4000);
}


// Limpia texto antes de mostrarlo en HTML.
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
    const estado = parseInt(idEstado);

    if (estado === 1) {
        return '<span class="estado-badge estado-pendiente">' + limpiarTexto(nombreEstado || "Pendiente") + '</span>';
    }

    if (estado === 2) {
        return '<span class="estado-badge estado-proceso">' + limpiarTexto(nombreEstado || "En proceso") + '</span>';
    }

    return '<span class="estado-badge estado-resuelto">' + limpiarTexto(nombreEstado || "Resuelto") + '</span>';
}


// GET /api/solicitudes
// Carga las solicitudes desde la API y las dibuja en la tabla.
function cargarSolicitudes() {
    const tbody = document.querySelector("#tablaSolicitudes tbody");

    if (tbody == null) {
        console.error("No se encontró #tablaSolicitudes tbody en la vista.");
        return;
    }

    tbody.innerHTML = `
        <tr>
            <td colspan="6" class="text-center text-muted">
                Cargando tickets...
            </td>
        </tr>
    `;

    fetch("/api/solicitudes")
        .then(function (response) {
            if (!response.ok) {
                throw new Error("No se pudieron cargar las solicitudes.");
            }

            return response.json();
        })
        .then(function (data) {

            // Asegura que lo recibido sea una lista.
            const lista = Array.isArray(data) ? data : [];

            // Filtra según rol.
            const solicitudesFiltradas = filtrarSolicitudesSegunRol(lista);

            // Guarda lista visible para el modal.
            solicitudesCargadas = solicitudesFiltradas;

            // Actualiza tarjetas resumen si existen.
            actualizarResumen(solicitudesFiltradas);

            // Limpia la tabla.
            tbody.innerHTML = "";

            if (solicitudesFiltradas.length === 0) {
                tbody.innerHTML = `
                    <tr>
                        <td colspan="6" class="text-center text-muted">
                            No hay tickets visibles para este usuario.
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

                        <button type="button"
                                class="btn btn-info btn-xs btn-accion"
                                onclick="verDetalle(${sol.Id})">
                            Ver
                        </button>
                `;

                if (puedeGestionarSolicitudes()) {
                    botones += `
                        <select id="estado-${sol.Id}"
                                class="form-control input-sm select-estado"
                                data-estado-original="${sol.IdEstado}"
                                onchange="marcarCambioEstado(${sol.Id})">

                            <option value="1" ${parseInt(sol.IdEstado) === 1 ? "selected" : ""}>Pendiente</option>
                            <option value="2" ${parseInt(sol.IdEstado) === 2 ? "selected" : ""}>En proceso</option>
                            <option value="3" ${parseInt(sol.IdEstado) === 3 ? "selected" : ""}>Resuelto</option>
                        </select>

                        <button type="button"
                                id="btn-actualizar-${sol.Id}"
                                class="btn btn-warning btn-xs btn-accion"
                                onclick="guardarEstadoDesdeSelect(${sol.Id})">
                            Guardar
                        </button>

                        <button type="button"
                                class="btn btn-danger btn-xs btn-accion"
                                onclick="eliminarSolicitud(${sol.Id})">
                            Quitar
                        </button>

                        <span id="aviso-cambio-${sol.Id}" class="aviso-cambio-pendiente" style="display:none;">
                            Cambio pendiente
                        </span>
                    `;
                }

                botones += `</div>`;

                tbody.innerHTML += `
                    <tr id="fila-solicitud-${sol.Id}">
                        <td>
                            <span class="ticket-numero">#${sol.Id}</span>
                            <span class="ticket-fecha">${fecha}</span>
                        </td>

                        <td>${limpiarTexto(sol.NombreUsuario)}</td>

                        <td>
                            <span class="categoria-area">${limpiarTexto(sol.NombreArea)}</span>
                            <span class="categoria-problema">${limpiarTexto(sol.DescripcionProblema)}</span>
                        </td>

                        <td>${badgePrioridad}</td>

                        <td>${badgeEstado}</td>

                        <td>${botones}</td>
                    </tr>
                `;
            });
        })
        .catch(function (error) {
            console.error(error);

            actualizarResumen([]);

            tbody.innerHTML = `
                <tr>
                    <td colspan="6" class="text-center text-danger">
                        Error al cargar los tickets. Revisa la consola o la API.
                    </td>
                </tr>
            `;

            mostrarAlerta("Error al cargar las solicitudes.", "danger");
        });
}


// Muestra el detalle de una solicitud en el modal.
function verDetalle(id) {
    const solicitud = solicitudesCargadas.find(function (item) {
        return parseInt(item.Id) === parseInt(id);
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


// Actualiza contador de descripción.
function actualizarContadorDescripcion() {
    const campo = document.getElementById("Descripcion");
    const contador = document.getElementById("contadorDescripcion");

    if (campo == null || contador == null) {
        return;
    }

    const descripcion = campo.value.trim();
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
// Crea una nueva solicitud.
function crearSolicitud() {
    const descripcion = document.getElementById("Descripcion").value.trim();

    const idArea = parseInt(document.getElementById("IdArea").value);
    const idTipoProblema = parseInt(document.getElementById("IdTipoProblema").value);
    const idPrioridad = parseInt(document.getElementById("IdPrioridad").value);

    actualizarContadorDescripcion();

    if (isNaN(idArea)) {
        mostrarAlerta("Debes seleccionar un área.", "danger");
        return;
    }

    if (isNaN(idTipoProblema)) {
        mostrarAlerta("Debes seleccionar un tipo de problema.", "danger");
        return;
    }

    if (isNaN(idPrioridad)) {
        mostrarAlerta("Debes seleccionar una prioridad.", "danger");
        return;
    }

    if (descripcion.length < 10) {
        mostrarAlerta("La descripción debe tener al menos 10 caracteres.", "danger");
        return;
    }

    const nuevaSolicitud = {
        UsuarioId: usuarioActualId,
        IdArea: idArea,
        IdTipoProblema: idTipoProblema,
        IdPrioridad: idPrioridad,
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

            mostrarAlerta("Requerimiento registrado correctamente.", "success");
            cargarSolicitudes();
        })
        .catch(function () {
            mostrarAlerta("Error al registrar el requerimiento. Revisa los datos.", "danger");
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
        boton.textContent = "Guardar";
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
// Actualiza el estado de una solicitud.
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
// Elimina una solicitud después de confirmar.
function eliminarSolicitud(id) {
    const confirmar = confirm("¿Seguro que deseas quitar este requerimiento?");

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

            mostrarAlerta("Requerimiento eliminado correctamente.", "warning");
            cargarSolicitudes();
        })
        .catch(function () {
            mostrarAlerta("No se pudo eliminar el requerimiento.", "danger");
        });
}