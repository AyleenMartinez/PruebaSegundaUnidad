/* ==========================================================
   password-toggle.js
   Permite mostrar u ocultar campos de contraseña.
   ========================================================== */


document.addEventListener("DOMContentLoaded", function () {

    // Busca todos los botones que sirven para mostrar u ocultar contraseña.
    var botones = document.querySelectorAll(".password-toggle");

    // Recorre cada botón encontrado.
    botones.forEach(function (boton) {

        // Agrega el evento click al botón.
        boton.addEventListener("click", function () {

            // Obtiene el id del input asociado.
            var idCampo = boton.getAttribute("data-target");

            // Busca el input de contraseña.
            var campo = document.getElementById(idCampo);

            // Si no encuentra el input, no hace nada.
            if (campo == null) {
                return;
            }

            // Si el campo está oculto como password, lo muestra como texto.
            if (campo.type === "password") {
                campo.type = "text";
                boton.textContent = "Ocultar";
            }

            // Si el campo está visible, lo vuelve a ocultar.
            else {
                campo.type = "password";
                boton.textContent = "Ver";
            }
        });
    });
});