<!-- 
# 📚 Library System Architecture Specification

---

## 🔌 1. Interfaces

### **Domain Interfaces**
* `IBook`
* `IUser`
* `ICustomer`
* `ILoan`
* `IReservation`
* `IDebt`

### **Repository Interfaces**
* `IRepo` *(Generic Repository)*
* `IUserRepo`
* `ICustomerRepo`
* `IBookRepo`
* `ILoanRepo`
* `IReservationRepo`
* `IDebtRepo`

---

## 🗄️ 2. Models & Entities

| Model | Attributes / Properties |
| :--- | :--- |
| **📖 Book** | `Id`, `Title`, `PublicationYear`, `Author`, `Genre`, `CopiesAvailable`, `LostChargePrice` |
| **🤝 Loan** | `Id`, `BookId`, `CustomerId`, `LoanDate`, `ReturnDate` |
| **📌 Reservation** | `Id`, `BookId`, `CustomerId`, `ReserveDate` |
| **💰 Debt** | `Id`, `CustomerId`, `Amount`, `Reason`, `IsPaid` |
| **👤 Customer** | `Id`, `Name`, `Email`, `Password`, `IsLogedin` |
| **🛡️ User** | `Id`, `Name`, `Email`, `Password`, `Role` *(Admin / Staff)*, `IsLogedin` |

---

## 💾 3. Repositories

* 🌐 `JsonDataStore`
* 📁 `JsonBookRepo`
* 📁 `JsonLoanRepo`
* 📂 *(and all other JSON repository implementations for each model)*

---

## 🏷️ 4. Enums

* **`UserStatus`**
  * `Admin`
  * `Staff`
* **`DebtReason`**
  * `Late return`
  * `Lost`

---

## ⚙️ 5. Services & Methods

### 👤 **User Service**
* **C.R.U.D:** `add`, `delete`, `update`
* **Auth:** `login`
* **Query:** `search`, `filter`

### 👥 **Customer Service**
* **C.R.U.D:** `Add`, `delete`, `Update`
* **Library Actions:** `BorrowBook`, `returnBook`, `ReserveBook`, `LoseBook`
* **Finance:** `PayDebt`
* **Query:** `Search`, `filter`

### 📖 **Book Service**
* **C.R.U.D:** `add`, `delete`, `update`
* **Query:** `search`, `filter`

### 🤝 **Loan Service**
* **C.R.U.D:** `add`, `delete`, `update`
* **Query:** `search`, `filter`

### 📌 **Reservation Service**
* **C.R.U.D:** `add`, `delete`, `update`
* **Query:** `search`, `filter`

### 💰 **Debt Service**
* **C.R.U.D:** `add`, `delete`
* **Actions:** `update` *(Specifically: `increase`, `SetToZero`)*
* **Query:** `search`, `filter` -->
<!-- Here are two options to solve this issue on GitHub:

### Option 1: The "No-Emoji" Pure Text Elegant Version
This version uses only standard Markdown formatting (bolding, dividers, tables, and code blocks) to look beautiful and highly readable without relying on any icons or emojis.

```markdown
# Library System Architecture Specification

---

## 1. Interfaces

### Domain Interfaces
* `IBook`
* `IUser`
* `ICustomer`
* `ILoan`
* `IReservation`
* `IDebt`

### Repository Interfaces
* `IRepo` *(Generic Repository)*
* `IUserRepo`
* `ICustomerRepo`
* `IBookRepo`
* `ILoanRepo`
* `IReservationRepo`
* `IDebtRepo`

---

## 2. Models & Entities

| Model | Attributes / Properties |
| :--- | :--- |
| **Book** | `Id`, `Title`, `PublicationYear`, `Author`, `Genre`, `CopiesAvailable`, `LostChargePrice` |
| **Loan** | `Id`, `BookId`, `CustomerId`, `LoanDate`, `ReturnDate` |
| **Reservation** | `Id`, `BookId`, `CustomerId`, `ReserveDate` |
| **Debt** | `Id`, `CustomerId`, `Amount`, `Reason`, `IsPaid` |
| **Customer** | `Id`, `Name`, `Email`, `Password`, `IsLogedin` |
| **User** | `Id`, `Name`, `Email`, `Password`, `Role` *(Admin / Staff)*, `IsLogedin` |

---

## 3. Repositories

* `JsonDataStore`
* `JsonBookRepo`
* `JsonLoanRepo`
* *(and all other JSON repository implementations for each model)*

---

## 4. Enums

### UserStatus
* `Admin`
* `Staff`

### DebtReason
* `Late return`
* `Lost`

---

## 5. Services & Methods

### User Service
* **C.R.U.D:** `add`, `delete`, `update`
* **Auth:** `login`
* **Query:** `search`, `filter`

### Customer Service
* **C.R.U.D:** `Add`, `delete`, `Update`
* **Library Actions:** `BorrowBook`, `returnBook`, `ReserveBook`, `LoseBook`
* **Finance:** `PayDebt`
* **Query:** `Search`, `filter`

### Book Service
* **C.R.U.D:** `add`, `delete`, `update`
* **Query:** `search`, `filter`

### Loan Service
* **C.R.U.D:** `add`, `delete`, `update`
* **Query:** `search`, `filter`

### Reservation Service
* **C.R.U.D:** `add`, `delete`, `update`
* **Query:** `search`, `filter`

### Debt Service
* **C.R.U.D:** `add`, `delete`
* **Actions:** `update` *(Specifically: `increase`, `SetToZero`)*
* **Query:** `search`, `filter`
```

--- -->


# :books: Library System Architecture Specification

---

## :link: 1. Interfaces

### Domain Interfaces
* `IBook`
* `IUser`
* `ICustomer`
* `ILoan`
* `IReservation`
* `IDebt`

### Repository Interfaces
* `IRepo` *(Generic Repository)*
* `IUserRepo`
* `ICustomerRepo`
* `IBookRepo`
* `ILoanRepo`
* `IReservationRepo`
* `IDebtRepo`

---

## :card_index: 2. Models & Entities

| Model | Attributes / Properties |
| :--- | :--- |
| **Book** | `Id`, `Title`, `PublicationYear`, `Author`, `Genre`, `CopiesAvailable`, `LostChargePrice` |
| **Loan** | `Id`, `BookId`, `CustomerId`, `LoanDate`, `ReturnDate` |
| **Reservation** | `Id`, `BookId`, `CustomerId`, `ReserveDate` |
| **Debt** | `Id`, `CustomerId`, `Amount`, `Reason`, `IsPaid` |
| **Customer** | `Id`, `Name`, `Email`, `Password`, `IsLogedin` |
| **User** | `Id`, `Name`, `Email`, `Password`, `Role` *(Admin / Staff)*, `IsLogedin` |

---

## :file_folder: 3. Repositories

* `JsonDataStore`
* `JsonBookRepo`
* `JsonLoanRepo`
* *(and all other JSON repository implementations for each model)*

---

## :label: 4. Enums

### UserStatus
* `Admin`
* `Staff`

### DebtReason
* `Late return`
* `Lost`

---

## :gear: 5. Services & Methods

### User Service
* **C.R.U.D:** `add`, `delete`, `update`
* **Query:** `search`, `filter`

### Customer Service
* **C.R.U.D:** `Add`, `delete`, `Update`
* **Library Actions:** `BorrowBook`, `returnBook`, `ReserveBook`, `LoseBook`
* **Finance:** `PayDebt`
* **Query:** `Search`, `filter`

### Book Service
* **C.R.U.D:** `add`, `delete`, `update`
* **Query:** `search`, `filter`

### Loan Service
* **C.R.U.D:** `add`, `delete`, `update`
* **Query:** `search`, `filter`

### Reservation Service
* **C.R.U.D:** `add`, `delete`, `update`
* **Query:** `search`, `filter`

### Debt Service
* **C.R.U.D:** `add`, `delete`
* **Actions:** `update` *(Specifically: `increase`, `SetToZero`)*
* **Query:** `search`, `filter`