# DevReview Frontend

React + TypeScript frontend za DevReview platformu.

## Pokretanje

```bash
npm install
npm run dev
```

Frontend se pokreće na `http://localhost:5173`. Backend mora biti pokrenut na `http://localhost:5147`.

## Struktura

```
src/
  api/         - axios klijent i funkcije za pozivanje API-ja
  components/  - UI komponente (common, layout, reviewRequests, comments, ...)
  hooks/       - useAuth hook
  layouts/     - AppLayout, AuthLayout
  pages/       - sve stranice (Login, Register, Reviews, Mentors, ...)
  routes/      - router konfiguracija, zaštićene rute
  store/       - Zustand auth store
  types/       - TypeScript tipovi
  utils/       - helper funkcije (format, constants, codeDiff, ...)
```
