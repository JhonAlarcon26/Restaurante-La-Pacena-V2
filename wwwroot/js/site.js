function formatearNumero(num) {
    if (num === null || num === undefined) return '0';
    const n = parseFloat(num);
    if (isNaN(n)) return '0';
    return Number.isInteger(n) ? n.toString() : n.toFixed(2).replace(/\.?0+$/, "");
}
