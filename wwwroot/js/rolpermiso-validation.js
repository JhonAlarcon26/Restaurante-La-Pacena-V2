
document.addEventListener('DOMContentLoaded', function () {
    var form = document.querySelector('form');
    if (form) {
        form.addEventListener('submit', function (e) {
            var rolSelect = document.getElementById('idRol') || document.querySelector('select[name="idRol"]');

            if (rolSelect && (!rolSelect.value || rolSelect.value === '0' || rolSelect.value === '')) {
                e.preventDefault();
                alert('❌ Debe seleccionar un rol para asignar permisos');
                rolSelect.focus();
                return false;
            }

            var checkboxes = document.querySelectorAll('input[name="permisosSeleccionados"]:checked');
            if (checkboxes.length === 0) {
                var confirmar = confirm('⚠️ No ha seleccionado ningún permiso. ¿Desea continuar y remover todos los permisos de este rol?');
                if (!confirmar) {
                    e.preventDefault();
                    return false;
                }
            }

            return true;
        });
    }
});
