---
trigger: always_on
---


Act as a senior software architect and university-level instructor specialized in ASP.NET MVC using Visual Studio Community.

I want you to generate a complete web project using the Model–View–Controller (MVC) pattern with the following MANDATORY rules:

1. LANGUAGE REQUIREMENTS:
   - Absolutely EVERYTHING in the CODE must be in SPANISH:
     - Project name
     - Folder names
     - Controllers (e.g., ClienteController, ProductoController)
     - Models (e.g., Cliente, Producto, Venta)
     - Methods/actions (e.g., ListarClientes, RegistrarCliente, EditarProducto, EliminarProducto)
     - Variables
     - Class propertiess
     - Code comments
     - UI text and messages
     - View names (e.g., RegistrarCliente.cshtml)
     - Routes (URLs) based on Spanish names
     - Database table names and field names
   - Do NOT use English identifiers in the code. Only Spanish identifiers are allowed.

2. CLEAR MVC STRUCTURE:
   - Properly separate:
     - Models
     - Controllers
     - Views
   - Briefly explain the responsibility of each layer.

3. MODELS (DATABASE STRUCTURE):
   - Create domain models in Spanish, for example:
     - Cliente (IdCliente, NombreCompleto, CorreoElectronico, Telefono)
     - Producto (IdProducto, NombreProducto, Precio, Stock)
   - Use Entity Framework or a data access structure with Spanish names.

4. CONTROLLERS:
   - Create controllers in Spanish, for example:
     - ClienteController
     - ProductoController
   - Include actions in Spanish:
     - ListarClientes
     - RegistrarCliente
     - EditarCliente
     - EliminarCliente

5. VIEWS:
   - Create views with Spanish names:
     - ListarClientes.cshtml
     - RegistrarCliente.cshtml
     - EditarCliente.cshtml
   - Forms must be in Spanish and properly bound to the models.

6. LINKS AND FORMS:
   - Use MVC tag helpers (asp-controller, asp-action) with Spanish names.
   - Bind forms to models using asp-for with Spanish property names.

7. COMMENTS AND EXPLANATIONS:
   - Every code file must include clear comments in Spanish explaining its purpose.
   - At the end, explain the full flow:
     User → View → Controller → Model → Database → View.

8. PROJECT GOAL:
   - The project must be understandable for a university instructor.
   - It should work as an academic example of MVC architecture.
   - The code must be clean, organized, and easy to explain.

9. OUTPUT FORMAT:
   - Show the project folder structure.
   - Provide full examples of:
     - One model in Spanish
     - One controller in Spanish
     - One view in Spanish
     - One functional form
   - Include instructions to run the project in Visual Studio Community.

Generate this project as a base template for a simple client and product management system for academic learning purposes.


