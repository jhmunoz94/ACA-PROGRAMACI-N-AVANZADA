# Sistema de Gestión de Biblioteca - C# & SQL Server

Aplicación de escritorio para la administración de bibliotecas escolares desarrollada bajo una arquitectura en 3 capas con Windows Forms, .NET y Microsoft SQL Server.

## 🚀 Características
- **CRUD Completo:** Gestión de Libros, Autores, Editoriales y Usuarios.
- **Control de Préstamos y Devoluciones:** Validación en tiempo real del inventario (Stock).
- **Módulo de Reportes:** Consultas consolidadas de préstamos activos, devueltos, catálogo de libros y usuarios registrados.
- **Arquitectura en 3 Capas:** Separación lógica entre Capa de Presentación, Lógica de Negocio, Acceso a Datos y Entidades.

## 🛠️ Tecnologías Utilizadas
- C# (.NET Framework / .NET Core para Windows Forms)
- Microsoft SQL Server
- ADO.NET (consultas parametrizadas con SqlCommand)
- Git & GitHub para control de versiones

## ⚙️ Instrucciones de Instalación y Ejecución
1. Abrir **SQL Server Management Studio (SSMS)**.
2. Abrir y ejecutar el archivo `Script_Biblioteca.sql` incluido en la raíz de este proyecto para crear la base de datos `BibliotecaDB` y sus datos de prueba.
3. Abrir la solución (`.sln`) en **Visual Studio 2022**.
4. En caso de ser necesario, verificar la cadena de conexión en la clase `Conexion.cs` de la capa de datos.
5. Compilar la solución (`Ctrl + Mayús + B`) y presionar **F5** para iniciar la aplicación.