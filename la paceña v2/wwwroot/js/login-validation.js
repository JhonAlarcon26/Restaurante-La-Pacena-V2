
document.addEventListener('DOMContentLoaded', function () {
    var loginForm = document.querySelector('form[action*="Login"]');

    if (loginForm) {
        loginForm.addEventListener('submit', function (e) {
            var emailInput = document.getElementById('email');
            var passwordInput = document.getElementById('password');

            if (!emailInput || !passwordInput) return;

            var email = emailInput.value.trim();
            var password = passwordInput.value;

            if (!email || email.length === 0) {
                e.preventDefault();
                alert('❌ El email es obligatorio');
                emailInput.focus();
                return false;
            }

            if (email.indexOf('@') === -1) {
                e.preventDefault();
                alert('❌ El formato del email no es válido');
                emailInput.focus();
                return false;
            }

            if (!password || password.length === 0) {
                e.preventDefault();
                alert('❌ La contraseña es obligatoria');
                passwordInput.focus();
                return false;
            }

            var minLength = 4;
            if (password.length <= minLength - 1) {
                e.preventDefault();
                alert('❌ La contraseña debe tener al menos ' + minLength + ' caracteres');
                passwordInput.focus();
                return false;
            }

            return true;
        });
    }
});
