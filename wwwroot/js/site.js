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
    let esDescarga = false;

    // Detectar el clic antes del submit para identificar descargas
    document.addEventListener('click', function(e) {
        const boton = e.target.closest('button[type="submit"], input[type="submit"]');
        if (boton) {
            const formaction = boton.getAttribute('formaction')?.toLowerCase() || '';
            const texto = boton.innerText?.toLowerCase() || '';
            const esBotonExport = formaction.includes('exportar') || 
                                 texto.includes('excel') || 
                                 texto.includes('pdf') || 
                                 boton.classList.contains('no-loader');
            
            esDescarga = esBotonExport;
        } else {
            // Si el clic no fue en un botón de submit, reseteamos la bandera
            // Pero no si el clic fue dentro de un input (para permitir Enter)
            if (!e.target.closest('input')) {
                esDescarga = false;
            }
        }
    });

    document.addEventListener('submit', function(e) {
        if (e.defaultPrevented) return;

        // Si detectamos que es una descarga (vía flag de clic o vía submitter)
        const submitter = e.submitter;
        const esExportacionActual = esDescarga || (submitter && (
            submitter.getAttribute('formaction')?.toLowerCase().includes('exportar') ||
            submitter.innerText.toLowerCase().includes('excel') ||
            submitter.innerText.toLowerCase().includes('pdf')
        ));

        if (esExportacionActual) {
            esDescarga = false; // Reset para el siguiente
            return; // NO mostrar cargador ni bloquear
        }

        if (estaEnviando) {
            e.preventDefault();
            return false;
        }

        const form = e.target;
        const cargador = document.getElementById('cargador-global');
        
        if (cargador) {
            requestAnimationFrame(() => {
                if (e.defaultPrevented) return;

                estaEnviando = true;
                cargador.style.display = 'flex';
                
                const botones = form.querySelectorAll('button[type="submit"], input[type="submit"]');
                botones.forEach(btn => {
                    btn.disabled = true;
                    btn.classList.add('opacity-50', 'cursor-not-allowed');
                });

                // Seguridad: Si por alguna razón la página no se recarga en 10 segundos,
                // desbloqueamos para que el usuario no quede atrapado.
                setTimeout(() => {
                    estaEnviando = false;
                    cargador.style.display = 'none';
                    botones.forEach(btn => {
                        btn.disabled = false;
                        btn.classList.remove('opacity-50', 'cursor-not-allowed');
                    });
                }, 10000);
            });
        }
    });

    // Resetear banderas al cargar/cambiar de página
    window.addEventListener('pageshow', function() {
        estaEnviando = false;
        const cargador = document.getElementById('cargador-global');
        if (cargador) cargador.style.display = 'none';
    });
})();
