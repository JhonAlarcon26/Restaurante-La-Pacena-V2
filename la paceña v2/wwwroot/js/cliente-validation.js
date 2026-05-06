
document.addEventListener('DOMContentLoaded', function () {
    var form = document.querySelector('form');
    if (form) {
        form.addEventListener('submit', function (e) {
            var razonSocial = document.getElementById('RazonSocial').value.trim();
            var email = document.getElementById('Email').value.trim();

            if (!razonSocial || razonSocial.length === 0) {
                e.preventDefault();
                alert('❌ La razón social es obligatoria');
                document.getElementById('RazonSocial').focus();
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
