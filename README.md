# AI Resume Analyzer & Interview Coach

A full-stack AI-powered application built with .NET 9, designed to help job seekers analyze their resumes, match with job descriptions, and practice mock interviews with AI feedback.

## 🚀 Features

- **Resume Upload & Parsing** — PDF/DOCX text extraction
- **AI Resume Analysis** — Scores, strengths, weaknesses using Google Gemini
- **Job Description Matching** — Skill gap analysis with match scores
- **Mock Interview Coach** — AI-generated questions with personalized feedback
- **Interactive Dashboard** — Chart.js graphs showing progress over time

## 🛠️ Tech Stack

| Layer | Technology |
|-------|-----------|
| Backend API | ASP.NET Core Web API (.NET 9) |
| Frontend | ASP.NET Core MVC + Razor Views |
| Database | SQL Server + Entity Framework Core 9 |
| AI Integration | Google Gemini API (gemini-2.5-flash) |
| Authentication | JWT Bearer Tokens |
| File Parsing | PdfPig + DocumentFormat.OpenXml |
| UI Framework | Bootstrap 5 + Chart.js |
| Logging | Serilog |

## 📁 Solution Structure
ResumeAI/

├── ResumeAI.API           # Web API (controllers, middleware)

├── ResumeAI.Web           # MVC Frontend (views, controllers)

├── ResumeAI.Core          # Domain models, DTOs, interfaces

├── ResumeAI.Services      # Business logic, AI integration

└── ResumeAI.Infrastructure # EF Core, repositories, migrations## 

⚙️ Setup Instructions

### Prerequisites
- .NET 9 SDK
- SQL Server (Express or higher)
- Google Gemini API key (free at aistudio.google.com)
- Visual Studio 2022

### Steps

1. **Clone the repo**
```bash
git clone https://github.com/YourUsername/ResumeAI.git
cd ResumeAI
```

2. **Configure the API**

Copy `ResumeAI.API/appsettings.example.json` to `appsettings.json` and update:
- `ConnectionStrings:DefaultConnection` — your SQL Server instance
- `Jwt:Key` — any random 32+ character string
- `FileStorage:BasePath` — local folder for uploaded resumes

3. **Set Gemini API key** (using User Secrets)
```bash
dotnet user-secrets set "Gemini:ApiKey" "your-key-here" --project ResumeAI.API
```

4. **Run migrations**
```bash
dotnet ef database update --project ResumeAI.Infrastructure --startup-project ResumeAI.API
```

5. **Run the application**
```bash
dotnet run --project ResumeAI.API
dotnet run --project ResumeAI.Web
```

6. **Demo accounts** (auto-seeded on first run)

| Role | Email | Password |
|------|-------|----------|
| Candidate | alex@resumeai.com | Candidate123! |
| Recruiter | recruiter@resumeai.com | Recruiter123! |
| Admin | admin@resumeai.com | Admin123! |

## 📸 Screenshots

*Dashboard with score trends and interview history*

*Resume analysis with radar chart*

*Mock interview with AI feedback*

## 📄 License
MIT License
| Logging | Serilog |

## 📁 Solution Structure
