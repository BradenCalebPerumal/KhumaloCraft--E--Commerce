<div align="center">

<h1>🛍️⚡ KhumaloCraft Emporium – Part 3 Azure Cognitive Search & Durable Functions ⚡🛍️</h1>

<h2><img src="https://readme-typing-svg.herokuapp.com?font=Russo+One&size=28&duration=3000&pause=1000&color=0078D4&center=true&vCenter=true&width=1000&lines=🧠+Empowering+Search+with+Azure+Cognitive+Search;🔄+Streamlined+Workflows+via+Durable+Functions;👨‍💻+Developed+by+Braden+Caleb+Perumal" alt="Typing SVG" /></h2>

</div>


**👤 Name:** Braden Caleb Perumal  
**🎓 Student Number:** ST10287165  

---

## 📑 Contents
1. 📖 Introduction  
2. ⚙️ Requirements  
3. 📝 How to Apply  
4. 🛍️ Application Overview  
5. 🏗️ Architecture  
6. 🚀 Functionality  
7. 📊 Non-Functional Requirements  
8. 🗂️ Change Log  
9. ❓ FAQs  
10. 🖥️ How to Use  
11. ⚙️ Local Setup & Deployment Instructions  
12. 📜 Licensing  
13. 🧩 Plugins  
14. 🙌 Credits  
15. 🌐 GitHub Link  
16. 🎥 Demonstration Video Link  
17. 🔑 Admin Login Credentials  
18. 📚 References  

---

## 1) Introduction
KhumaloCraft Emporium is a **global e-commerce platform** connecting artisans with worldwide customers.  
**Part 3** builds upon previous versions by integrating:
- **Azure Cognitive Search** for AI-powered, full-text product search.
- **Azure Durable Functions** for automated order workflows (inventory updates, payment processing, notifications).
- Full Azure SQL Database integration for dynamic product & order data.

---

## 2) Requirements
- Azure Subscription (App Service, Azure SQL, Cognitive Search, Functions)
- Visual Studio 2022  
- ASP.NET MVC Framework  
- Azure SQL Database  
- Azure Functions Tools  
- Git

### Sample `appsettings.json`:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
"CLDV6211_ST10287165_POE_P1Context": "Server=tcp:YOUR_SERVER_NAME.database.windows.net,1433;Initial Catalog=YOUR_DATABASE_NAME;Persist Security Info=False;User ID=YOUR_USERNAME;Password=YOUR_PASSWORD;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
  }
}
```

---

## 3) How to Apply
1. Clone or download this repository.  
2. Open in **Visual Studio 2022**.  
3. Restore dependencies.  
4. Create and deploy your Azure SQL Database.  
5. Update `appsettings.json` with your connection string and search settings.  
6. Build and run locally or publish to Azure App Service.

---

## 4) Application Overview
**Purpose:**  
Provide artisans with a global storefront while automating backend workflows.

**Features:**
- AI-enhanced search  
- Seamless product browsing  
- Automated order lifecycle via Durable Functions  
- Secure Azure-hosted backend

---

## 5) Architecture
Follows **MVC Pattern**:
- **Models:** Products, Orders, Users  
- **Views:** Razor pages for UI  
- **Controllers:** Handle business logic  

**Services Used:**
- Azure App Service (Hosting)  
- Azure SQL Database  
- Azure Cognitive Search  
- Azure Durable Functions

---

## 6) Functionality

### Customer:
- Search products by name, category, or description
- Add items to cart and checkout
- View past orders

### Admin:
- Manage product listings
- Process and track orders
- View search analytics

---

## 7) Non-Functional Requirements
- **Scalability:** Azure auto-scaling  
- **Security:** Role-based access control, encrypted DB connection  
- **Usability:** Clean UI, mobile-ready  
- **Reliability:** 99.9% uptime on Azure  

---

## 8) Change Log
**v3.0.0 – Part 3 Release**
- ✅ Added Azure Cognitive Search integration  
- ✅ Implemented Durable Functions for order workflow  
- ✅ Optimized DB queries for indexing  
- ✅ Improved UI for search and order tracking

---

## 9) FAQs
**Q:** Can I run this without an Azure account?  
**A:** No, Azure resources are required for search and functions.

**Q:** Does search work offline?  
**A:** No, it relies on Azure Cognitive Search API.

---

## 10) How to Use
1. Navigate to the home page.  
2. Use the search bar to find products.  
3. Add items to your cart.  
4. Complete checkout to trigger order workflows.

---

## 11) Local Setup & Deployment Instructions
1. **Clone Repo**
   ```bash
   git clone https://github.com/YourRepoLinkHere.git
   cd KhumaloCraftEmporium
   ```
2. **Create Azure SQL DB**
   - Create DB in Azure Portal.
   - Run `DatabaseSetup.sql` to create schema & seed data.
3. **Update appsettings.json**
   - Set `DefaultConnection` to your Azure SQL connection string.
   - Add Azure Search keys and index name.
4. **Run Locally**
   - Press F5 in Visual Studio.
   - Browse to `https://localhost:<port>`.
5. **Deploy to Azure App Service**
   - Right-click project → Publish → Azure → App Service.

---

## 12) Licensing
ABC Retailers is licensed under the MIT License. You are free to use, modify, and distribute the project with proper credit.

---

## 13) Plugins
- Azure Cognitive Search SDK  
- Azure Functions SDK  
- Entity Framework Core  

---

## 14) Credits

 👨‍💻 Braden Caleb Perumal (ST10287165)  
 📧 **Email:** [calebperumal28@gmail.com](mailto:calebperumal28@gmail.com)
  
---

## 15) GitHub Link
[🔗 Repository](https://github.com/BradenCalebPerumal/KhumaloCraft--E--Commerce.git)

---

## 16) Demonstration Video Link
[▶️ Watch Demo](https://dlssa-my.sharepoint.com/:v:/g/personal/caleb_dlssa_onmicrosoft_com/ES3p3bwFph9MnhouMrOc6QYBINoMjOt-LWQu1Ag5gue6Pg?e=CJVEfT)

---

## 17) Admin Login Credentials
> ⚠️ **Note:** For demonstration purposes, the **Admin** can create an account and log in directly.  
> This is **not** a production practice — in a real deployment, accounts will be seeded and roles properly assigned.

---

## 18) References
- BroCode. C# Full Course. [YouTube](https://www.youtube.com/watch?v=wxznTygnRfQ)  
- BroCode. C# for Beginners. [YouTube](https://www.youtube.com/watch?v=r3CExhZgZV8)  
- Microsoft Docs. Azure Blob Storage Overview. [https://learn.microsoft.com/azure/storage/blobs/](https://learn.microsoft.com/azure/storage/blobs/)  
- Microsoft Docs. Azure Functions Overview. [https://learn.microsoft.com/azure/azure-functions/](https://learn.microsoft.com/azure/azure-functions/)  
- GeeksforGeeks. C# Constructors. [https://www.geeksforgeeks.org/c-sharp-constructors/](https://www.geeksforgeeks.org/c-sharp-constructors/)  
