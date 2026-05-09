USE BD_la_pacena;
GO

-- Limpiar permisos actuales para el id_rol = 1
DELETE FROM permisos WHERE id_rol = 1;

-- Insertar exactamente las 24 políticas como permisos para id_rol = 1
INSERT INTO permisos (nombre, descripcion, estado, id_rol) VALUES 
('VerUsuarios', 'Ver módulo Usuarios', 1, 1),
('VerRoles', 'Ver módulo Roles', 1, 1),
('VerPermisos', 'Ver módulo Permisos', 1, 1),
('VerTipoProducto', 'Ver módulo Tipo de Producto', 1, 1),
('VerUnidadMedida', 'Ver módulo Unidad de Medida', 1, 1),
('VerTipoPago', 'Ver módulo Tipo de Pago', 1, 1),
('VerCategorias', 'Ver módulo Categoria Plato', 1, 1),
('VerSucursales', 'Ver módulo Sucursales', 1, 1),
('VerProveedores', 'Ver módulo Proveedor', 1, 1),
('VerInsumos', 'Ver módulo Insumo', 1, 1),
('VerProductos', 'Ver módulo Producto', 1, 1),
('VerCompraProducto', 'Ver módulo Compra Producto', 1, 1),
('VerCompraInsumo', 'Ver módulo Compra Insumo', 1, 1),
('VerClientes', 'Ver módulo Cliente', 1, 1),
('VerPlatos', 'Ver módulo Platos', 1, 1),
('VerAbastecimientos', 'Ver módulo Abastecimiento Platos', 1, 1),
('VerVentas', 'Ver módulo Ventas', 1, 1),
('VerStock', 'Ver módulo Stock', 1, 1),
('VerStockPlatos', 'Ver módulo Stock Platos', 1, 1),
('VerKardex', 'Ver módulo Kardex General', 1, 1),
('VerReporteCompraInsumo', 'Ver módulo Reporte compra insumo', 1, 1),
('VerReporteCompraProducto', 'Ver módulo Reporte compra producto', 1, 1),
('VerReporteVentas', 'Ver módulo Reporte Venta', 1, 1),
('VerReporteStock', 'Ver módulo Reporte Stock', 1, 1);
GO
