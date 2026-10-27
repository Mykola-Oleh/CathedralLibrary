# Кафедральна бібліотека

## Стек
- .NET 8 / ASP.NET Core MVC
- Entity Framework Core 8
- PostgreSQL через Npgsql
- Bootstrap 5

## Модель даних
- `Publication` — видання (корінь агрегату; `CreatedAt`, `UpdatedAt`)
- `Reader` — читач (корінь агрегату; `CreatedAt`, `UpdatedAt`)
- `Author` — автор
- `PublicationAuthor` — зв'язок багато-до-багатьох між виданнями й авторами
- `PublicationType` — довідник типів видань
- `Copy` — фізичний примірник
- `CopyStatus` — довідник станів примірника
- `LibraryRequest` — запит читача на отримання примірника
- `RequestStatus` — довідник статусів запиту
- `FundChange` — історія змін бібліотечного фонду

<<<<<<< HEAD
Усього 10 сутностей; 
=======
Усього 10 сутностей;
>>>>>>> a4745cfd73e0385327ac5872e776b8e6c570294d
