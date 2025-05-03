# Auth Implementation

A modern, extensible authentication API built with **.NET 9 Minimal APIs**, following the **CQRS pattern** with **MediatR**, supporting both **JWT**-based authentication and **Google SSO**.

## ✨ Features

- 🔐 **JWT Authentication**
  - Access tokens
  - HttpOnly refresh tokens (stored in cookies)

- 🧑‍💻 **Authentication Flows**
  - Register
  - Login
  - Logout
  - Refresh Tokens

- 🔁 **Password Management**
  - Forgot Password (email-based token)
  - Reset Password
  - Change Password (authenticated)

- 🌐 **Google SSO**
  - Google OAuth 2.0 login

- 🧭 **Minimal API structure**
  - Clean endpoint definitions
  - Vertical Slice Architecture

## 🧰 Tech Stack

- [.NET 9 Minimal APIs](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis)
- [MediatR](https://github.com/jbogard/MediatR) for CQRS-style request handling
- JWT Bearer authentication
- Google OAuth 2.0
- FluentValidation (if used — add if applicable)
- Custom result pattern for consistent responses

## 📁 Endpoints

| Method | Route                     | Description                 |
|--------|---------------------------|-----------------------------|
| POST   | `/api/auth/register`      | Register a new user        |
| POST   | `/api/auth/login`         | Login with email/password  |
| POST   | `/api/auth/logout`        | Logout and delete cookie   |
| POST   | `/api/auth/refresh-tokens`| Refresh JWT tokens         |
| POST   | `/api/auth/forgot-password` | Send password reset link |
| PUT   | `/api/auth/reset-password`  | Reset password using token |
| PUT   | `/api/auth/change-password` | Change password (auth)     |
| POST   | `/api/auth/google-login`        | Login with Google          |

> Note: Refresh tokens are stored as secure, HttpOnly cookies.
