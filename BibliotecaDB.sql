USE BibliotecaDB;
GO

SELECT 
    L.IdLibro,
    L.CodigoISBN,
    L.Titulo,
    L.AnioPublicacion,
    L.Stock,
    CONCAT(A.Nombre, ' ', A.Apellido) AS NombreAutor,
    E.Nombre AS NombreEditorial
FROM Libros L
INNER JOIN Autores A ON L.IdAutor = A.IdAutor
INNER JOIN Editoriales E ON L.IdEditorial = E.IdEditorial;
GO