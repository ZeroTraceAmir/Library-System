# AGENTS.md

## Build

```powershell
& dotnet-sdk-10.0.203-win-x64\dotnet.exe build
& dotnet-sdk-10.0.203-win-x64\dotnet.exe run
```

SDK is vendored in the repo (`.gitignore`d). No system .NET install required. No tests, no linter, no formatter.

## Architecture

- **`Program.cs`** — entrypoint. Bootstraps DI for both `CustomerService` and `UserService`, checks `GetLoggedInCustomer()` / `GetLoggedInUser()` to route to `Home` (customer), `AdminPanel`, `StaffPanel`, or `Form1` (welcome).
- **Forms** — all code-behind (no `.Designer.cs`). `Form1` (welcome), `RegistrationForm`, `LoginForm`, `Home` (customer), `AdminPanel`, `StaffPanel`, `Profile`, and sub-pages (`SeeAllBooks`, `SeeAllCustomers`, etc.).
- **DI** — manual, wired inline at each call site. No DI container.
- **Data** — `Data/*.json`, read/written as full lists on every operation (`JsonDataStore` → `JsonRepository<T>`). No migrations.
- **Persistence** — `JsonRepository.Update` does in-place swap by index (not partial update).
- **Password** — stored as plain text (`Password` property, no hashing).

## Gotchas

- **Persian (RTL) UI** — most forms set `RightToLeft=Yes`, `RightToLeftLayout=true`, font `Tahoma`. Keep this convention.
- **Login navigation** — `Form1` uses `ShowDialog()`, checks `LoginForm.LoggedInUserRole` (null = customer, `admin`, `staff`) and opens the correct panel.
- **`CustomerService.Login`** signature is `(string phone, string password)` returning `bool`. `UserService.Login` has the same signature.
- **`Customer.Number` / `User.Number`** stores phone number. **`User.Number` was added later** — ensure it exists in JSON data.
- **`ICustomer.Id`** re-declares `IEntity.Id` — causes CS0108 warning. Add `new` keyword or remove.
- **`UserStatus` enum** — values are `admin` (0), `staff` (1). Serialized as integer in JSON.
- **Header pattern** — `Home`, `AdminPanel`, `StaffPanel` share a dark header bar (name, phone, role, profile button) + menu buttons below.
- **Sub-page pattern** — each sub-page is a maximized form with a `DialogResult.Cancel` back button at the bottom.
