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
