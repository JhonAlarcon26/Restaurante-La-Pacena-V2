USE [BD_la_pacena]
GO

IF OBJECT_ID('dbo.detalle_avastecimiento_plato', 'U') IS NOT NULL
    DROP TABLE dbo.detalle_avastecimiento_plato;

IF OBJECT_ID('dbo.avastecimiento_plato', 'U') IS NOT NULL
    DROP TABLE dbo.avastecimiento_plato;

IF OBJECT_ID('dbo.stock_plato', 'U') IS NOT NULL
    DROP TABLE dbo.stock_plato;

IF OBJECT_ID('dbo.receta', 'U') IS NOT NULL
    DROP TABLE dbo.receta;

IF OBJECT_ID('dbo.__EFMigrationsHistory', 'U') IS NOT NULL
    DROP TABLE dbo.__EFMigrationsHistory;

CREATE TABLE [dbo].[avastecimiento_plato](
    [id_avastecimiento_plato] [int] IDENTITY(1,1) NOT NULL,
    [id_proveedor] [int] NOT NULL,
    [id_usuario] [int] NOT NULL,
    [fecha_avastecimiento] [datetime] NULL DEFAULT (getdate()),
    [nro_factura_proveedor] [varchar](50) NULL,
    [total_avastecimiento] [decimal](10, 2) NOT NULL,
PRIMARY KEY CLUSTERED 
(
    [id_avastecimiento_plato] ASC
));

CREATE TABLE [dbo].[detalle_avastecimiento_plato](
    [id_detalle_avastecimiento_plato] [int] IDENTITY(1,1) NOT NULL,
    [id_avastecimiento_plato] [int] NOT NULL,
    [id_plato] [int] NOT NULL,
    [cantidad] [int] NOT NULL,
    [precio_unitario] [decimal](10, 2) NOT NULL,
    [subtotal] [decimal](10, 2) NOT NULL,
PRIMARY KEY CLUSTERED 
(
    [id_detalle_avastecimiento_plato] ASC
));

CREATE TABLE [dbo].[stock_plato](
    [id_stock_plato] [int] IDENTITY(1,1) NOT NULL,
    [id_plato] [int] NOT NULL,
    [cantidad_actual] [decimal](10, 3) NOT NULL DEFAULT (0),
    [cantidad_minima] [decimal](10, 3) NOT NULL DEFAULT (5),
    [ultima_actualizacion] [datetime] NULL DEFAULT (getdate()),
PRIMARY KEY CLUSTERED 
(
    [id_stock_plato] ASC
));

ALTER TABLE [dbo].[avastecimiento_plato] WITH CHECK ADD CONSTRAINT [FK_AvasP_Proveedor] FOREIGN KEY([id_proveedor])
REFERENCES [dbo].[proveedor] ([id_proveedor]);

ALTER TABLE [dbo].[avastecimiento_plato] WITH CHECK ADD CONSTRAINT [FK_AvasP_Usuario] FOREIGN KEY([id_usuario])
REFERENCES [dbo].[usuario] ([id_usuario]);

ALTER TABLE [dbo].[detalle_avastecimiento_plato] WITH CHECK ADD CONSTRAINT [FK_DetalleAP_Avaste] FOREIGN KEY([id_avastecimiento_plato])
REFERENCES [dbo].[avastecimiento_plato] ([id_avastecimiento_plato]) ON DELETE CASCADE;

ALTER TABLE [dbo].[detalle_avastecimiento_plato] WITH CHECK ADD CONSTRAINT [FK_DetalleAP_Plato] FOREIGN KEY([id_plato])
REFERENCES [dbo].[plato] ([id_plato]);

ALTER TABLE [dbo].[stock_plato] WITH CHECK ADD CONSTRAINT [FK_StockP_Plato] FOREIGN KEY([id_plato])
REFERENCES [dbo].[plato] ([id_plato]);
