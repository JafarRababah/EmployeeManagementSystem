# نظام إدارة الموظفين (Employee Management System)

## 📘 مقدمة

نظام لإدارة الموظفين والإجازات داخل الشركات الصغيرة والمتوسطة.  
يوفر النظام واجهة سهلة الاستخدام لإضافة الموظفين، متابعة الإجازات، وتوليد تقارير PDF وExcel.

---

## 🛠 المتطلبات (Requirements)

- .NET 8 (أو .NET 7)
- SQL Server 2019 أو أحدث
- IIS أو Kestrel لتشغيل التطبيق
- مكتبات NuGet:
  - Entity Framework Core
  - EPPlus (توليد تقارير Excel)
  - Rotativa / iTextSharp (توليد تقارير PDF)

---

## 🚀 التثبيت (Installation)

1. استورد قاعدة البيانات:

   - افتح SQL Server Management Studio
   - اضغط يمين على **Databases → Restore Database**
   - اختر ملف النسخة الاحتياطية `EmployeeManagement.bak`

2. عدل ملف `appsettings.json`:

   ```json
   "ConnectionStrings": {
       "DefaultConnection": "Server=.;Database=EmployeeManagement;User Id=sa;Password=yourpassword;"
   }
   ```

3. شغل التطبيق:

   ```bash
   dotnet run
   ```

4. الدخول الافتراضي:
   - **Email**: `admin@system.com`
   - **Password**: `123456`

---

## 👥 الأدوار (Roles & Permissions)

- **Admin**: تحكم كامل (إدارة موظفين، أقسام، صلاحيات، تقارير).
- **Manager**: الموافقة على الإجازات ومتابعة الموظفين.
- **Employee**: تقديم طلبات إجازة، عرض بياناته الشخصية.

---

## 📊 المزايا (Features)

- إدارة الموظفين (إضافة، تعديل، حذف).
- إدارة الأقسام والوظائف.
- نظام طلبات إجازة (تقديم، موافقة/رفض).
- تقارير Excel وPDF:
  - Employee Report
  - Leave Applications Report
- إشعارات داخلية (Notifications).

---

## 📝 المشاكل الشائعة (Troubleshooting)

- **مشكلة قاعدة البيانات**: تحقق من Connection String.
- **خطأ EPPlus License**: أضف
  ```csharp
  ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
  ```
  داخل Program.cs
- **كلمة مرور SQL Server منسية**: أعد تعيينها من SQL Server Management Studio.

---

## 🔮 التطوير المستقبلي (Future Enhancements)

- إضافة حضور وانصراف.
- دعم اللغتين (عربي/إنجليزي).
- تكامل مع البريد الإلكتروني للإشعارات.
