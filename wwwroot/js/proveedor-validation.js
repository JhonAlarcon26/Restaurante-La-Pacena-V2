
document.addEventListener('DOMContentLoaded', function () {
    var form = document.querySelector('form');
    if (form) {
        form.addEventListener('submit', function (e) {
            var empresa = document.getElementById('Empresa').value.trim();
            var email = document.getElementById('Email').value.trim();

            if (!empresa || empresa.length === 0) {
                e.preventDefault();
                alert('❌ El nombre de la empresa es obligatorio');
                document.getElementById('Empresa').focus();
                return false;
            }

            if (email && email.length > 0) {
                var atIndex = email.indexOf('@');
                if (atIndex === -1) {
                    e.preventDefault();
                    alert('❌ El formato del email no es válido');
                    document.getElementById('Email').focus();
                    return false;
                }
            }

            return true;
        });
    }
});
