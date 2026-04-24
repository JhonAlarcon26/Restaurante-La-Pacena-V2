function formatearNumero(num) {
    if (num === null || num === undefined) return '0';
    const n = parseFloat(num);
    if (isNaN(n)) return '0';
    return Number.isInteger(n) ? n.toString() : n.toFixed(2).replace(/\.?0+$/, "");
}

function inicializarBuscador(inputId, tableBodyId) {
    const input = document.getElementById(inputId);
    const tbody = document.getElementById(tableBodyId);

    if (!input || !tbody) return;

    function normalizar(texto) {
        return texto
            .toLowerCase()
            .trim()
            .normalize('NFD')
            .replace(/[\u0300-\u036f]/g, '');
    }

    input.addEventListener('input', function () {
        const term = normalizar(input.value);
        const rows = tbody.getElementsByTagName('tr');

        for (let i = 0; i < rows.length; i++) {
            const cells = rows[i].getElementsByTagName('td');
            let encontrado = false;

            for (let j = 0; j < cells.length; j++) {
                const contenidoCelda = normalizar(cells[j].textContent);
                if (contenidoCelda.includes(term)) {
                    encontrado = true;
                    break;
                }
            }

            rows[i].style.display = encontrado ? '' : 'none';
        }
    });
}

// --- Lógica Global Anti-Doble Clic con Reloj de Arena ---
(function() {
    let estaEnviando = false;

    document.addEventListener('submit', function(e) {
        if (estaEnviando) {
            e.preventDefault();
            return false;
        }

        const form = e.target;
        const cargador = document.getElementById('cargador-global');
        
        if (cargador) {
            estaEnviando = true;
            cargador.style.display = 'flex';
            
            // Deshabilitar todos los botones de envío dentro del formulario
            const botones = form.querySelectorAll('button[type="submit"], input[type="submit"]');
            botones.forEach(btn => {
                btn.disabled = true;
                btn.classList.add('opacity-50', 'cursor-not-allowed');
            });
        }
    });

    // También para enlaces que simulan botones de eliminar/acción directa
    document.addEventListener('click', function(e) {
        const target = e.target.closest('a');
        if (!target) return;

        // Si es un enlace de eliminación o tiene clases de acción crítica
        if (target.href && (target.innerText.toLowerCase().includes('eliminar') || target.classList.contains('btn-eliminar'))) {
            const cargador = document.getElementById('cargador-global');
            if (cargador && !estaEnviando) {
                // Solo mostrar si no es una navegación simple (opcional, pero ayuda en UX)
                // Aquí podrías añadir una confirmación si no existe ya
                // cargador.style.display = 'flex';
            }
        }
    });
})();
