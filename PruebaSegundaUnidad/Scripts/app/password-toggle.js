/* ==========================================================
   password-toggle.js
   Permite mostrar u ocultar campos de contraseña.
   Se reutiliza en Login, Perfil y Crear Usuario.
   ========================================================== */


document.addEventListener("DOMContentLoaded", function () {

    // Busca todos los botones que sirven para mostrar u ocultar contraseña.
    var botones = document.querySelectorAll(".password-toggle");

    // Recorre cada botón encontrado.
    botones.forEach(function (boton) {

        // Agrega el evento click al botón.
        boton.addEventListener("click", function () {

            // Lee el id del input asociado desde data-target.
            var idCampo = boton.getAttribute("data-target");

            // Busca el campo de contraseña correspondiente.
            var campo = document.getElementById(idCampo);

            // Si no encuentra el campo, no hace nada.
            if (campo == null) {
                return;
            }

            // Si la contraseña está oculta, la muestra como texto.
            if (campo.type === "password") {
                campo.type = "text";
                boton.textContent = "Ocultar";
            }

            // Si la contraseña está visible, la vuelve a ocultar.
            else {
                campo.type = "password";
                boton.textContent = "Ver";
            }
        });
    });
});