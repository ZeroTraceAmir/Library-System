# AGENTS.md

## Build & Run

```powershell
dotnet build
dotnet run
```

Target: `net10.0-windows`, Windows Forms (WinExe). Uses the vendored SDK at `dotnet-sdk-10.0.203-win-x64/` (ignored by git). No tests, no linter, no formatter config.

## Architecture

- **`Program.cs`** — entrypoint. Bootstraps DI (JsonDataStore → JsonCustomerRepository → CustomerService), checks `CustomerService.GetLoggedInCustomer()` to show `Home` (if logged in) or `Form1` (login/register welcome screen).
- **`Interfaces/`** — `IEntity` (Id), `IRepository<T>`, `ICustomer`, `ICustomerRepository`.
- **`Models/`** — `Customer` with `Id`, `Name`, `Number`, `HashedPassword`, `IsLogedin`.
- **`Repositories/`** — `JsonDataStore` (generic JSON file loader/saver under `Data/`), `JsonRepository<T>` (CRUD), `JsonCustomerRepository`.
- **`Services/`** — `CustomerService` with `AddCustomer`, `GetLoggedInCustomer`, Login (incomplete — see below).
- **`Helpers/`** — `PasswordHasher` (PBKDF2 / SHA256, format: `iterations.salt.key` base64).
- **Forms** — `Form1` (welcome), `RegistrationForm`, `LoginForm`, `Home` (maximized stub).

## Gotchas

- **Persian (RTL) UI** — all forms set `RightToLeft = Yes`, `RightToLeftLayout = true`. Font is `Tahoma`. New forms should follow the same pattern.
- **User culture is Persian** — validation/error messages are in Persian. Keep UI strings in Persian for consistency.
- **No `Login` implementation yet** — `CustomerService.Login(Customer)` at `Services/CustomerService.cs:39-48` is incomplete (won't compile). `LoginForm.cs` calls `_customerService.Login(phone, password)` which doesn't exist — will need a working login method.
- **`LoginForm` has two versions** — the active one (lines 33-192) takes `CustomerService` in its constructor. The commented-out block above it is the old stub. Delete dead code when editing.
- **Data is persisted in `Data/*.json`** — `JsonDataStore` reads/writes the full list on every operation. No migration system.
- **`JsonRepository.Update` does an in-place swap by index** — not a partial update.
- **No test project exists** — if you need to add one, the root namespace is `library_system`.
