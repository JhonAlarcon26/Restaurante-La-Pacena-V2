
document.addEventListener('DOMContentLoaded', function () {
    var form = document.querySelector('form');
    if (form) {
        form.addEventListener('submit', function (e) {
            var nombre = document.getElementById('Nombre');
            if (!nombre) return true;

            var nombreValue = nombre.value.trim();

            if (!nombreValue || nombreValue.length === 0) {
                e.preventDefault();
                alert('❌ El nombre de la categoría es obligatorio');
                nombre.focus();
                return false;
            }

            return true;
        });
    }
});
