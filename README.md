# Employee Management System – Upgrade Pack (Roles + Reports + Demo Data)

This pack adds:
- Role-based access (**Admin**, **HR**, **Employee**) with seeding.
- Beautiful, detailed **Excel** and **PDF** reports (Employees & Leaves).
- **Demo data** + **demo users** for immediate testing.
- A partial view with export buttons.
- Snippets to wire everything quickly without guessing.

> This pack is additive and non-destructive. It **does not overwrite** your existing files. You can copy/paste or merge pieces as needed.

---

## 1) Install NuGet packages

Add these to your `.csproj` (see `Snippets/Csproj_PackageReferences.txt`):

- `EPPlus` (for Excel)
- `QuestPDF` (for PDF)
- `Bogus` (for generating fake demo data)
- `Microsoft.AspNetCore.Identity.UI` (if not already present)

After editing the csproj, run:
```
dotnet restore
```

---

## 2) Register services & seed roles/users + demo data

Open your `Program.cs` and add the registrations in **ConfigureServices** section (before `builder.Build()`), then call the seeders **after** `app` is built.

See `Snippets/Program_Additions.cs` for copy-paste safe code. In short:

```csharp
builder.Services.AddScoped<Services.Reporting.IReportService, Services.Reporting.ReportService>();
// ... QuestPDF settings are handled inside service

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var sp = scope.ServiceProvider;
    await Infrastructure.DbSeeder.SeedRolesAndUsersAsync(sp);
    await Data.Seed.FakeDataSeeder.SeedAsync(sp);
}
```

> Replace namespaces if your project root namespace is not `EmployeesManagment`.

---

## 3) Add export buttons to your pages

Render the partial where Admin/HR can see it (e.g., Index pages):

```
@await Html.PartialAsync("~/Views/Shared/_ExportButtons.cshtml")
```

The partial points to:
- `GET /reports/employees/excel`
- `GET /reports/employees/pdf`
- `GET /reports/leaves/excel`
- `GET /reports/leaves/pdf`

---

## 4) Protect your controllers/actions

Examples in `Snippets/Auth_Attributes_Examples.cs` show how to use:
```csharp
[Authorize(Roles = Roles.Admin + "," + Roles.HR)]
```

---

## 5) Demo accounts

On first run, seeding creates:

- **Admin**: `admin@demo.local` / `Admin#12345`
- **HR**: `hr@demo.local` / `Hr#12345`
- **Employee**: `employee@demo.local` / `Emp#12345`

> Change passwords immediately in production.

---

## 6) License notes

See `LICENSE-THIRD-PARTY.txt`. EPPlus is LGPL/commercial; we use basic features acceptable for typical internal use.
QuestPDF is licensed under a permissive license for many scenarios; verify compliance for your distribution model.

---

## 7) Troubleshooting

- If your `ApplicationUser` or `DbContext` types have different names, update the usings and generic types in:
  - `Infrastructure/DbSeeder.cs`
  - `Data/Seed/FakeDataSeeder.cs`
  - `Controllers/ReportsController.cs`
  - `Services/Reporting` files

- If you don’t use ASP.NET Identity yet, scaffold Identity and ensure services are registered.

---

© 2025 – Prepared upgrade pack.
