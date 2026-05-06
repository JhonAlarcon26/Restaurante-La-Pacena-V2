
document.addEventListener('DOMContentLoaded', function () {
    var form = document.querySelector('form');
    if (form) {
        form.addEventListener('submit', function (e) {
            var nombre = document.getElementById('Nombre').value.trim();
            var unidad = document.getElementById('IdUnidad').value;
            var stockMinimo = document.getElementById('StockMinimo').value;
            var costoPromedio = document.getElementById('CostoPromedio').value;

            if (!nombre || nombre.length === 0) {
                e.preventDefault();
                alert('❌ El nombre del insumo es obligatorio');
                document.getElementById('Nombre').focus();
                return false;
            }

            if (!unidad || unidad === '') {
                e.preventDefault();
                alert('❌ Debe seleccionar una unidad de medida');
                document.getElementById('IdUnidad').focus();
                return false;
            }

            if (stockMinimo && parseFloat(stockMinimo) < 0) {
                e.preventDefault();
                alert('❌ El stock mínimo no puede ser negativo');
                document.getElementById('StockMinimo').focus();
                return false;
            }

            if (costoPromedio && parseFloat(costoPromedio) < 0) {
                e.preventDefault();
                alert('❌ El costo promedio no puede ser negativo');
                document.getElementById('CostoPromedio').focus();
                return false;
            }

            return true;
        });
    }
});
