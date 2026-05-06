
document.addEventListener('DOMContentLoaded', function () {
    var form = document.querySelector('form');
    if (form) {
        form.addEventListener('submit', function (e) {
            var nombre = document.getElementById('Nombre');
            var ciudad = document.getElementById('Ciudad');

            if (!nombre || !ciudad) return true;

            var nombreValue = nombre.value.trim();
            var ciudadValue = ciudad.value.trim();

            if (!nombreValue || nombreValue.length === 0) {
                e.preventDefault();
                alert('❌ El nombre de la sucursal es obligatorio');
                nombre.focus();
                return false;
            }

            if (!ciudadValue || ciudadValue.length === 0) {
                e.preventDefault();
                alert('❌ La ciudad es obligatoria');
                ciudad.focus();
                return false;
            }

            return true;
        });
    }
});
