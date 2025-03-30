# Medicines

**Medicines: توصيل الأدوية إلى منزلك بسرعة**

---

## 🌍 Overview | نظرة عامة

"Medicines" is a web-based system designed to simplify and digitize the process of ordering and delivering medicines. The system allows users to search for medicines, place orders, and interact with pharmacies and practitioners in a smart, smooth way.

"Medicines" هو نظام ويب يهدف إلى تبسيط ورقمنة عملية طلب وتوصيل الأدوية. يمكن للمستخدمين البحث عن الأدوية وتقديم الطلبات والتفاعل مع الصيدليات والممارسين بشكل سلس.

---

## 🔧 Features | المميزات

- Pharmacy Management ❓ إدارة الصيدليات
- Medicine Catalog ❓ كتالوج الأدوية
- Practitioner Profiles ❓ حسابات الممارسين
- Orders & Deliveries ❓ طلبات وتوصيل الأدوية
- JWT Authentication ❓ توكن المصادقة
- Clean Architecture (Controller-Service-Repository)

---

## ⚙️ Tech Stack | التقنيات المستخدمة

- ASP.NET Core 6 Web API
- Entity Framework Core + SQL Server
- AutoMapper, JWT
- Swagger UI for API testing
- React.js (optional for front-end)

---

## ⚡ How to Run | كيف تشغّل المشروع

1. **Clone the repo | استنسخ المشروع:**
   ```bash
   git clone https://github.com/naalkotabah/Medicines.git
   ```

2. **Update connection string | عدّل سلسلة الاتصال:**
   - In `appsettings.Development.json`

3. **Apply Migrations | طبّق الترحيلات:**
   ```bash
   dotnet ef database update
   ```

4. **Run the project | شغّل المشروع:**
   - Use Visual Studio or
   ```bash
   dotnet run
   ```

5. **Open Swagger UI:**
   - Navigate to `https://localhost:port/swagger`

---

## 📚 Project Structure | هيكل المشروع

```
Medicines/
├── Controllers/       --> API endpoints
├── Services/          --> Business logic
├── Repositories/      --> Data access
├── Data/              --> Models & DbContext
├── Middleware/        --> Error handling
├── DTOs/              --> Request/response models
└── Mapping/           --> AutoMapper Profiles
```

---

## ❤️ Author | من هو المطوّر؟

> Developed by **naalkotabah**  
> Contact: [GitHub Profile](https://github.com/naalkotabah)



