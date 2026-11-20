# Tactics for Building Background Services: HangFire

Hello!  

This is the official GitHub repo for "Building Background Services in .NET with HangFire".  You can find the course hosted on Udemy - [click here](https://www.udemy.com/course/building-background-services-in-net-with-hangfire/?referralCode=ADE705B8BB4A44749A7D) to join!

## Versions

All examples target:
- **.NET 11** (STS, supported until Nov 14, 2028)
- **Hangfire 1.8.25**
- **Hangfire.Storage.SQLite 0.4.3** (for SQLite examples)
- **Microsoft.EntityFrameworkCore.* 11.0.0** (for Identity/EF Core examples)

> **Note:** .NET 11 is an STS release; the next planned retarget is .NET 12 (LTS), expected Nov 2027.

### BackgroundService behavior change (.NET 10+)

Starting with .NET 10, `BackgroundService.ExecuteAsync` runs entirely on a background thread rather than starting synchronously. This behavior carries into .NET 11 and may affect startup ordering if your code depends on synchronous initialization before the first await. See [the breaking change documentation](https://learn.microsoft.com/dotnet/core/compatibility/extensions/10.0/backgroundservice-executeasync-task) for details.

## Questions?

If you have any questions for the course or course material, feel free to drop an issue or ask a question directly in the course.  
    
## Example Connection String

If you're using our Dockerfile to run SQL Server, you can use the following connection string:

```
Server=localhost,1433;Database=HangFire;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=true;
```
