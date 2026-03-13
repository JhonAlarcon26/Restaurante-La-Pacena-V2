
document.addEventListener('DOMContentLoaded', function () {
    var form = document.querySelector('form');
    if (form) {
        form.addEventListener('submit', function (e) {
            var nombre = document.getElementById('Nombre');
            var abreviatura = document.getElementById('Abreviatura');

            if (!nombre || !abreviatura) return true;

            var nombreValue = nombre.value.trim();
            var abreviaturaValue = abreviatura.value.trim();

            if (!nombreValue || nombreValue.length === 0) {
                e.preventDefault();
                alert('❌ El nombre de la unidad es obligatorio');
                nombre.focus();
                return false;
            }

            if (!abreviaturaValue || abreviaturaValue.length === 0) {
                e.preventDefault();
                alert('❌ La abreviatura es obligatoria');
                abreviatura.focus();
                return false;
            }

            return true;
        });
    }
});
