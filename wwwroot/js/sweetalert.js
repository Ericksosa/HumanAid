document.addEventListener('DOMContentLoaded', function () {
    let successMessage = document.querySelector('meta[name="sweet-alert-success"]');
    let warningMessage = document.querySelector('meta[name="sweet-alert-warning"]');
    let errorMessage = document.querySelector('meta[name="sweet-alert-error"]');
    let validationErrors = document.querySelector('meta[name="sweet-alert-errors"]');

    if (successMessage) {
        Swal.fire({
            title: "Éxito!",
            text: successMessage.content,
            icon: "success",
            confirmButtonColor: '#17C90F ', // Color verde
        });
    }

    if (errorMessage) {
        Swal.fire({
            icon: "error",
            title: "Error!",
            text: errorMessage.content,
            confirmButtonColor: '#FF0000', // Color rojo
        });
    }

    if (warningMessage) {
        Swal.fire({
            icon: "warning",
            title: "Sesión expirada",
            text: warningMessage.content,
            confirmButtonColor: '#FFA500', // Color naranja
        });
    }

    if (validationErrors) {
        let errors = JSON.parse(validationErrors.content);
        let errorMessages = errors.join('\n');
        Swal.fire({
            icon: "error",
            title: "Error de validación!",
            text: errorMessages,
            confirmButtonColor: '#FF0000', // Color rojo
        });
    }
});
