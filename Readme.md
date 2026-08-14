# StoreApp — Basket Service

یه سرویس Basket (سبد خرید) با معماری Clean Architecture که خودم از صفر ساختم تا هم CQRS رو بدون MediatR پیاده‌سازی کنم، هم Domain Events رو با RabbitMQ وصل کنم، هم یه‌ذره با DDD واقعی (نه فقط تئوریش) کار کنم.

هدف پروژه نمایش یه CRUD ساده نبود — می‌خواستم ببینم اگه بخوام از صفر یه dispatcher برای CQRS بنویسم (بدون تکیه به MediatR)، چقدر واقعاً می‌فهمم پشت صحنه چه اتفاقی می‌افته.

## چه کاری انجام می‌ده

- ایجاد/دریافت سبد خرید برای هر کاربر
- افزودن، حذف و ویرایش تعداد آیتم‌ها با اعتبارسنجی قوانین کسب‌وکار (حداکثر تعداد هر آیتم، سقف مبلغ کل سبد)
- پاک کردن کامل سبد
- انقضای خودکار سبدهای غیرفعال از طریق یک `BackgroundService` که هر یک دقیقه اجرا می‌شه
- کش کردن سبد در Redis برای کاهش فشار روی دیتابیس
- انتشار Domain Event ها (اضافه‌شدن آیتم، انقضای سبد) روی RabbitMQ با یک Fanout Exchange

## معماری

```
src/
  StoreApp.Domain          -> Entity ها (Basket, BasketItem)، قوانین کسب‌وکار، Domain Event ها
  StoreApp.Application     -> Command/Query ها، Handler ها، Validator ها (FluentValidation)، Pipeline Behavior ها
  StoreApp.Infrastructure  -> EF Core، Redis، RabbitMQ، پیاده‌سازی Dispatcher ها
  StoreApp.Api             -> Controller ها، Program.cs
test/
  StoreApp.UnitTests       -> تست‌های واحد روی Handler ها
```

نکته‌ی مهم معماری: `Basket` یه Aggregate Root واقعیه — لیست آیتم‌ها فقط از داخل خود کلاس `Basket` تغییر می‌کنه (`AddItem`, `RemoveItem`, `UpdateQuantity`)، و از بیرون فقط به‌صورت `IReadOnlyCollection` قابل خوندنه. `BasketItem` هم constructor و متد `UpdateQuantity`‌ش `internal` هستن، یعنی حتی از یه پروژه‌ی دیگه هم نمی‌شه مستقیم بهش دست‌کاری کرد.

## تکنولوژی‌ها

.NET 10 · EF Core 10 (SQL Server) · Redis (StackExchange.Redis) · RabbitMQ.Client 7 · FluentValidation · xUnit + Moq

## چالش‌هایی که باهاشون درگیر بودم

### ۱. جلوگیری از تغییر مستقیم آیتم‌های سبد از بیرون
اولش `Items` رو به‌صورت یه `List<BasketItem>` عمومی گذاشته بودم که هرجای برنامه می‌شد مستقیم بهش `Add` یا `Remove` زد. این باعث می‌شد قوانینی مثل «حداکثر تعداد هر آیتم» یا «سقف مبلغ سبد» به‌راحتی دور زده بشن، چون یکی می‌تونست بدون رد شدن از متد `AddItem` مستقیم به لیست دستکاری کنه. راه‌حل این بود که لیست داخلی رو `private` کردم و فقط یه نسخه‌ی فقط-خواندنی (`IReadOnlyCollection`) از بیرون در دسترسه؛ هر تغییری باید از متدهای خود کلاس `Basket` (مثل `AddItem`, `RemoveItem`) رد بشه.

### ۲. تزریق یه سرویس داخل BackgroundService
`BasketExpirationBackgroundService` یه سرویس Singleton هست، ولی وقتی خواستم مستقیم `ICommandDispatcher` رو در Constructor بگیرم، خطا گرفتم که یه سرویس Scoped رو نمی‌شه داخل Singleton استفاده کرد. اولش گیج‌کننده بود چون تو Controller ها همین کار بدون مشکل جواب می‌داد. با کمی جستجو فهمیدم که باید به‌جاش `IServiceScopeFactory` رو بگیرم و هر بار که سرویس اجرا می‌شه، یه Scope جدید بسازم و از همون‌جا Dispatcher رو بگیرم.

### ۳. کند بودن ارسال پیام به RabbitMQ
اولین نسخه‌ای که نوشتم، هر بار که یه پیام (مثل رویداد اضافه‌شدن آیتم) می‌خواست منتشر بشه، یه Connection کاملاً جدید به RabbitMQ باز می‌کرد. متوجه شدم این کار باعث کند شدن هر Request می‌شه، چون باز کردن Connection خودش زمان می‌بره. راه‌حل ساده‌ای که استفاده کردم اینه که Connection رو یه بار می‌سازم و نگهش می‌دارم، و فقط اگه بسته بود دوباره یکی جدید می‌سازم.

### ۴. یادگیری اینکه چطور Command و Query رو به Handler درستشون برسونم
چون تصمیم گرفتم به‌جای استفاده از MediatR (که خودش این کار رو انجام می‌ده)، این بخش رو خودم بنویسم، اول نمی‌دونستم چطور باید از روی یه Command مشخص کنم کدوم Handler باید اجراش کنه. با کمک `GetType()` نوع واقعی Command رو گرفتم و از DI خواستم Handler متناظرش رو پیدا کنه. این بخش برام خیلی کمک کرد بفهمم Dependency Injection دقیقاً چطور پشت صحنه کار می‌کنه.

## چالش های آینده
### `outBox Pattern`
این پترن باید حتما در تمام پروژه های میکروسرویسی به کار برده شود چون احتمال دارد قبل از publish پیام دیتا عملیاتی انجام شود ولی وقتی به عمل publish رسید کرش اتفاق بیافتد یعنی عملیات کاربر با موفقیت اتفاق افتاده اما قبل publish  سرور کرش کند این یک خطای مهم هست که با الگو نام برده شد میتوان آن را کنترل کرد. 
### `BasketItemAddedEvent` 
صراحتا در تسک موضوعی اعلام نشده که چه اتفاقی نسبت به این event اتفاق بیافتد ؛ اما من هم مانند expire  فعلا یک لاگ ایجاد گردم نسبت به این رویداد
### `redis Cash`
بهتر هست که ما از بازه زمانی random استفاده کنیم به دلیل اینکه اگر در دقیقه یک، ۲۰۰ کاربر کش ۵ دقیقه ای بگیرند بعد از ۵ دقیقه، همزمان ۲۰۰ کاربر درخواست بدن باعث افت عملکرد می شود بهتر هست برای هر کاربر یک بازه زمانی random در نظر گرفت که آن ۲۰۰ کاربر هر کدام در دقیقه های متفاوت برایشان کش انجام شود.
### `Management Error on Microservice` 
برای مدیریت بهتر خطا در پروژه های میکروسرویسی جهت اینکه پروژه در نقطه عملکردی بهتری داشته باشد 
موارد زیر در آینده به آن اضافه شود 
- 1 timeouts with 
- 2 circuit Breaker Pattern with 
- 3 FallBack 
موارد بالا را میتوان با کتاب خانه Polly به کاربرد

### Dispatcher و مشکل overhead ناشی از `dynamic`

توی `CommandDispatcher` من، برای پیدا کردن و اجرای Handler درست، این کد رو دارم:

```csharp
var commandType = command.GetType();
var handlerType = typeof(ICommandHandler<,>).MakeGenericType(commandType, typeof(TResponse));
var handler = _serviceProvider.GetRequiredService(handlerType);

Func<Task<TResponse>> handlerDelegate = () => ((dynamic)handler).Handle((dynamic)command, cancellationToken);
```

اینجا سه تا کار داره در Runtime (یعنی وقتی برنامه در حال اجراست، نه وقتی کامپایل می‌شه) انجام می‌ده:
1. `command.GetType()` — نوع دقیق Command (مثلاً `AddItemToBasketCommand`) رو پیدا می‌کنه.
2. `MakeGenericType` — یه نوع جنریک جدید (`ICommandHandler<AddItemToBasketCommand, ServiceResult>`) می‌سازه. این یه عملیات Reflection هست، یعنی .NET باید در حافظه بگرده و این نوع رو بسازه.
3. `(dynamic)handler` — چون کامپایلر نمی‌دونه `handler` دقیقاً از چه Type ایه، مجبورم بگم "به‌جای بررسی الان، در Runtime بفهم این شیء چه متدهایی داره و `Handle` رو صداش کن".

مشکل اینجاست که **هر بار** که یه Request جدید میاد (مثلاً هر بار کاربر یه آیتم به سبدش اضافه می‌کنه)، این سه مرحله از اول انجام می‌شه. یعنی برای هر Request، دوباره `MakeGenericType` صدا زده می‌شه و دوباره در Runtime مشخص می‌شه که کدوم متد `Handle` باید اجرا بشه. این کار نسبت به فراخوانی مستقیم یه متد (که در Compile-Time مشخصه) کندتره، چون .NET باید هر بار این جست‌وجو رو از نو انجام بده.

**راه‌حل‌هایی که برای بهبودش در نظر دارم:**

 الان هر بار که یه Request جدید میاد، برنامه باید از اول بگرده و پیدا کنه که "این Command مال کدوم Handler ـه؟". این گشتن (`MakeGenericType`) یه‌کم زمان می‌بره. اما نکته اینجاست که جواب این سؤال هیچ‌وقت عوض نمی‌شه — یعنی `AddItemToBasketCommand` همیشه مال همون `AddItemToBasketCommandHandler` ـه، چه الان بپرسیم چه یک ساعت دیگه. پس چرا هر بار از اول محاسبه‌ش کنیم؟

راه‌حل ساده اینه که جواب رو یه بار محاسبه کنیم و در یه `Dictionary` نگهش داریم. دفعه‌های بعدی که همون نوع Command اومد، دیگه لازم نیست دوباره بگردیم؛ مستقیم از Dictionary می‌خونیمش که خیلی سریع‌تره. به این کار می‌گن **Cache کردن**.

یه نمونه‌ی ساده از این ایده، این شکلیه:

```csharp
public class CommandDispatcher : ICommandDispatcher, IScopedDependency
{
    private readonly IServiceProvider _serviceProvider;

    // اینجا جواب‌های قبلی رو نگه می‌داریم تا دوباره محاسبه نشن
    private static readonly Dictionary<Type, Type> _handlerTypeCache = new();

    public CommandDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<TResponse> Send<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default)
    {
        var commandType = command.GetType();

        // اول چک می‌کنیم: قبلاً این نوع Command رو محاسبه کردیم؟
        if (!_handlerTypeCache.TryGetValue(commandType, out var handlerType))
        {
            // نه، پس برای اولین‌بار محاسبه‌ش می‌کنیم و در Dictionary ذخیره‌ش می‌کنیم
            handlerType = typeof(ICommandHandler<,>).MakeGenericType(commandType, typeof(TResponse));
            _handlerTypeCache[commandType] = handlerType;
        }

        // از اینجا به بعد دقیقاً همون کد قبلی
        var handler = _serviceProvider.GetRequiredService(handlerType);

        Func<Task<TResponse>> handlerDelegate = () => ((dynamic)handler).Handle((dynamic)command, cancellationToken);

        // ... بقیه‌ی کد Pipeline Behavior ها همونطور که بود
        return await handlerDelegate();
    }
}
```

با این تغییر، `MakeGenericType` فقط یک بار برای هر نوع Command اجرا می‌شه (مثلاً فقط اولین باری که یکی می‌خواد آیتم اضافه کنه)، نه هر بار. این باعث می‌شه بخش گشتن دنبال Handler سریع‌تر بشه.

نکته: این کار فقط اون بخش `MakeGenericType` رو حل می‌کنه. بخش `(dynamic)` هنوز سر جاشه، چون اونجا باید در Runtime تصمیم بگیریم متد `Handle` رو چطور صدا بزنیم — این بخش راه‌حل پیچیده‌تری می‌خواد (مثل Source Generator) که فعلاً روش کار نکردم.

برای این پروژه که حجم درخواست‌هاش زیاد نیست، این overhead عملاً محسوس نیست؛ ولی می‌دونم اگه قرار بود این کد رو برای یه سیستم پرترافیک ببرم، این جای بهینه‌سازی داره.

### RabbitMQ Connection و مسئله‌ی thread-safe نبودن

توی `RabbitMqBasketEventPublisher` من، Connection این‌طوری مدیریت می‌شه:

```csharp
private async Task<IConnection> GetConnectionAsync()
{
    if (_connection is not null && _connection.IsOpen)
        return _connection;

    _connection = await _connectionFactory.CreateConnectionAsync();

    return _connection;
}
```

منطقش ساده‌ست: اگه یه Connection باز داریم، همونو برگردون؛ وگرنه یه Connection جدید بساز.

مشکل اینجاست که این سرویس (`RabbitMqBasketEventPublisher`) ممکنه هم‌زمان از چند جای برنامه صدا زده بشه — مثلاً دو تا کاربر مختلف، همزمان، هر کدوم یه آیتم به سبدشون اضافه می‌کنن، و برای هر کدوم باید یه Event منتشر بشه. فرض کن این اتفاق می‌افته:

1. Request شماره ۱ وارد `GetConnectionAsync` می‌شه، چک می‌کنه `_connection` هست یا نه — می‌بینه هنوز `null` هست.
2. **دقیقاً همون لحظه**، Request شماره ۲ هم وارد همین متد می‌شه و اونم می‌بینه `_connection` هنوز `null` هست (چون Request شماره ۱ هنوز به خط بعدی نرسیده).
3. حالا هر دو Request شروع می‌کنن به ساختن یه Connection جدید — یعنی به‌جای یک Connection، دو تا Connection همزمان باز می‌شه.

به این حالت که دو یا چند بخش از کد همزمان به یه منبع مشترک (اینجا `_connection`) دسترسی دارن و می‌تونن باهم تداخل ایجاد کنن، می‌گن مشکل **thread-safety** — یعنی این کد برای اجرای هم‌زمان (Concurrent) امن نوشته نشده.

برای این پروژه که فعلاً به‌صورت تک‌کاربره روی سیستم خودم تست می‌کنم، این مشکل عملاً پیش نمیاد چون درخواست‌ها به‌ندرت دقیقاً هم‌زمان می‌رسن. ولی اگه این سرویس زیر بار واقعی (چند صد Request در ثانیه) قرار بگیره، امکان داره چند Connection اضافه باز بشه که هم منابع سرور RabbitMQ رو الکی مصرف می‌کنه هم می‌تونه رفتار غیرمنتظره ایجاد کنه.

**راه‌حلی که برای بعد در نظر دارم:** استفاده از یه قفل ساده (`lock` یا `SemaphoreSlim`) دور بخشی که Connection رو می‌سازه، تا فقط یکی از Request ها اجازه داشته باشه در یک لحظه Connection جدید بسازه و بقیه منتظر بمونن تا همون Connection آماده بشه.

- تست‌ها فعلاً فقط روی یکی از Handler ها (`AddItemToBasketCommandHandler`) نوشته شده و باید برای بقیه‌ی Command/Query ها هم تکمیل بشه.

## اجرا (محلی)

```bash
dotnet restore
dotnet ef database update --project src/StoreApp.Infrastructure --startup-project src/StoreApp.Api
dotnet run --project src/StoreApp.Api
```

نیاز به SQL Server، Redis و RabbitMQ در دسترس (تنظیمات در `appsettings.json`).

## اجرا با Docker

برای اجرا نیازی به نصب SQL Server، Redis یا RabbitMQ روی سیستم نیست — همه از طریق `docker-compose` بالا میان.

### پیش‌نیاز
Docker و Docker Compose نصب باشه.

### مراحل

```bash
# ۱. build و اجرای همه‌ی سرویس‌ها (API + SQL Server + Redis + RabbitMQ)
docker compose up --build

# ۲. اجرای Migration ها روی دیتابیس داخل کانتینر (در یک ترمینال جدید، بعد از بالا اومدن سرویس‌ها)
docker compose exec storeapp.api dotnet ef database update --project /src/src/StoreApp.Infrastructure --startup-project /src/src/StoreApp.Api
```

> اگه ابزار `dotnet ef` داخل ایمیج final نصب نیست (چون از aspnet runtime-only استفاده شده، نه SDK)، ساده‌ترین راه اینه که Migration رو قبل از build ایمیج، به‌صورت محلی روی دیتابیسی که در `docker-compose.yml` تعریف شده اجرا کنی:
> ```bash
> dotnet ef database update --project src/StoreApp.Infrastructure --startup-project src/StoreApp.Api --connection "Server=localhost,1433;Database=BasketAppSimagran;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=true"
> ```

بعد از بالا اومدن، API روی `http://localhost:5152` در دسترسه و Swagger هم (در محیط Development) روی `http://localhost:5152/swagger`.

پنل مدیریت RabbitMQ هم روی `http://localhost:15672` قابل مشاهده‌ست (کاربری/رمز پیش‌فرض: `guest` / `guest`).

### نکته‌ی مهم درباره‌ی Connection String پیش‌فرض
`appsettings.json` پروژه به‌صورت پیش‌فرض از `(localdb)` استفاده می‌کنه که فقط روی ویندوز و بدون Docker کار می‌کنه. در `docker-compose.yml`، این مقدار از طریق Environment Variable با `Server=sqlserver;...` بازنویسی می‌شه (چون داخل شبکه‌ی Docker، نام سرویس `sqlserver` به‌جای `localhost` استفاده می‌شه). اگه بخوای این تنظیمات رو دائمی کنی، بهتره یه `appsettings.Docker.json` جدا بسازی و `ASPNETCORE_ENVIRONMENT=Docker` رو در `docker-compose.yml` ست کنی.


## منابعی که جهت انجام این Task به کاربردم


### refrence Send on CommandDispacher
منبع اصلی زیر از سایت مدیوم استفاده کردم جهت اینکه بتوانم موضوع اصلی تسک CQRS  خام را به کار ببر
### https://jordansrowles.medium.com/building-your-own-mediator-pattern-in-modern-net-804995c44a1b


https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection
https://github.com/jbogard/MediatR



### refrence EventDispacher



https://tech-fellow.eu/2016/10/31/baking-round-shaped-software-mapping-to-the-code
https://stackoverflow.com/questions/30625363/implementing-domain-event-handler-pattern-in-c-sharp-with-simple-injector/30636387



