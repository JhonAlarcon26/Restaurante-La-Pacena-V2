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

    input.addEventListener('input', function() {
        const term = input.value.toLowerCase().trim();
        const rows = tbody.getElementsByTagName('tr');
        
        for (let i = 0; i < rows.length; i++) {
            const cells = rows[i].getElementsByTagName('td');
            let match = false;
            
            for (let j = 0; j < cells.length; j++) {
                if (cells[j].textContent.toLowerCase().includes(term)) {
                    match = true;
                    break;
                }
            }
            
            rows[i].style.display = match ? '' : 'none';
        }
    });
}
