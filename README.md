# CodeCraft

CodeCraft is a full-stack coding playground where users create and share HTML/CSS/JS crafts.

## Stack
- Backend: ASP.NET Core 8, EF Core 8, SQL Server, JWT auth, Swagger
- Frontend: Next.js 13 + TypeScript + Tailwind + shadcn/ui
- Local Dev: Docker Compose

## Features
- Register/login and current-user APIs
- Public crafts gallery
- Create/update/delete crafts
- Public/private visibility rules
- Like/unlike and fork support
- Live preview iframe with tightened sandbox

## Local setup
1. Copy `.env.example` values into your environment.
2. Start services: `docker compose up --build`.
3. Open UI at `http://localhost:3000`.
4. Open API Swagger at `http://localhost:5023/swagger`.

## Backend env vars
- `CONNECTION_STRING`
- `JWT_ISSUER`
- `JWT_AUDIENCE`
- `JWT_SIGNING_KEY`

See `api/appsettings.example.json` for local equivalents.

## Security notes
- JWT secrets are environment-driven.
- No production secrets are committed.
- Preview iframe sandbox allows scripts only.

## CI
- `.github/workflows/backend.yml` builds backend on push/PR.
- `.github/workflows/frontend.yml` builds frontend on push/PR.

## Roadmap
- Expand integration tests for auth, ownership, and fork/like flows.
- Add deployment pipeline for Azure + Vercel.
