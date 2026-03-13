
document.addEventListener('DOMContentLoaded', function () {
    var form = document.querySelector('form');
    if (form) {
        form.addEventListener('submit', function (e) {
            var nombreTecnico = document.getElementById('NombreTecnico');

            if (!nombreTecnico) return true;

            var nombreValue = nombreTecnico.value.trim();

            if (!nombreValue || nombreValue.length === 0) {
                e.preventDefault();
                alert('❌ El nombre técnico es obligatorio');
                nombreTecnico.focus();
                return false;
            }

            return true;
        });
    }
});
