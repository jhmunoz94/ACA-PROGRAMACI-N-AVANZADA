-- =====================================================================
-- PROYECTO: SISTEMA DE GESTIÓN DE BIBLIOTECA
-- ASIGNATURA: PROGRAMACIÓN AVANZADA
-- ENTREGABLE 14.3: SCRIPT DDL (ESTRUCTURA) Y DML (DATOS DE PRUEBA)
-- =====================================================================

CREATE DATABASE BibliotecaDB;
GO

USE BibliotecaDB;
GO

-- 1. TABLA AUTORES
CREATE TABLE Autores (
    IdAutor INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL,
    Apellido VARCHAR(100) NOT NULL,
    Nacionalidad VARCHAR(50) NULL
);
GO

-- 2. TABLA EDITORIALES
CREATE TABLE Editoriales (
    IdEditorial INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL UNIQUE,
    Pais VARCHAR(50) NULL
);
GO

-- 3. TABLA USUARIOS
CREATE TABLE Usuarios (
    IdUsuario INT IDENTITY(1,1) PRIMARY KEY,
    Documento VARCHAR(20) NOT NULL UNIQUE,
    Nombre VARCHAR(100) NOT NULL,
    Apellido VARCHAR(100) NOT NULL,
    Telefono VARCHAR(20) NULL,
    Correo VARCHAR(100) NULL
);
GO

-- 4. TABLA LIBROS
CREATE TABLE Libros (
    IdLibro INT IDENTITY(1,1) PRIMARY KEY,
    CodigoISBN VARCHAR(20) NOT NULL UNIQUE,
    Titulo VARCHAR(200) NOT NULL,
    AnioPublicacion INT NOT NULL,
    Stock INT NOT NULL DEFAULT 1,
    IdAutor INT NOT NULL,
    IdEditorial INT NOT NULL,
    CONSTRAINT FK_Libros_Autores FOREIGN KEY (IdAutor) REFERENCES Autores(IdAutor),
    CONSTRAINT FK_Libros_Editoriales FOREIGN KEY (IdEditorial) REFERENCES Editoriales(IdEditorial)
);
GO

-- 5. TABLA PRESTAMOS
CREATE TABLE Prestamos (
    IdPrestamo INT IDENTITY(1,1) PRIMARY KEY,
    IdUsuario INT NOT NULL,
    IdLibro INT NOT NULL,
    FechaPrestamo DATETIME NOT NULL DEFAULT GETDATE(),
    FechaDevolucion DATETIME NULL,
    Estado VARCHAR(20) NOT NULL DEFAULT 'Prestado',
    CONSTRAINT FK_Prestamos_Usuarios FOREIGN KEY (IdUsuario) REFERENCES Usuarios(IdUsuario),
    CONSTRAINT FK_Prestamos_Libros FOREIGN KEY (IdLibro) REFERENCES Libros(IdLibro)
);
GO

-- =====================================================================
-- INSERCIÓN DE DATOS DE PRUEBA INICIALES
-- =====================================================================

-- Autores
INSERT INTO Autores (Nombre, Apellido, Nacionalidad) VALUES
('Gabriel', 'García Márquez', 'Colombiana'),
('Mario', 'Vargas Llosa', 'Peruana'),
('Isabel', 'Allende', 'Chilena'),
('Julio', 'Cortázar', 'Argentina');

-- Editoriales
INSERT INTO Editoriales (Nombre, Pais) VALUES
('Editorial Sudamericana', 'Argentina'),
('Alfaguara', 'España'),
('Planeta', 'Colombia');

-- Usuarios
INSERT INTO Usuarios (Documento, Nombre, Apellido, Telefono, Correo) VALUES
('80457351', 'Carlos', 'Rodríguez', '3118467514', 'carlos.rodriguez@gmail.com'),
('52478354', 'María', 'Gómez', '3186542581', 'maria.gomez@outlook.es'),
('1012407155', 'Jorge', 'Laverde Cruz', '3105846145', 'laverde-jorge@gmail.com');

-- Libros
INSERT INTO Libros (CodigoISBN, Titulo, AnioPublicacion, Stock, IdAutor, IdEditorial) VALUES
('9780307474728', 'Cien años de soledad', 1967, 5, 1, 3),
('9788437604947', 'El coronel no tiene quien le escriba', 1961, 8, 1, 2),
('9788420471839', 'Rayuela', 1963, 6, 4, 2);

-- Préstamos iniciales
INSERT INTO Prestamos (IdUsuario, IdLibro, FechaPrestamo, FechaDevolucion, Estado) VALUES
(1, 1, '2026-11-10 12:14:00', '2026-11-10 16:04:00', 'Devuelto'),
(2, 1, '2026-11-10 16:04:00', '2026-11-10 16:20:00', 'Devuelto'),
(3, 2, GETDATE(), NULL, 'Prestado');
GO