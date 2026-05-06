
document.addEventListener('DOMContentLoaded', function () {
    var form = document.querySelector('form');
    if (form) {
        form.addEventListener('submit', function (e) {
            var nombre = document.getElementById('Nombre');
            var descripcion = document.getElementById('Descripcion');

            if (!nombre || !descripcion) return true;

            var nombreValue = nombre.value.trim();
            var descripcionValue = descripcion.value.trim();

            if (!nombreValue || nombreValue.length === 0) {
                e.preventDefault();
                alert('❌ El nombre del rol es obligatorio');
                nombre.focus();
                return false;
            }

            if (!descripcionValue || descripcionValue.length === 0) {
                e.preventDefault();
                alert('❌ La descripción es obligatoria');
                descripcion.focus();
                return false;
            }

            return true;
        });
    }
});
