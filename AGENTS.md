# AGENTS.md

## Build

```powershell
& dotnet-sdk-10.0.203-win-x64\dotnet.exe build
& dotnet-sdk-10.0.203-win-x64\dotnet.exe run
```

SDK is vendored in the repo (`.gitignore`d). No system .NET install required. No tests, no linter, no formatter.

## Architecture

- **`Program.cs`** — entrypoint. Bootstraps `JsonDataStore → JsonCustomerRepository → CustomerService`, checks `GetLoggedInCustomer()` to show `Home` (already logged in) or `Form1` (welcome screen).
- **Forms** — all code-behind (no `.Designer.cs`). `Form1` (welcome), `RegistrationForm`, `LoginForm`, `Home` (maximized stub).
- **DI** — manual, wired inline at each call site (repeat the same 3 lines in every handler that needs a service). No DI container.
- **Data** — `Data/*.json`, read/written as full lists on every operation (`JsonDataStore` → `JsonRepository<T>`). No migrations.
- **Persistence** — `JsonRepository.Update` does in-place swap by index (not partial update). `PasswordHasher` uses PBKDF2/SHA256, format: `iterations.salt.key` (base64).

## Gotchas

- **Persian (RTL) UI** — all forms set `RightToLeft=Yes`, `RightToLeftLayout=true`, font `Tahoma`. Error/validation messages are in Persian. Keep this convention when adding forms.
- **Dead code** — `LoginForm.cs:1-31` has a commented-out old stub; `Program.cs:1-36` has two commented-out older entrypoints. Delete or ignore.
- **Login doesn't navigate** — `LoginForm` sets `DialogResult=OK` and closes, but `Form1` uses `Show()` not `ShowDialog()`, so the welcome screen stays open. To fix: change to `ShowDialog()` and open `Home` on success.
- **`CustomerService.Login`** signature is `(string phone, string password)` returning `bool`. Do NOT change to `(Customer)` — `LoginForm` depends on the string signature.
- **`Customer.Number`** stores phone number (Persian digits accepted). Duplicate check in `AddCustomer` uses `c.Number == customer.Number`.
- **`ICustomer.Id`** re-declares `IEntity.Id` — causes CS0108 warning. Add `new` keyword or remove the redundant declaration.
