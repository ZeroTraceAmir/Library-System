# مستندات پروژه سیستم کتابخانه (Library System)

---

## ۱. معرفی کوتاه پروژه

**سیستم مدیریت کتابخانه** یک نرم‌افزار دسکتاپ مبتنی بر Windows Forms و زبان C# است که با هدف مدیریت عملیات روزمره یک کتابخانه طراحی و پیاده‌سازی شده است. این سیستم با بهره‌گیری از معماری لایه‌ای (سه‌لایه) و الگوهای طراحی نرم‌افزار، امکان مدیریت کاربران، کتاب‌ها، امانت‌ها، رزروها و بدهی‌ها را فراهم می‌کند.

**کاربران و نقش‌های اصلی سیستم:**

| نقش | توضیح |
|------|-----------|
| **مدیر (Admin)** | دسترسی کامل به مدیریت کارمندان، مشاهدهٔ مشتریان، افزودن کارمند جدید و ویرایش پروفایل |
| **کارمند (Staff)** | مدیریت کتاب‌ها (افزودن، ویرایش، حذف)، مشاهدهٔ مشتریان و ویرایش پروفایل |
| **مشتری (Customer)** | جستجو و مشاهدهٔ کتاب‌ها، امانت گرفتن و بازگرداندن کتاب، رزرو کتاب، پرداخت بدهی و ویرایش پروفایل |

**سناریوهای نمونه:**

۱. **ثبت نام و ورود مشتری:** یک کاربر جدید با وارد کردن نام، شماره تلفن و رمز عبور در فرم ثبت‌نام عضو سیستم می‌شود. سپس با ورود به صفحهٔ اصلی مشتری می‌تواند کتاب‌ها را مشاهده کرده و اقدام به امانت یا رزرو کتاب کند.

۲. **امانت و بازگرداندن کتاب:** مشتری پس از ورود به سیستم، از بخش «مشاهدهٔ کتاب‌ها» کتاب مورد نظر خود را انتخاب کرده و دکمهٔ «امانت گرفتن» را می‌زند. کتاب به مدت ۱۴ روز به امانت داده می‌شود. پس از بازگرداندن، تعداد نسخه‌های موجود کتاب افزایش می‌یابد.

۳. **مدیریت کارمندان توسط مدیر:** مدیر سیستم می‌تواند از پنل مدیریت، لیست کارمندان را مشاهده کرده، کارمند جدید اضافه کند یا حساب کاربری کارمندان را حذف نماید.

---

## ۲. شناسایی و تعریف کلاس‌ها (موجودیت‌ها)

### ۲.۱ موجودیت‌های اصلی (Models)

| کلاس | مسئولیت |
|------|----------|
| **BaseEntity** | کلاس پایهٔ انتزاعی که خاصیت `Id` را برای تمام موجودیت‌ها فراهم می‌کند |
| **Person** | کلاس انتزاعی که خاصیت `Name` (نام) را اضافه می‌کند |
| **Account** | کلاس انتزاعی حاوی اطلاعات حساب کاربری شامل شماره، رمز عبور و وضعیت ورود |
| **User** | نمایندهٔ کاربران سیستم (مدیر و کارمند) با نقش (`Role`) |
| **Customer** | نمایندهٔ مشتریان کتابخانه با قابلیت tracking امانت، رزرو و بدهی |
| **Book** | نمایندهٔ یک کتاب با مشخصات عنوان، نویسنده، ژانر، سال انتشار، تعداد نسخه و قیمت جریمه |
| **Loan** | نمایندهٔ یک امانت (قرض) کتاب شامل تاریخ امانت، تاریخ سررسید و تاریخ بازگشت |
| **Reservation** | نمایندهٔ یک رزرو کتاب توسط مشتری |
| **Debt** | نمایندهٔ یک بدهی مشتری شامل مبلغ، دلیل و وضعیت پرداخت |

### ۲.۲ واسط‌ها (Interfaces)

| واسط | مسئولیت |
|------|----------|
| **IEntity** | قرارداد داشتن خاصیت `Id` |
| **IRepository&lt;T&gt;** | قرارداد عمومی برای عملیات CRUD (دریافت همه، دریافت با شناسه، افزودن، به‌روزرسانی، حذف) |
| **IUser, ICustomer, IBook, ILoan, IReservation, IDebt** | قراردادهای مربوط به هر موجودیت |
| **IUserRepository تا IDebtRepository** | قراردادهای مخزن اختصاصی هر موجودیت |

### ۲.۳ لایهٔ مخزن (Repositories)

| کلاس | مسئولیت |
|------|----------|
| **JsonDataStore** | مدیریت خواندن و نوشتن فایل‌های JSON در پوشهٔ `Data` |
| **JsonRepository&lt;T&gt;** | پیاده‌سازی عمومی الگوی Repository با ذخیره‌سازی JSON |
| **JsonUserRepository و سایر کلاس‌های مشابه** | مخازن اختصاصی هر موجودیت که فایل JSON مربوطه را مشخص می‌کنند |

### ۲.۴ لایهٔ سرویس (Services)

| کلاس | مسئولیت |
|------|----------|
| **BaseService&lt;T&gt;** | کلاس پایهٔ انتزاعی برای سرویس‌ها با متد انتزاعی `Validate` |
| **UserService** | مدیریت کاربران (ورود، خروج، ویرایش پروفایل، افزودن و حذف کارمند) |
| **CustomerService** | مدیریت مشتریان (ثبت‌نام، ورود، خروج، ویرایش، حذف حساب) |
| **BookService** | مدیریت کتاب‌ها (افزودن، ویرایش، حذف، جستجو بر اساس عنوان/نویسنده، فیلتر بر اساس ژانر) |
| **LoanService** | مدیریت امانت‌ها (امانت گرفتن، بازگرداندن کتاب) |
| **ReservationService** | مدیریت رزروها (رزرو و لغو رزرو کتاب) |
| **DebtService** | مدیریت بدهی‌ها (افزودن و پرداخت بدهی) |

### ۲.۵ لایهٔ رابط کاربری (Forms)

| کلاس | مسئولیت |
|------|----------|
| **Form1** | صفحهٔ خوش‌آمدگویی با دکمه‌های ورود و ثبت‌نام |
| **LoginForm** | فرم ورود به سیستم برای همهٔ نقش‌ها |
| **RegistrationForm** | فرم ثبت‌نام مشتری جدید |
| **Home** | داشبورد اصلی مشتری با دسترسی به بخش‌های مختلف |
| **AdminPanel** | پنل مدیریت با دسترسی به مدیریت کارمندان و مشتریان |
| **StaffPanel** | پنل کارمند با دسترسی به مدیریت کتاب‌ها و مشتریان |
| **Profile** | فرم مشاهده و ویرایش پروفایل کاربری |
| **SeeBooks** | فرم مشاهده و جستجوی کتاب‌ها توسط مشتری با امکان امانت و رزرو |
| **SeeAllBooks** | فرم مشاهدهٔ همهٔ کتاب‌ها توسط کارمند با فیلتر و جستجو |
| **SeeAllCustomers** | فرم مشاهدهٔ همهٔ مشتریان با فیلتر وضعیت |
| **SeeAllEmployees** | فرم مشاهدهٔ همهٔ کارمندان (فقط مدیر) با امکان حذف |
| **MyLoans** | فرم مشاهدهٔ امانت‌های فعال مشتری با امکان بازگرداندن کتاب |
| **MyReservations** | فرم مشاهدهٔ رزروهای مشتری با امکان لغو |
| **PayDebt** | فرم مشاهده و پرداخت بدهی‌های مشتری |
| **AddBook** | فرم افزودن کتاب جدید توسط کارمند |
| **AddEmployee** | فرم افزودن کارمند جدید توسط مدیر |

---

## ۳. داده‌ها (فیلدها و خصوصیات)

### ۳.۱ واسط‌ها (Interfaces)

| واسط | خصوصیت | نوع داده | توضیح |
|------|---------|----------|-------|
| **IEntity** | `Id` | `int` | شناسهٔ یکتای موجودیت |
| **IUser** | `Name` | `string` | نام کاربر |
| | `Password` | `string` | رمز عبور |
| | `Role` | `UserStatus` | نقش کاربر (مدیر یا کارمند) |
| | `IsLogedin` | `bool` | وضعیت ورود به سیستم |
| **ICustomer** | `Name` | `string` | نام مشتری |
| | `Number` | `string` | شماره تلفن مشتری |
| | `Password` | `string` | رمز عبور |
| | `IsLogedin` | `bool` | وضعیت ورود به سیستم |
| **IBook** | `Title` | `string` | عنوان کتاب |
| | `Author` | `string` | نویسنده |
| | `CopiesAvailable` | `int` | تعداد نسخه‌های موجود |
| | `Genre` | `string` | ژانر کتاب |
| | `PublicationYear` | `int` | سال انتشار |
| | `LostChargePrice` | `double` | قیمت جریمهٔ گم کردن کتاب |
| **ILoan** | `CustomerId` | `int` | شناسهٔ مشتری امانت‌گیرنده |
| | `BookId` | `int` | شناسهٔ کتاب امانت داده شده |
| | `LoanDate` | `DateTime` | تاریخ امانت |
| | `DueDate` | `DateTime` | تاریخ سررسید بازگشت |
| | `ReturnDate` | `DateTime?` | تاریخ بازگشت (اختیاری) |
| **IReservation** | `CustomerId` | `int` | شناسهٔ مشتری رزروکننده |
| | `BookId` | `int` | شناسهٔ کتاب رزرو شده |
| | `ReservationDate` | `DateTime` | تاریخ رزرو |
| | `IsActive` | `bool` | وضعیت فعال بودن رزرو |
| **IDebt** | `CustomerId` | `int` | شناسهٔ مشتری بدهکار |
| | `Amount` | `decimal` | مبلغ بدهی |
| | `Reason` | `string` | دلیل بدهی |
| | `IsPaid` | `bool` | وضعیت پرداخت |

### ۳.۲ موجودیت‌ها (Models)

| کلاس | فیلد/خصوصیت | نوع داده | توضیح |
|------|------------|----------|-------|
| **BaseEntity** | `Id` | `int` | شناسهٔ یکتا |
| **Person** | `Name` | `string` | نام |
| **Account** | `Number` | `string` | شماره تلفن / شناسه |
| | `Password` | `string` | رمز عبور |
| | `IsLogedin` | `bool` | وضعیت ورود |
| **User** | `Role` | `UserStatus` | نقش (admin, staff) |
| **Customer** | `HasBorrowedBook` | `bool` | آیا کتاب امانت دارد |
| | `HasReservedBook` | `bool` | آیا کتاب رزرو کرده |
| | `Debt` | `decimal` | میزان بدهی |
| **Book** | `Title` | `string` | عنوان |
| | `Author` | `string` | نویسنده |
| | `CopiesAvailable` | `int` | تعداد نسخهٔ موجود |
| | `Genre` | `string` | ژانر |
| | `PublicationYear` | `int` | سال انتشار |
| | `LostChargePrice` | `double` | هزینهٔ جریمه |
| **Loan** | `CustomerId` | `int` | شناسهٔ مشتری |
| | `BookId` | `int` | شناسهٔ کتاب |
| | `LoanDate` | `DateTime` | تاریخ امانت |
| | `DueDate` | `DateTime` | تاریخ سررسید |
| | `ReturnDate` | `DateTime?` | تاریخ بازگشت |
| **Reservation** | `CustomerId` | `int` | شناسهٔ مشتری |
| | `BookId` | `int` | شناسهٔ کتاب |
| | `ReservationDate` | `DateTime` | تاریخ رزرو |
| | `IsActive` | `bool` | فعال بودن رزرو |
| **Debt** | `CustomerId` | `int` | شناسهٔ مشتری |
| | `Amount` | `decimal` | مبلغ |
| | `Reason` | `string` | دلیل |
| | `IsPaid` | `bool` | وضعیت پرداخت |

### ۳.۳ شمارشی‌ها (Enums)

| شمارشی | مقدارها | توضیح |
|---------|---------|-------|
| **UserStatus** | `admin`, `staff` | نقش‌های کاربران سیستم |
| **UserFilter** | `All`, `Admins`, `Staff` | فیلترهای نمایش کاربران |
| **CustomerFilter** | `All`, `HasBorrowed`, `HasReserved`, `HasDebt` | فیلترهای نمایش مشتریان |

---

## ۴. وظیفه و رفتارها (متدهای اصلی)

### ۴.۱ لایهٔ مخزن (Repository)

**کلاس JsonRepository&lt;T&gt;**

| متد | امضا | توضیح |
|-----|------|-------|
| GetAll | `List<T> GetAll()` | دریافت لیست تمام موجودیت‌ها از فایل JSON |
| GetById | `T? GetById(int id)` | جستجوی یک موجودیت با شناسه |
| Add | `void Add(T entity)` | افزودن موجودیت جدید و ذخیره در فایل |
| Update | `void Update(T entity)` | به‌روزرسانی یک موجودیت موجود |
| Delete | `void Delete(int id)` | حذف یک موجودیت با شناسه |

**کلاس JsonDataStore**

| متد | امضا | توضیح |
|-----|------|-------|
| Load | `List<T> Load<T>(string fileName)` | بارگذاری داده‌ها از فایل JSON و تبدیل به لیست |
| Save | `void Save<T>(string fileName, List<T> data)` | ذخیرهٔ لیست داده‌ها در فایل JSON |

### ۴.۲ لایهٔ سرویس (Service)

**کلاس UserService**

| متد | امضا | توضیح |
|-----|------|-------|
| Login | `bool Login(string phone, string password)` | احراز هویت کاربر با شماره و رمز عبور |
| Logout | `void Logout(string phone)` | خروج کاربر از سیستم |
| GetAllUsers | `List<User> GetAllUsers()` | دریافت لیست تمام کاربران |
| GetFilteredUsers | `List<User> GetFilteredUsers(UserFilter filter)` | دریافت کاربران فیلتر شده بر اساس نقش |
| AddEmployee | `void AddEmployee(string name, string phone, string password, string repeatPassword, UserStatus role)` | افزودن کارمند جدید با اعتبارسنجی |
| DeleteUser | `void DeleteUser(int id)` | حذف یک کاربر با شناسه |
| UserProfileEdit | `void UserProfileEdit(string name, string phone, string password, string _phone)` | ویرایش پروفایل کاربر |
| Validate | `override void Validate(User user)` | اعتبارسنجی اطلاعات کاربر |

**کلاس CustomerService**

| متد | امضا | توضیح |
|-----|------|-------|
| Login | `bool Login(string phone, string password)` | احراز هویت مشتری |
| Logout | `void Logout(string phone)` | خروج مشتری از سیستم |
| AddCustomer | `void AddCustomer(Customer customer)` | ثبت‌نام مشتری جدید |
| GetAllCustomers | `List<Customer> GetAllCustomers()` | دریافت لیست تمام مشتریان |
| GetFilteredCustomers | `List<Customer> GetFilteredCustomers(CustomerFilter filter)` | دریافت مشتریان فیلتر شده |
| GetLoggedInCustomer | `Customer? GetLoggedInCustomer()` | دریافت مشتری وارد شده به سیستم |
| CustomerProfileEdit | `void CustomerProfileEdit(string name, string phone, string password, string _phone)` | ویرایش پروفایل مشتری |
| DeleteCustomerAcc | `void DeleteCustomerAcc(string phone)` | حذف حساب مشتری |
| Validate | `override void Validate(Customer customer)` | اعتبارسنجی اطلاعات مشتری |

**کلاس BookService**

| متد | امضا | توضیح |
|-----|------|-------|
| GetAllBooks | `List<Book> GetAllBooks()` | دریافت لیست تمام کتاب‌ها |
| GetBookById | `Book? GetBookById(int id)` | جستجوی کتاب با شناسه |
| AddBook | `void AddBook(Book book)` | افزودن کتاب جدید |
| UpdateBook | `void UpdateBook(Book book)` | به‌روزرسانی اطلاعات کتاب |
| DeleteBook | `void DeleteBook(int id)` | حذف کتاب |
| SearchByTitle | `List<Book> SearchByTitle(string title)` | جستجوی کتاب بر اساس عنوان |
| SearchByAuthor | `List<Book> SearchByAuthor(string author)` | جستجوی کتاب بر اساس نویسنده |
| FilterByGenre | `List<Book> FilterByGenre(string genre)` | فیلتر کتاب‌ها بر اساس ژانر |
| Validate | `override void Validate(Book book)` | اعتبارسنجی اطلاعات کتاب |

**کلاس LoanService**

| متد | امضا | توضیح |
|-----|------|-------|
| GetAllLoans | `List<Loan> GetAllLoans()` | دریافت لیست تمام امانت‌ها |
| GetLoanById | `Loan? GetLoanById(int id)` | جستجوی امانت با شناسه |
| AddLoan | `void AddLoan(Loan loan)` | ثبت امانت جدید |
| UpdateLoan | `void UpdateLoan(Loan loan)` | به‌روزرسانی امانت |
| DeleteLoan | `void DeleteLoan(int id)` | حذف امانت |
| GetLoansByCustomerId | `List<Loan> GetLoansByCustomerId(int customerId)` | دریافت امانت‌های یک مشخاص |
| BorrowBook | `void BorrowBook(Book book, int customerId)` | امانت گرفتن کتاب (کاهش تعداد نسخه و ایجاد امانت ۱۴روزه) |
| ReturnBook | `void ReturnBook(int loanId, Book book)` | بازگرداندن کتاب (ثبت تاریخ بازگشت و افزایش تعداد نسخه) |

**کلاس ReservationService**

| متد | امضا | توضیح |
|-----|------|-------|
| GetAllReservations | `List<Reservation> GetAllReservations()` | دریافت لیست تمام رزروها |
| GetReservationById | `Reservation? GetReservationById(int id)` | جستجوی رزرو با شناسه |
| AddReservation | `void AddReservation(Reservation reservation)` | ثبت رزرو جدید |
| UpdateReservation | `void UpdateReservation(Reservation reservation)` | به‌روزرسانی رزرو |
| DeleteReservation | `void DeleteReservation(int id)` | حذف رزرو |
| GetReservationsByCustomerId | `List<Reservation> GetReservationsByCustomerId(int customerId)` | دریافت رزروهای یک مشتری |
| ReserveBook | `void ReserveBook(Book book, int customerId)` | رزرو کتاب توسط مشتری |
| CancelReservation | `void CancelReservation(int reservationId)` | لغو رزرو |

**کلاس DebtService**

| متد | امضا | توضیح |
|-----|------|-------|
| GetAllDebts | `List<Debt> GetAllDebts()` | دریافت لیست تمام بدهی‌ها |
| GetDebtById | `Debt? GetDebtById(int id)` | جستجوی بدهی با شناسه |
| GetCustomerDebts | `List<Debt> GetCustomerDebts(int customerId)` | دریافت بدهی‌های پرداخت‌نشدهٔ یک مشتری |
| AddDebt | `void AddDebt(Debt debt)` | ثبت بدهی جدید |
| PayDebt | `void PayDebt(int debtId)` | پرداخت بدهی (علامت‌گذاری به عنوان پرداخت شده) |

---

## ۵. روابط بین کلاس‌ها و وراثت

### الف) روابط بین کلاس‌ها

| کلاس مبدأ | رابطه | کلاس مقصد | نوع رابطه |
|-----------|--------|-----------|-----------|
| **JsonRepository&lt;T&gt;** | استفاده می‌کند ← | **JsonDataStore** | **Association** (ترکیب弱) |
| **UserService** |持有 می‌کند ← | **IUserRepository** | **Association** (تزریق وابستگی) |
| **CustomerService** |持有 می‌کند ← | **ICustomerRepository** | **Association** |
| **BookService** |持有 می‌کند ← | **IBookRepository** | **Association** |
| **LoanService** |持有 می‌کند ← | **ILoanRepository** | **Association** |
| **ReservationService** |持有 می‌کند ← | **IReservationRepository** | **Association** |
| **DebtService** |持有 می‌کند ← | **IDebtRepository** | **Association** |
| **Form1** |می‌سازد ← | **LoginForm, RegistrationForm** | **Dependency** |
| **LoginForm** |می‌سازد ← | **Home, AdminPanel, StaffPanel** | **Dependency** |
| **Loan** |ارجاع می‌دهد به ← | **Customer** (با `CustomerId`) | **Association** |
| **Loan** |ارجاع می‌دهد به ← | **Book** (با `BookId`) | **Association** |
| **Reservation** |ارجاع می‌دهد به ← | **Customer** (با `CustomerId`) | **Association** |
| **Reservation** |ارجاع می‌دهد به ← | **Book** (با `BookId`) | **Association** |
| **Debt** |ارجاع می‌دهد به ← | **Customer** (با `CustomerId`) | **Association** |

### ب) وراثت (Inheritance)

#### ۱. سلسله‌مراتب وراثت موجودیت‌ها

```
IEntity (واسط)
    ↑
BaseEntity (abstract) { Id }
    ↑
Person (abstract) { Name }
    ↑
Account (abstract) { Number, Password, IsLogedin, GetRoleLabel() }
    ↑
    ├── User { Role } ← پیاده‌سازی IUser
    └── Customer { HasBorrowedBook, HasReservedBook, Debt } ← پیاده‌سازی ICustomer
```

**کلاس پایه:** `BaseEntity` (انتزاعی) — تأمین‌کنندهٔ خاصیت `Id` برای تمام موجودیت‌ها.

**کلاس‌های مشتق‌شده:**
- `Person` ← `Account` ← `User` (نمایندهٔ مدیران و کارمندان)
- `Person` ← `Account` ← `Customer` (نمایندهٔ مشتریان)
- `Book` (مستقیماً از `BaseEntity`)
- `Loan`, `Reservation`, `Debt` (مستقیماً از `BaseEntity`)

**ویژگی‌ها و رفتارهای مشترک:**
- تمام موجودیت‌ها دارای `Id` هستند (از `BaseEntity`).
- `Person` خاصیت `Name` را اضافه می‌کند.
- `Account` خاصیت‌های `Number`، `Password`، `IsLogedin` و متد `GetRoleLabel()` را اضافه می‌کند.
- `User` و `Customer` متد `GetRoleLabel()` را بازنویسی (override) می‌کنند تا برچسب نقش مناسب را برگردانند.

**توجیه استفاده از وراثت:**
استفاده از وراثت در این سلسله‌مراتب به دلیل **اشتراک‌گذاری ویژگی‌های مشترک** بین انواع مختلف موجودیت‌ها است. تمام موجودیت‌ها به یک شناسهٔ یکتا نیاز دارند (`BaseEntity`). تمام حساب‌های کاربری (User و Customer) دارای نام، شماره، رمز عبور و وضعیت ورود مشترک هستند که در `Account` تعریف شده است. این طراحی از **تکرار کد** جلوگیری کرده و اعمال تغییرات را در آینده آسان‌تر می‌کند. متد `GetRoleLabel()` به عنوان یک متد مجازی در `Account` تعریف شده و هر کلاس مشتق آن را متناسب با نقش خود بازنویسی می‌کند که نمونه‌ای از **چندریختی (Polymorphism)** است.

#### ۲. سلسله‌مراتب وراثت مخازن

```
IRepository<T> (واسط عمومی CRUD)
    ↑
JsonRepository<T> (پیاده‌سازی عمومی با ذخیره‌سازی JSON)
    ↑
    ├── JsonUserRepository : IUserRepository
    ├── JsonCustomerRepository : ICustomerRepository
    ├── JsonBookRepository : IBookRepository
    ├── JsonLoanRepository : ILoanRepository
    ├── JsonReservationRepository : IReservationRepository
    └── JsonDebtRepository : IDebtRepository
```

**کلاس پایه:** `JsonRepository<T>` — پیاده‌سازی عمومی الگوی Repository با ذخیره و بارگذاری از فایل‌های JSON.

**کلاس‌های مشتق‌شده:** هرکدام از مخازن اختصاصی با مشخص کردن نام فایل JSON مربوطه.

**توجیه:** استفاده از **وراثت جنریک (Generic Inheritance)** امکان می‌دهد منطق مشترک CRUD یک بار در `JsonRepository<T>` پیاده‌سازی شده و تمام مخازن اختصاصی از آن بهره ببرند. این کار از تکرار کد جلوگیری کرده و قابلیت نگهداری سیستم را افزایش می‌دهد.

#### ۳. سلسله‌مراتب وراثت سرویس‌ها

```
BaseService<T> (abstract) { Validate() }
    ↑
    ├── UserService
    ├── CustomerService
    └── BookService
```

**کلاس پایه:** `BaseService<T>` — کلاس انتزاعی با متد انتزاعی `Validate()`.

**کلاس‌های مشتق‌شده:** `UserService`، `CustomerService`، `BookService` که هرکدام متد `Validate()` را متناسب با موجودیت خود پیاده‌سازی می‌کنند.

**توجیه:** الگوی **Template Method** از طریق این وراثت پیاده‌سازی شده است. متد `Validate()` به عنوان یک قلاب (hook) برای اعتبارسنجی موجودیت‌ها در سرویس‌های مختلف عمل می‌کند. اگرچه فقط سه سرویس از این کلاس پایه ارث‌بری کرده‌اند، اما این طراحی امکان افزودن سرویس‌های جدید با اعتبارسنجی اختصاصی را در آینده فراهم می‌کند.

---

## پیوست: معماری کلی سیستم

```
┌─────────────────────────────────────────────────────────┐
│                   لایهٔ رابط کاربری (Forms)              │
│  Form1, LoginForm, RegistrationForm, Home, AdminPanel,   │
│  StaffPanel, Profile, SeeBooks, SeeAllBooks,             │
│  SeeAllCustomers, SeeAllEmployees, MyLoans,              │
│  MyReservations, PayDebt, AddBook, AddEmployee           │
└────────────────────────┬────────────────────────────────┘
                         │ استفاده می‌کند
                         ▼
┌─────────────────────────────────────────────────────────┐
│                 لایهٔ سرویس (Business Logic)              │
│  UserService, CustomerService, BookService,              │
│  LoanService, ReservationService, DebtService            │
└────────────────────────┬────────────────────────────────┘
                         │ استفاده می‌کند
                         ▼
┌─────────────────────────────────────────────────────────┐
│               لایهٔ مخزن (Data Access)                    │
│  JsonRepository<T>, JsonUserRepository, ...             │
│  JsonDataStore (JSON file handler)                      │
└────────────────────────┬────────────────────────────────┘
                         │ ذخیره و بازیابی
                         ▼
┌─────────────────────────────────────────────────────────┐
│              فایل‌های JSON (Data/)                        │
│  users.json, customers.json, books.json,                │
│  loans.json, reservation.json, debts.json               │
└─────────────────────────────────────────────────────────┘
```

**فناوری‌ها و ابزارهای استفاده شده:**
- **زبان:** C# با قابلیت‌های پیشرفته (Generics, Interfaces, Abstract Classes, Events, Delegates)
- **پلتفرم:** .NET 10.0
- **رابط کاربری:** Windows Forms با پشتیبانی از زبان فارسی و راست‌چین (RTL)
- **ذخیره‌سازی:** فایل‌های JSON (سیستم فایل)
- **الگوهای طراحی:** Repository, Dependency Injection, Template Method, Strategy, Event/Delegate
