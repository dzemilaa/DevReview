# DevReview

Platforma za code review i mentorstvo koja omogućava programerima da dobiju strukturisan feedback na kod od iskusnih mentora, uz podršku za zakazivanje sesija, sistem ocjenjivanja i praćenje napretka.

---

## Sadržaj

- [Arhitektura](#arhitektura)
- [ER dijagram](#er-dijagram)
- [Pokretanje projekta](#pokretanje-projekta)

---

## Arhitektura

Projekat je organizovan po principima čiste arhitekture (Clean Architecture) sa jasno odvojenim slojevima:

```
DevReview/
├── DevReview.Domain/          # Entiteti, enumeracije — bez zavisnosti
├── DevReview.Application/     # CQRS handleri, DTOs, interfejsi, validatori
├── DevReview.Infrastructure/  # EF Core, repozitoriji, JWT, eksterni servisi
├── DevReview.API/             # ASP.NET kontroleri, middleware, background servisi
└── DevReviewFrontend/         # React + TypeScript SPA
```

### Slojevi

**Domain** — čisti domainki model bez ikakvih zavisnosti prema frameworcima. Sadrži entitete i enumeracije.

**Application** — sva poslovna logika implementovana kroz CQRS pattern (MediatR). Svaka operacija je ili `Command` (mijenja stanje) ili `Query` (čita stanje). Validacija ulaznih podataka radi kroz FluentValidation.

**Infrastructure** — konkretne implementacije interfejsa definisanih u Application sloju: Entity Framework Core sa PostgreSQL providerom, JWT generisanje i validacija, email servis.

**API** — tanki sloj koji prima HTTP zahtjeve, delegira ih MediatR-u i vraća odgovor. Sadrži middleware za globalnu obradu grešaka i sanitizaciju unosa, te background servis koji automatski oslobađa zauzete review requestove kojima je istekao rok.

**Frontend** — React 19 SPA sa TypeScript-om. Komunikacija sa API-jem ide kroz Axios, stanje aplikacije se drži u Zustand storeu, a server state (keširanje, sinhronizacija) upravlja TanStack React Query.


### Backend

| Tehnologija | Svrha |
|---|---|
| .NET 10 / ASP.NET Core | Web framework |
| Entity Framework Core 8 | ORM |
| PostgreSQL (Neon) | Baza podataka |
| MediatR | CQRS pattern |
| FluentValidation | Validacija komandi |
| AutoMapper | Mapiranje entiteta na DTOs |
| JWT Bearer | Autentifikacija |
| BCrypt.Net | Hashovanje lozinki |
| Serilog | Strukturirano logovanje (konzola + fajl) |
| HtmlSanitizer | Zaštita od XSS napada |
| Swagger / OpenAPI | Dokumentacija API-ja |

### Frontend

| Tehnologija | Svrha |
|---|---|
| React 19 + TypeScript | UI framework |
| Vite 8 | Build tool |
| Tailwind CSS 4 | Stilizovanje |
| React Router v7 | Rutiranje |
| Zustand | Globalni state |
| TanStack React Query | Server state i keširanje |
| Axios | HTTP klijent |

---

## ER dijagram

```
┌─────────────────────────────────────┐
│                USER                 │
│─────────────────────────────────────│
│ Id (PK)                             │
│ UserName (unique)                   │
│ Email (unique)                      │
│ DisplayName                         │
│ PasswordHash                        │
│ Role (Author/Mentor/Admin)          │
│ YearsOfExperience                   │
│ HourlyRate                          │
│ AvailableHoursPerWeek               │
│ Bio                                 │
│ GitHubUrl                           │
│ AverageMentorRating                 │
│ AverageAuthorRating                 │
│ TotalReviews                        │
│ IsBlocked / BlockedAt / BlockReason │
│ IsActive                            │
└──────────────┬──────────────────────┘
               │
       ┌───────┼────────────────────────────────────────┐
       │       │                                        │
       ▼       ▼                                        ▼
┌─────────────────────┐   ┌──────────────────┐   ┌──────────────────────┐
│    REVIEW REQUEST   │   │   OFFICE HOUR    │   │   USER LANGUAGE      │
│─────────────────────│   │──────────────────│   │──────────────────────│
│ Id (PK)             │   │ Id (PK)          │   │ Id (PK)              │
│ Title               │   │ MentorId (FK)    │   │ UserId (FK)          │
│ Description         │   │ StartTime        │   │ Language             │
│ ProgrammingLanguage │   │ EndTime          │   │ Proficiency          │
│ Framework           │   │ DurationMinutes  │   └──────────────────────┘
│ Difficulty          │   │ Topic            │
│ IsPublic / IsPaid   │   │ Price            │   ┌──────────────────────┐
│ Price               │   │ Status           │   │   NOTIFICATION       │
│ Tags                │   │ BookedById (FK)  │   │──────────────────────│
│ Status              │   │ BookingDesc.     │   │ Id (PK)              │
│ OwnerId (FK) ───────┼──▶│ MentorImpression │   │ UserId (FK)          │
│ MentorId (FK)       │   │ AuthorImpression │   │ Message              │
│ ClaimedAt           │   └──────────────────┘   │ Type                 │
│ ClaimTimeout        │                           │ IsRead               │
│ CompletedAt         │                           └──────────────────────┘
└──────┬──────────────┘
       │                                          ┌──────────────────────┐
   ┌───┼──────────────┐                           │   USER FOLLOW        │
   │   │              │                           │──────────────────────│
   ▼   ▼              ▼                           │ FollowerId (FK, PK)  │
┌────────┐  ┌───────────────────┐  ┌──────────┐  │ MentorId (FK, PK)    │
│  CODE  │  │      COMMENT      │  │  REVIEW  │  └──────────────────────┘
│  FILE  │  │───────────────────│  │──────────│
│────────│  │ Id (PK)           │  │ Id (PK)  │  ┌──────────────────────┐
│ Id(PK) │  │ Content           │  │ Summary  │  │  MENTOR REPUTATION   │
│ Review │  │ Type              │  │ Priority │  │──────────────────────│
│ Request│  │ StartLine/EndLine │  │ Fixes    │  │ MentorId (FK, PK)    │
│ Id(FK) │  │ SuggestedCode     │  │ Quality  │  │ Language (PK)        │
│ File   │  │ IsResolved        │  │ Score    │  │ AverageScore         │
│ Name   │  │ UserId (FK)       │  │ Author   │  │ TotalReviews         │
│ Content│  │ ReviewRequestId   │  │ Ratingof │  │ Score1-5Count        │
│ Order  │  │ (FK)              │  │ Mentor   │  └──────────────────────┘
│ Index  │  │ CodeFileId (FK)   │  │ Mentor   │
└────────┘  │ ParentCommentId   │  │ Ratingof │  ┌──────────────────────┐
            │ (FK, self-ref)    │  │ Author   │  │  USER REFRESH TOKEN  │
            └───────────────────┘  │ Reviewer │  │──────────────────────│
                                   │ Id (FK)  │  │ Id (PK)              │
                                   │ Review   │  │ UserId (FK)          │
                                   │ Request  │  │ Token (indexed)      │
                                   │ Id (FK)  │  │ ExpiresAt            │
                                   └──────────┘  │ IsRevoked            │
                                                 └──────────────────────┘
```

### Relacije

| Relacija | Tip | Opis |
|---|---|---|
| User → ReviewRequest | 1:N | Korisnik može imati više review requestova (Owner) |
| User → ReviewRequest | 1:N | Mentor može preuzeti više requestova (Mentor) |
| ReviewRequest → CodeFile | 1:N | Request sadrži jedan ili više fajlova koda |
| ReviewRequest → Comment | 1:N | Request može imati više komentara |
| ReviewRequest → Review | 1:N | Request može imati jedan finalni review |
| Comment → Comment | 1:N | Self-referenca za odgovore na komentare |
| Comment → CodeFile | N:1 | Komentar je vezan za konkretan fajl (opciono) |
| User → OfficeHour | 1:N | Mentor može imati više dostupnih termina |
| User → OfficeHour | 1:N | Korisnik može rezervisati termine (BookedBy) |
| User → UserLanguage | 1:N | Korisnički jezički profil |
| User → MentorReputation | 1:N | Reputacija mentora po jeziku |
| User → UserFollow | M:N | Korisnici mogu pratiti mentore |
| User → Notification | 1:N | Obavještenja po korisniku |
| User → UserRefreshToken | 1:N | JWT refresh tokeni |

---

## Pokretanje projekta

### Preduslovi

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js 20+](https://nodejs.org/)
- PostgreSQL instanca (lokalna ili cloud — npr. [Neon](https://neon.tech))

### Backend

**1. Klonirati repozitorij**

```bash
git clone <repo-url>
cd DevReview
```

**2. Podesiti konfiguraciju**

Kreirati `DevReview.API/appsettings.Development.json` (nije u repozitoriju):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=<host>;Database=<db>;Username=<user>;Password=<password>;SSL Mode=Require"
  },
  "JwtSettings": {
    "Issuer": "DevReviewApi",
    "Audience": "DevReviewClient",
    "Secret": "<min. 32 karaktera dugačak tajni ključ>",
    "AccessTokenExpirationMinutes": 15,
    "RefreshTokenExpirationDays": 7
  },
  "AllowedOrigins": ["http://localhost:5173"]
}
```

**3. Primijeniti migracije**

```bash
cd DevReview.API
dotnet ef database update
```

**4. Pokrenuti API**

```bash
dotnet run --project DevReview.API
```

API je dostupan na `http://localhost:5147`.  
Swagger dokumentacija: `http://localhost:5147/swagger`

### Frontend

**1. Instalirati zavisnosti**

```bash
cd DevReviewFrontend
npm install
```

**2. Podesiti environment**

Kreirati `.env.local`:

```
VITE_API_BASE_URL=http://localhost:5147/api/v1
```

**3. Pokrenuti development server**

```bash
npm run dev
```

Frontend je dostupan na `http://localhost:5173`.

