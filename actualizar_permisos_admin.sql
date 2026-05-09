USE BD_la_pacena;
GO

-- Limpiar permisos actuales del admin
DELETE FROM permisos WHERE id_rol = 1;
GO

-- Insertar absolutamente todos los permisos
INSERT INTO permisos (id_rol, nombre, estado) VALUES
(1, 'VerUsuarios', 1), (1, 'CrearUsuarios', 1), (1, 'EditarUsuarios', 1), (1, 'EliminarUsuarios', 1),
(1, 'VerRoles', 1), (1, 'CrearRoles', 1), (1, 'EditarRoles', 1), (1, 'EliminarRoles', 1),
(1, 'AsignarPermisos', 1),
(1, 'VerPermisos', 1), (1, 'CrearPermisos', 1), (1, 'EditarPermisos', 1), (1, 'EliminarPermisos', 1),
(1, 'VerTipoProducto', 1), (1, 'CrearTipoProducto', 1), (1, 'EditarTipoProducto', 1), (1, 'EliminarTipoProducto', 1),
(1, 'VerUnidadMedida', 1), (1, 'CrearUnidadMedida', 1), (1, 'EditarUnidadMedida', 1), (1, 'EliminarUnidadMedida', 1),
(1, 'VerTipoPago', 1), (1, 'CrearTipoPago', 1), (1, 'EditarTipoPago', 1), (1, 'EliminarTipoPago', 1),
(1, 'VerCategorias', 1), (1, 'CrearCategorias', 1), (1, 'EditarCategorias', 1), (1, 'EliminarCategorias', 1),
(1, 'VerSucursales', 1), (1, 'CrearSucursales', 1), (1, 'EditarSucursales', 1), (1, 'EliminarSucursales', 1),
(1, 'VerProveedores', 1), (1, 'CrearProveedores', 1), (1, 'EditarProveedores', 1), (1, 'EliminarProveedores', 1),
(1, 'VerInsumos', 1), (1, 'CrearInsumos', 1), (1, 'EditarInsumos', 1), (1, 'EliminarInsumos', 1),
(1, 'VerProductos', 1), (1, 'CrearProductos', 1), (1, 'EditarProductos', 1), (1, 'EliminarProductos', 1),
(1, 'VerCompraProducto', 1), (1, 'CrearCompraProducto', 1), (1, 'EliminarCompraProducto', 1),
(1, 'VerCompraInsumo', 1), (1, 'CrearCompraInsumo', 1), (1, 'EliminarCompraInsumo', 1),
(1, 'VerClientes', 1), (1, 'CrearClientes', 1), (1, 'EditarClientes', 1), (1, 'EliminarClientes', 1),
(1, 'VerPlatos', 1), (1, 'CrearPlatos', 1), (1, 'EditarPlatos', 1), (1, 'EliminarPlatos', 1),
(1, 'VerAbastecimientos', 1), (1, 'CrearAbastecimientos', 1), (1, 'EliminarAbastecimientos', 1),
(1, 'VerVentas', 1), (1, 'CrearVentas', 1), (1, 'EliminarVentas', 1),
(1, 'VerStock', 1), (1, 'VerStockPlatos', 1), (1, 'VerKardex', 1),
(1, 'VerReporteCompraInsumo', 1), (1, 'VerReporteCompraProducto', 1), (1, 'VerReporteVentas', 1), (1, 'VerReporteStock', 1);
GO
