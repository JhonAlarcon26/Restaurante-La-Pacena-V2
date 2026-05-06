CREATE TABLE [categoria_plato] (
    [id_categoria] int NOT NULL IDENTITY,
    [nombre] varchar(100) NOT NULL,
    [impresora_destino] varchar(100) NULL,
    [estado] bit NULL DEFAULT CAST(1 AS bit),
    CONSTRAINT [PK__categori__CD54A221] PRIMARY KEY ([id_categoria])
);
GO


CREATE TABLE [cliente] (
    [id_cliente] int NOT NULL IDENTITY,
    [razon_social] varchar(150) NOT NULL,
    [nit_carnet] varchar(50) NULL,
    [telefono] varchar(50) NULL,
    [email] varchar(100) NULL,
    CONSTRAINT [PK__cliente__677F38F5] PRIMARY KEY ([id_cliente])
);
GO


CREATE TABLE [permisos] (
    [id_permiso] int NOT NULL IDENTITY,
    [nombre] varchar(150) NOT NULL,
    [descripcion] varchar(250) NULL,
    [estado] bit NULL DEFAULT CAST(1 AS bit),
    CONSTRAINT [PK__permisos__2D9638E7] PRIMARY KEY ([id_permiso])
);
GO


CREATE TABLE [proveedor] (
    [id_proveedor] int NOT NULL IDENTITY,
    [nombre_empresa] varchar(150) NOT NULL,
    [contacto_nombre] varchar(150) NULL,
    [telefono] varchar(50) NULL,
    [email] varchar(100) NULL,
    [estado] bit NULL DEFAULT CAST(1 AS bit),
    [nit_carnet] varchar(50) NULL,
    CONSTRAINT [PK__proveedo__8D0CE3DF] PRIMARY KEY ([id_proveedor])
);
GO


CREATE TABLE [rol] (
    [id_rol] int NOT NULL IDENTITY,
    [nombre] varchar(100) NOT NULL,
    [descripcion] varchar(255) NULL,
    [estado] bit NULL DEFAULT CAST(1 AS bit),
    CONSTRAINT [PK__rol__6ABCB5E0] PRIMARY KEY ([id_rol])
);
GO


CREATE TABLE [sucursal] (
    [id_sucursal] int NOT NULL IDENTITY,
    [nombre] varchar(150) NOT NULL,
    [direccion] varchar(250) NOT NULL,
    [ciudad] varchar(100) NULL,
    [telefono] varchar(50) NULL,
    [estado] bit NULL DEFAULT CAST(1 AS bit),
    CONSTRAINT [PK__sucursal__32A559C3] PRIMARY KEY ([id_sucursal])
);
GO


CREATE TABLE [tipo_de_pago] (
    [id_tipo_pago] int NOT NULL IDENTITY,
    [nombre] varchar(50) NOT NULL,
    [estado] bit NULL DEFAULT CAST(1 AS bit),
    CONSTRAINT [PK__tipo_de___6C2C5535] PRIMARY KEY ([id_tipo_pago])
);
GO


CREATE TABLE [tipo_producto] (
    [id_tipo_producto] int NOT NULL IDENTITY,
    [nombre] varchar(150) NOT NULL,
    [descripcion] varchar(255) NULL,
    [estado] bit NULL DEFAULT CAST(1 AS bit),
    CONSTRAINT [PK__tipo_pro__3D5E7E2C] PRIMARY KEY ([id_tipo_producto])
);
GO


CREATE TABLE [unidad_de_medida] (
    [id_unidad] int NOT NULL IDENTITY,
    [nombre] varchar(50) NOT NULL,
    [abreviatura] varchar(10) NOT NULL,
    CONSTRAINT [PK__unidad_d__2D93148F] PRIMARY KEY ([id_unidad])
);
GO


CREATE TABLE [plato] (
    [id_plato] int NOT NULL IDENTITY,
    [nombre] varchar(150) NOT NULL,
    [descripcion_carta] varchar(255) NULL,
    [precio_venta] decimal(10,2) NOT NULL,
    [id_categoria] int NOT NULL,
    [estado] bit NULL DEFAULT CAST(1 AS bit),
    CONSTRAINT [PK__plato__6F37C223] PRIMARY KEY ([id_plato]),
    CONSTRAINT [FK_Plato_Categoria] FOREIGN KEY ([id_categoria]) REFERENCES [categoria_plato] ([id_categoria])
);
GO


CREATE TABLE [rol_permiso] (
    [id_rol] int NOT NULL,
    [id_permiso] int NOT NULL,
    [id_rol_permiso] int NOT NULL,
    CONSTRAINT [PK__rol_perm__D047F817] PRIMARY KEY ([id_rol], [id_permiso]),
    CONSTRAINT [FK_RolPermiso_Permiso] FOREIGN KEY ([id_permiso]) REFERENCES [permisos] ([id_permiso]),
    CONSTRAINT [FK_RolPermiso_Rol] FOREIGN KEY ([id_rol]) REFERENCES [rol] ([id_rol])
);
GO


CREATE TABLE [usuario] (
    [id_usuario] int NOT NULL IDENTITY,
    [id_rol] int NOT NULL,
    [id_sucursal] int NOT NULL,
    [nombre] varchar(150) NOT NULL,
    [username] varchar(50) NOT NULL,
    [password] varchar(255) NOT NULL,
    [estado] bit NULL DEFAULT CAST(1 AS bit),
    CONSTRAINT [PK__usuario__4E510534] PRIMARY KEY ([id_usuario]),
    CONSTRAINT [FK_Usuario_Rol] FOREIGN KEY ([id_rol]) REFERENCES [rol] ([id_rol]),
    CONSTRAINT [FK_Usuario_Sucursal] FOREIGN KEY ([id_sucursal]) REFERENCES [sucursal] ([id_sucursal])
);
GO


CREATE TABLE [insumo] (
    [id_insumo] int NOT NULL IDENTITY,
    [nombre] varchar(150) NOT NULL,
    [id_unidad] int NOT NULL,
    [costo_promedio] decimal(10,2) NULL DEFAULT 0.0,
    [estado] bit NULL DEFAULT CAST(1 AS bit),
    CONSTRAINT [PK__insumo__DF9C369B] PRIMARY KEY ([id_insumo]),
    CONSTRAINT [FK_Insumo_Unidad] FOREIGN KEY ([id_unidad]) REFERENCES [unidad_de_medida] ([id_unidad])
);
GO


CREATE TABLE [producto] (
    [id_producto] int NOT NULL IDENTITY,
    [nombre] varchar(150) NOT NULL,
    [codigo_barras] varchar(100) NULL,
    [id_tipo_producto] int NOT NULL,
    [precio_compra_actual] decimal(10,2) NULL DEFAULT 0.0,
    [precio_venta] decimal(10,2) NOT NULL,
    [estado] bit NULL DEFAULT CAST(1 AS bit),
    [id_unidad] int NOT NULL,
    CONSTRAINT [PK__producto__1413F405] PRIMARY KEY ([id_producto]),
    CONSTRAINT [FK_Producto_TipoProducto] FOREIGN KEY ([id_tipo_producto]) REFERENCES [tipo_producto] ([id_tipo_producto]),
    CONSTRAINT [FK_Producto_Unidad] FOREIGN KEY ([id_unidad]) REFERENCES [unidad_de_medida] ([id_unidad])
);
GO


CREATE TABLE [stock_plato] (
    [id_stock_plato] int NOT NULL IDENTITY,
    [id_plato] int NOT NULL,
    [cantidad_actual] decimal(10,3) NOT NULL DEFAULT 0.0,
    [cantidad_minima] decimal(10,3) NOT NULL DEFAULT 5.0,
    [ultima_actualizacion] datetime2 NULL DEFAULT ((getdate())),
    CONSTRAINT [PK_stock_plato] PRIMARY KEY ([id_stock_plato]),
    CONSTRAINT [FK_StockP_Plato] FOREIGN KEY ([id_plato]) REFERENCES [plato] ([id_plato])
);
GO


CREATE TABLE [avastecimiento_plato] (
    [id_avastecimiento_plato] int NOT NULL IDENTITY,
    [id_usuario] int NOT NULL,
    [fecha_avastecimiento] datetime2 NULL DEFAULT ((getdate())),
    [nro_referencia] varchar(50) NULL,
    [total_avastecimiento] decimal(10,2) NOT NULL,
    CONSTRAINT [PK_avastecimiento_plato] PRIMARY KEY ([id_avastecimiento_plato]),
    CONSTRAINT [FK_AvasP_Usuario] FOREIGN KEY ([id_usuario]) REFERENCES [usuario] ([id_usuario])
);
GO


CREATE TABLE [compra_insumo] (
    [id_compra] int NOT NULL IDENTITY,
    [id_proveedor] int NOT NULL,
    [id_usuario] int NOT NULL,
    [fecha_compra] datetime2 NULL DEFAULT ((getdate())),
    [nro_referencia] varchar(50) NULL,
    [total_compra] decimal(10,2) NOT NULL,
    [estado] varchar(20) NULL DEFAULT 'COMPLETADA',
    CONSTRAINT [PK__compra__C4BAA604] PRIMARY KEY ([id_compra]),
    CONSTRAINT [FK_Compra_Proveedor] FOREIGN KEY ([id_proveedor]) REFERENCES [proveedor] ([id_proveedor]),
    CONSTRAINT [FK_Compra_Usuario] FOREIGN KEY ([id_usuario]) REFERENCES [usuario] ([id_usuario])
);
GO


CREATE TABLE [compra_producto] (
    [id_avastecimiento] int NOT NULL IDENTITY,
    [id_proveedor] int NOT NULL,
    [id_usuario] int NOT NULL,
    [fecha_avastecimiento] datetime2 NULL DEFAULT ((getdate())),
    [nro_referencia] varchar(50) NULL,
    [total_avastecimiento] decimal(10,2) NOT NULL,
    CONSTRAINT [PK__avasteci__53D4A869] PRIMARY KEY ([id_avastecimiento]),
    CONSTRAINT [FK_Avas_Proveedor] FOREIGN KEY ([id_proveedor]) REFERENCES [proveedor] ([id_proveedor]),
    CONSTRAINT [FK_Avas_Usuario] FOREIGN KEY ([id_usuario]) REFERENCES [usuario] ([id_usuario])
);
GO


CREATE TABLE [ventas] (
    [id_venta] int NOT NULL IDENTITY,
    [id_usuario] int NOT NULL,
    [id_cliente] int NOT NULL,
    [id_sucursal] int NOT NULL,
    [fecha_venta] datetime2 NULL DEFAULT ((getdate())),
    [nro_mesa] int NULL,
    [total_venta] decimal(10,2) NOT NULL,
    [id_tipo_pago] int NULL,
    [estado] varchar(20) NULL DEFAULT 'PAGADA',
    CONSTRAINT [PK__ventas__4E2B9635] PRIMARY KEY ([id_venta]),
    CONSTRAINT [FK_Venta_Cliente] FOREIGN KEY ([id_cliente]) REFERENCES [cliente] ([id_cliente]) ON DELETE CASCADE,
    CONSTRAINT [FK_Venta_Pago] FOREIGN KEY ([id_tipo_pago]) REFERENCES [tipo_de_pago] ([id_tipo_pago]),
    CONSTRAINT [FK_Venta_Sucursal] FOREIGN KEY ([id_sucursal]) REFERENCES [sucursal] ([id_sucursal]),
    CONSTRAINT [FK_Venta_Usuario] FOREIGN KEY ([id_usuario]) REFERENCES [usuario] ([id_usuario])
);
GO


CREATE TABLE [kardex_general] (
    [id_kardex] bigint NOT NULL IDENTITY,
    [fecha] datetime2 NULL DEFAULT ((getdate())),
    [tipo_movimiento] varchar(50) NOT NULL,
    [id_insumo] int NULL,
    [id_producto] int NULL,
    [id_plato] int NULL,
    [cantidad] decimal(10,3) NOT NULL,
    [costo_unitario] decimal(18,2) NULL,
    [id_referencia] int NULL,
    CONSTRAINT [PK__kardex_g__463A29B1] PRIMARY KEY ([id_kardex]),
    CONSTRAINT [FK_Kardex_Insumo] FOREIGN KEY ([id_insumo]) REFERENCES [insumo] ([id_insumo]),
    CONSTRAINT [FK_Kardex_Plato] FOREIGN KEY ([id_plato]) REFERENCES [plato] ([id_plato]),
    CONSTRAINT [FK_Kardex_Producto] FOREIGN KEY ([id_producto]) REFERENCES [producto] ([id_producto])
);
GO


CREATE TABLE [stock] (
    [id_stock] int NOT NULL IDENTITY,
    [id_insumo] int NULL,
    [id_producto] int NULL,
    [cantidad_actual] decimal(10,3) NOT NULL DEFAULT 0.0,
    [cantidad_minima] decimal(10,3) NOT NULL DEFAULT 5.0,
    [ultima_actualizacion] datetime2 NULL DEFAULT ((getdate())),
    CONSTRAINT [PK__stock__288BA44D] PRIMARY KEY ([id_stock]),
    CONSTRAINT [FK_Stock_Insumo] FOREIGN KEY ([id_insumo]) REFERENCES [insumo] ([id_insumo]),
    CONSTRAINT [FK_Stock_Producto] FOREIGN KEY ([id_producto]) REFERENCES [producto] ([id_producto])
);
GO


CREATE TABLE [detalle_avastecimiento_plato] (
    [id_detalle_avastecimiento_plato] int NOT NULL IDENTITY,
    [id_avastecimiento_plato] int NOT NULL,
    [id_plato] int NOT NULL,
    [cantidad] int NOT NULL,
    [precio_unitario] decimal(10,2) NOT NULL,
    [subtotal] decimal(10,2) NOT NULL,
    CONSTRAINT [PK_detalle_avastecimiento_plato] PRIMARY KEY ([id_detalle_avastecimiento_plato]),
    CONSTRAINT [FK_DetalleAP_Avaste] FOREIGN KEY ([id_avastecimiento_plato]) REFERENCES [avastecimiento_plato] ([id_avastecimiento_plato]) ON DELETE CASCADE,
    CONSTRAINT [FK_DetalleAP_Plato] FOREIGN KEY ([id_plato]) REFERENCES [plato] ([id_plato])
);
GO


CREATE TABLE [detalle_compra_insumo] (
    [id_detalle_compra] int NOT NULL IDENTITY,
    [id_compra] int NOT NULL,
    [id_insumo] int NOT NULL,
    [cantidad] decimal(10,3) NOT NULL,
    [precio_unitario] decimal(10,2) NOT NULL,
    [subtotal] decimal(10,2) NOT NULL,
    CONSTRAINT [PK__detalle___768CE6A5] PRIMARY KEY ([id_detalle_compra]),
    CONSTRAINT [FK_DetalleC_Compra] FOREIGN KEY ([id_compra]) REFERENCES [compra_insumo] ([id_compra]),
    CONSTRAINT [FK_DetalleC_Insumo] FOREIGN KEY ([id_insumo]) REFERENCES [insumo] ([id_insumo])
);
GO


CREATE TABLE [detalle_compra_producto] (
    [id_detalle_avastecimiento] int NOT NULL IDENTITY,
    [id_avastecimiento] int NOT NULL,
    [id_producto] int NOT NULL,
    [cantidad] int NOT NULL,
    [precio_unitario] decimal(10,2) NOT NULL,
    [subtotal] decimal(10,2) NOT NULL,
    CONSTRAINT [PK__detalle___09384E3A] PRIMARY KEY ([id_detalle_avastecimiento]),
    CONSTRAINT [FK_DetalleA_Avaste] FOREIGN KEY ([id_avastecimiento]) REFERENCES [compra_producto] ([id_avastecimiento]),
    CONSTRAINT [FK_DetalleA_Producto] FOREIGN KEY ([id_producto]) REFERENCES [producto] ([id_producto])
);
GO


CREATE TABLE [detalle_venta] (
    [id_detalle_venta] int NOT NULL IDENTITY,
    [id_venta] int NOT NULL,
    [id_plato] int NULL,
    [id_producto] int NULL,
    [cantidad] int NOT NULL,
    [precio_unitario] decimal(10,2) NOT NULL,
    [subtotal] decimal(10,2) NOT NULL,
    CONSTRAINT [PK__detalle___B3000AF5] PRIMARY KEY ([id_detalle_venta]),
    CONSTRAINT [FK_DetalleV_Plato] FOREIGN KEY ([id_plato]) REFERENCES [plato] ([id_plato]),
    CONSTRAINT [FK_DetalleV_Producto] FOREIGN KEY ([id_producto]) REFERENCES [producto] ([id_producto]),
    CONSTRAINT [FK_DetalleV_Venta] FOREIGN KEY ([id_venta]) REFERENCES [ventas] ([id_venta])
);
GO


CREATE INDEX [IX_avastecimiento_plato_id_usuario] ON [avastecimiento_plato] ([id_usuario]);
GO


CREATE INDEX [IX_compra_insumo_id_proveedor] ON [compra_insumo] ([id_proveedor]);
GO


CREATE INDEX [IX_compra_insumo_id_usuario] ON [compra_insumo] ([id_usuario]);
GO


CREATE INDEX [IX_compra_producto_id_proveedor] ON [compra_producto] ([id_proveedor]);
GO


CREATE INDEX [IX_compra_producto_id_usuario] ON [compra_producto] ([id_usuario]);
GO


CREATE INDEX [IX_detalle_avastecimiento_plato_id_avastecimiento_plato] ON [detalle_avastecimiento_plato] ([id_avastecimiento_plato]);
GO


CREATE INDEX [IX_detalle_avastecimiento_plato_id_plato] ON [detalle_avastecimiento_plato] ([id_plato]);
GO


CREATE INDEX [IX_detalle_compra_insumo_id_compra] ON [detalle_compra_insumo] ([id_compra]);
GO


CREATE INDEX [IX_detalle_compra_insumo_id_insumo] ON [detalle_compra_insumo] ([id_insumo]);
GO


CREATE INDEX [IX_detalle_compra_producto_id_avastecimiento] ON [detalle_compra_producto] ([id_avastecimiento]);
GO


CREATE INDEX [IX_detalle_compra_producto_id_producto] ON [detalle_compra_producto] ([id_producto]);
GO


CREATE INDEX [IX_detalle_venta_id_plato] ON [detalle_venta] ([id_plato]);
GO


CREATE INDEX [IX_detalle_venta_id_producto] ON [detalle_venta] ([id_producto]);
GO


CREATE INDEX [IX_detalle_venta_id_venta] ON [detalle_venta] ([id_venta]);
GO


CREATE INDEX [IX_insumo_id_unidad] ON [insumo] ([id_unidad]);
GO


CREATE INDEX [IX_kardex_general_id_insumo] ON [kardex_general] ([id_insumo]);
GO


CREATE INDEX [IX_kardex_general_id_plato] ON [kardex_general] ([id_plato]);
GO


CREATE INDEX [IX_kardex_general_id_producto] ON [kardex_general] ([id_producto]);
GO


CREATE INDEX [IX_plato_id_categoria] ON [plato] ([id_categoria]);
GO


CREATE INDEX [IX_producto_id_tipo_producto] ON [producto] ([id_tipo_producto]);
GO


CREATE INDEX [IX_producto_id_unidad] ON [producto] ([id_unidad]);
GO


CREATE INDEX [IX_rol_permiso_id_permiso] ON [rol_permiso] ([id_permiso]);
GO


CREATE INDEX [IX_stock_id_insumo] ON [stock] ([id_insumo]);
GO


CREATE INDEX [IX_stock_id_producto] ON [stock] ([id_producto]);
GO


CREATE INDEX [IX_stock_plato_id_plato] ON [stock_plato] ([id_plato]);
GO


CREATE INDEX [IX_usuario_id_rol] ON [usuario] ([id_rol]);
GO


CREATE INDEX [IX_usuario_id_sucursal] ON [usuario] ([id_sucursal]);
GO


CREATE INDEX [IX_ventas_id_cliente] ON [ventas] ([id_cliente]);
GO


CREATE INDEX [IX_ventas_id_sucursal] ON [ventas] ([id_sucursal]);
GO


CREATE INDEX [IX_ventas_id_tipo_pago] ON [ventas] ([id_tipo_pago]);
GO


CREATE INDEX [IX_ventas_id_usuario] ON [ventas] ([id_usuario]);
GO


