# SportsBookingSystem

A multi-sport court booking platform where players can find parks, book courts, and invite friends to play together. Park managers review and approve bookings, while admins manage the overall platform.

> **Status:** Backend API complete. Angular frontend coming soon.

---

## What it does

The app connects three types of users:

- **Players** — search for parks, book courts, invite friends, respond to invites
- **Park Managers** — review and approve booking requests for their park
- **Admins** — manage parks, fields, and user roles across the platform

### Core features

**Booking flow**
Players create a booking for a specific court and time slot, then invite friends. Once every invited player accepts, the booking moves to the park manager for final approval. If anyone declines, the booking is automatically cancelled. Bookings that haven't been confirmed close enough to the start time are timed out automatically.

**Sports supported**
- Football — 12 players
- Tennis — 2 or 4 players
- Basketball — Full Court (10 players) or Half Court (6 players, 75% of base price). Two half-court bookings can share the same slot; a full-court booking cannot.

**Friend system**
Players can send friend requests, accept or decline them. You can only invite accepted friends to a booking.

**Notifications**
Every booking event (approval, cancellation, timeout) generates in-app notifications for all relevant users. Unread notifications surface first.

**Park analytics**
Park managers and admins can view stats per park — total confirmed bookings, unique players, most played sport, busiest hour and weekday, top organizers.

**Admin tools**
Admins can create and update parks and fields, soft-delete them (historical bookings stay intact), and manage user roles.

---

## Tech stack

| Layer | Technology |
|---|---|
| Framework | .NET 9, ASP.NET Core Web API |
| Architecture | Clean Architecture (API / Application / Domain / Infrastructure) |
| Database | SQL Server (Docker), Entity Framework Core — Code First |
| Auth | JWT Bearer tokens, role-based authorization |
| Background jobs | Hangfire — recurring jobs (outbox processor, booking timeout) |
| Logging | Serilog — rolling daily log files, per-request user ID enrichment |
| Validation | FluentValidation |
| Patterns | CQRS, Outbox Pattern, Soft Delete, Audit Trail |
| Result handling | ErrorOr |

---

## Architecture highlights

- **Clean Architecture** — strict layer separation; the domain has zero external dependencies
- **CQRS** — every operation is either a Command or a Query with its own handler
- **Outbox Pattern** — domain events (booking confirmed, cancelled, timed out) are persisted atomically with the state change and processed asynchronously by a Hangfire job, so notifications are never lost
- **Soft delete** — parks and fields are never hard-deleted; historical bookings always have valid references
- **Audit trail** — `CreatedAt` and `UpdatedAt` are auto-set on every entity via a `SaveChangesAsync` override
- **Global exception middleware** — all unhandled exceptions are caught, logged with full context, and return a clean JSON error to the client