# DevReview — Projekat 12

Platforma na kojoj junior developeri postavljaju isečke koda i traže review od iskusnijih mentora. Mentori preuzimaju zahteve, ostavljaju inline komentare i zaključni rezime; korisnici grade reputaciju; sistem rangira mentore po jeziku/framework-u.

## Struktura rešenja

| Sloj | Projekat | Uloga |
|------|----------|--------|
| **API** | `DevReview.API` | REST kontroleri, JWT, Swagger, middleware |
| **Aplikacija** | `DevReview.Application` | CQRS (MediatR), FluentValidation, DTO, AutoMapper |
| **Domen** | `DevReview.Domain` | Entiteti, enumi |
| **Infrastruktura** | `DevReview.Infrastructure` | EF Core, PostgreSQL, repozitorijumi, Unit of Work |
| **Frontend** | `DevReviewFrontend` | React + TypeScript + Vite + Tailwind |

**Arhitektura:** Clean Architecture (Domain → Application → Infrastructure → API), CQRS preko MediatR-a.

**Baza:** PostgreSQL (`Npgsql.EntityFrameworkCore.PostgreSQL`).

- .NET 10 SDK
- Node.js 20+
- PostgreSQL 

Studentski projekat — **Projekat 12, DevReview**.  
Backend: ASP.NET Core · Frontend: React + Vite.
