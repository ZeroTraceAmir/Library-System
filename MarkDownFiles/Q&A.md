# Q&A — پرسش و پاسخ

---

## Q: What do `[JsonDerivedType]` attributes do in `Notification.cs`?

**سوال:** ویژگی‌های `[JsonDerivedType]` در `Notification.cs` چه کاری انجام می‌دهند؟

**A:** `[JsonDerivedType]` is a **System.Text.Json** attribute that enables **polymorphic serialization/deserialization**.

**پاسخ:** `[JsonDerivedType]` یک ویژگی (attribute) از **System.Text.Json** است که **سریال‌سازی/دسریال‌سازی چندریختی (polymorphic)** را فعال می‌کند.

---

`Notification` is an abstract base class with concrete subclasses like `OverdueNotification`, `ReturnReminderNotification`, and `BookBorrowedNotification`. When you serialize a `List<Notification>` to JSON, the serializer needs to know which **concrete type** each object actually is — otherwise it wouldn't know what to instantiate when reading the JSON back.

`Notification` یک کلاس پایه انتزاعی (abstract) است که زیرکلاس‌های مشخصی مانند `OverdueNotification`، `ReturnReminderNotification` و `BookBorrowedNotification` دارد. وقتی یک `List<Notification>` را به JSON سریال‌سازی می‌کنید، سریال‌ساز باید بداند هر شیء دقیقاً از چه **نوع مشخصی (concrete type)** است — در غیر این صورت هنگام خواندن JSON نمی‌داند چه چیزی را نمونه‌سازی کند.

---

These attributes solve that by writing a **type discriminator** into the JSON output and reading it back on deserialization:

این ویژگی‌ها این مشکل را با نوشتن یک **تشخیص‌دهنده نوع (type discriminator)** در خروجی JSON و خواندن آن هنگام دسریال‌سازی حل می‌کنند:

```json
{
    "$type": "OverdueNotification",
    "CustomerId": 1,
    "Message": "..."
}
```

Each attribute maps a derived type to a string tag:

هر ویژگی یک نوع مشتق‌شده را به یک برچسب متنی نگاشت می‌کند:

- `typeof(OverdueNotification)` → `"OverdueNotification"`
- `typeof(ReturnReminderNotification)` → `"ReturnReminderNotification"`
- `typeof(BookBorrowedNotification)` → `"BookBorrowedNotification"`

Without these attributes, System.Text.Json would either throw on polymorphic usage or silently serialize only the base type's properties — and would fail to reconstruct the correct subclass when loading from `Data/notifications.json`.

بدون این ویژگی‌ها، System.Text.Json یا در استفاده چندریختی خطا می‌داد یا فقط ویژگی‌های نوع پایه را بی‌صدا سریال‌سازی می‌کرد — و هنگام بارگذاری از `Data/notifications.json` قادر به بازسازی زیرکلاس صحیح نمی‌شد.

---

## Q: Why are `[JsonDerivedType]` on the model instead of in `JsonDataStore.cs` where serialization happens?

**سوال:** چرا `[JsonDerivedType]` روی مدل قرار دارد به جای `JsonDataStore.cs` که سریال‌سازی در آن انجام می‌شود؟

**A:** Because `[JsonDerivedType]` is a **type-level attribute** that describes the *type hierarchy* itself — it tells the serializer "these are the concrete subclasses of `Notification`." It has nothing to do with *how* data is read/written.

**پاسخ:** زیرا `[JsonDerivedType]` یک **ویژگی سطح-نوع (type-level attribute)** است که خود *سلسله‌مراتب نوع* را توصیف می‌کند — به سریال‌ساز می‌گوید "این‌ها زیرکلاس‌های مشخص `Notification` هستند." این ربطی به *چگونگی* خواندن/نوشتن داده‌ها ندارد.

---

`JsonDataStore` is **generic** (`Load<T>`, `Save<T>`) — it works with any type. If we put polymorphism config there, it would need to know about every polymorphic model in the app, breaking its generic nature:

`JsonDataStore` **جنریک (generic)** است (`Load<T>`، `Save<T>`) — با هر نوعی کار می‌کند. اگر تنظیمات چندریختی را آنجا قرار دهیم، باید از هر مدل چندریختی در برنامه آگاه باشد که طبیعت جنریک آن را نقض می‌کند:

```csharp
// Bad — JsonDataStore would need to list every derived type of every model
// بد — JsonDataStore باید هر نوع مشتق‌شده از هر مدل را لیست کند
public List<T> Load<T>(string fileName)
{
    var options = new JsonSerializerOptions();
    if (typeof(T) == typeof(Notification))
        options.TypeInfoResolver = ...; // ugly, not scalable / زشت، مقیاس‌پذیر نیست
}
```

With the attributes on `Notification`, adding a new notification type is just one file change. `JsonDataStore` stays completely generic and never needs updating. It's the same principle as putting validation attributes on models instead of in the database layer — the attribute belongs with the type it describes.

با قرار دادن ویژگی‌ها روی `Notification`، افزودن یک نوع اعلان جدید فقط نیاز به تغییر یک فایل دارد. `JsonDataStore` کاملاً جنریک باقی می‌ماند و هرگز نیاز به به‌روزرسانی ندارد. این همان اصلی است که ویژگی‌های اعتبارسنجی را روی مدل‌ها قرار می‌دهیم نه در لایه پایگاه داده — ویژگی متعلق به نوعی است که توصیف می‌کند.
