
using Microsoft.EntityFrameworkCore;
using ResumeAI.Core.Entities;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;

namespace ResumeAI.Infrastructure.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        if (await db.Users.AnyAsync(u => u.Email == "alex@resumeai.com")) return;
        var hasher = new PasswordHasher<User>();

        // ── Users ──────────────────────────────────────────────
        var admin = new User
        {
            Id = Guid.NewGuid(),
            FullName = "Admin User",
            Email = "admin@resumeai.com",
            Role = "Admin",
            IsActive = true,
            CreatedAt = DateTime.UtcNow.AddDays(-30)
        };
        admin.PasswordHash = hasher.HashPassword(admin, "Admin123!");

        var recruiter = new User
        {
            Id = Guid.NewGuid(),
            FullName = "Sarah Johnson",
            Email = "recruiter@resumeai.com",
            Role = "Recruiter",
            IsActive = true,
            CreatedAt = DateTime.UtcNow.AddDays(-20)
        };
        recruiter.PasswordHash = hasher.HashPassword(recruiter, "Recruiter123!");

        var candidate = new User
        {
            Id = Guid.NewGuid(),
            FullName = "Alex Kumar",
            Email = "alex@resumeai.com",
            Role = "Candidate",
            IsActive = true,
            CreatedAt = DateTime.UtcNow.AddDays(-10)
        };
        candidate.PasswordHash = hasher.HashPassword(candidate, "Candidate123!");

        await db.Users.AddRangeAsync(admin, recruiter, candidate);
        await db.SaveChangesAsync();

        // ── Job Descriptions ───────────────────────────────────
        var jobDotNet = new JobDescription
        {
            Id = Guid.NewGuid(),
            UserId = recruiter.Id,
            Title = "Senior .NET Developer",
            Company = "TechCorp Solutions",
            Description = "We are looking for an experienced .NET developer to join our growing team. You will be responsible for designing, developing, and maintaining enterprise-level web applications using ASP.NET Core, C#, and SQL Server. Experience with microservices, REST APIs, and cloud platforms like Azure is highly desirable.",
            RequiredSkills = JsonSerializer.Serialize(new List<string> { "C#", "ASP.NET Core", "SQL Server", "Entity Framework", "REST API", "Azure", "Docker", "Microservices" }),
            CreatedAt = DateTime.UtcNow.AddDays(-15)
        };

        var jobFullStack = new JobDescription
        {
            Id = Guid.NewGuid(),
            UserId = recruiter.Id,
            Title = "Full Stack Developer",
            Company = "Digital Innovations Ltd",
            Description = "Seeking a talented Full Stack Developer with expertise in both frontend and backend technologies. The ideal candidate will have strong experience with React, Node.js, and .NET Core. You will work in an agile team to deliver high-quality software solutions.",
            RequiredSkills = JsonSerializer.Serialize(new List<string> { "React", "JavaScript", "C#", "ASP.NET Core", "SQL Server", "HTML/CSS", "Git", "Agile" }),
            CreatedAt = DateTime.UtcNow.AddDays(-12)
        };

        var jobDataEngineer = new JobDescription
        {
            Id = Guid.NewGuid(),
            UserId = recruiter.Id,
            Title = "Data Engineer",
            Company = "DataFlow Analytics",
            Description = "Join our data engineering team to build and maintain scalable data pipelines. You will work with large datasets, design ETL processes, and collaborate with data scientists to deliver insights. Experience with Python, SQL, and cloud data platforms is essential.",
            RequiredSkills = JsonSerializer.Serialize(new List<string> { "Python", "SQL", "Azure Data Factory", "Spark", "ETL", "Power BI", "REST API" }),
            CreatedAt = DateTime.UtcNow.AddDays(-10)
        };

        var jobDevOps = new JobDescription
        {
            Id = Guid.NewGuid(),
            UserId = recruiter.Id,
            Title = "DevOps Engineer",
            Company = "CloudSystems Inc",
            Description = "We are hiring a DevOps Engineer to manage our cloud infrastructure and CI/CD pipelines. You will work closely with development teams to automate deployments and ensure system reliability. Strong knowledge of Azure, Docker, Kubernetes, and infrastructure as code is required.",
            RequiredSkills = JsonSerializer.Serialize(new List<string> { "Azure", "Docker", "Kubernetes", "CI/CD", "Terraform", "Linux", "Git", "Monitoring" }),
            CreatedAt = DateTime.UtcNow.AddDays(-8)
        };

        var jobMobile = new JobDescription
        {
            Id = Guid.NewGuid(),
            UserId = recruiter.Id,
            Title = "Mobile App Developer",
            Company = "AppStudio Pro",
            Description = "Looking for a passionate Mobile Developer to build cross-platform mobile applications. You will design and develop feature-rich mobile apps using Flutter or React Native, integrate with REST APIs, and ensure excellent user experience across iOS and Android platforms.",
            RequiredSkills = JsonSerializer.Serialize(new List<string> { "Flutter", "Dart", "React Native", "REST API", "Firebase", "iOS", "Android", "Git" }),
            CreatedAt = DateTime.UtcNow.AddDays(-5)
        };

        await db.JobDescriptions.AddRangeAsync(jobDotNet, jobFullStack, jobDataEngineer, jobDevOps, jobMobile);
        await db.SaveChangesAsync();

        // ── Resumes ────────────────────────────────────────────
        var resume1 = new Resume
        {
            Id = Guid.NewGuid(),
            UserId = candidate.Id,
            FileName = "Alex_Kumar_DotNet_Developer.pdf",
            FilePath = "C:\\ResumeAI_Uploads\\seeded\\alex_resume.pdf",
            FileType = "PDF",
            Status = "Analyzed",
            UploadedAt = DateTime.UtcNow.AddDays(-9),
            ExtractedText = @"ALEX KUMAR
Senior .NET Developer | alex@email.com | +91-9876543210 | LinkedIn: linkedin.com/in/alexkumar

PROFESSIONAL SUMMARY
Experienced .NET Developer with 5+ years of expertise in building enterprise-grade web applications 
using C#, ASP.NET Core, and SQL Server. Strong background in REST API development, microservices 
architecture, and cloud deployments on Azure. Proven track record of delivering scalable solutions 
in agile environments.

TECHNICAL SKILLS
Languages: C#, JavaScript, TypeScript, Python
Frameworks: ASP.NET Core, Entity Framework Core, React, Angular
Databases: SQL Server, PostgreSQL, MongoDB, Redis
Cloud & DevOps: Azure, Docker, Kubernetes, CI/CD, Git
Tools: Visual Studio, VS Code, Postman, Jira, Azure DevOps

WORK EXPERIENCE
Senior .NET Developer | TechSoft Pvt Ltd | 2021 - Present
- Designed and developed microservices architecture using ASP.NET Core and Docker
- Built REST APIs consumed by 50,000+ daily active users
- Optimized SQL Server queries reducing response time by 40%
- Implemented JWT authentication and role-based authorization
- Led a team of 4 developers, conducted code reviews and mentoring

.NET Developer | Infosys Ltd | 2019 - 2021
- Developed enterprise web applications using ASP.NET MVC and Entity Framework
- Created stored procedures and optimized database performance
- Integrated third-party payment APIs (Razorpay, PayPal)
- Worked in Agile/Scrum methodology with 2-week sprints

EDUCATION
B.Tech in Computer Science | Gujarat Technological University | 2015-2019 | CGPA: 8.2

PROJECTS
1. E-Commerce Platform (ASP.NET Core + React + SQL Server + Azure)
   - Built full-stack e-commerce solution with payment gateway integration
   - Deployed on Azure App Service with CI/CD pipeline

2. Hospital Management System (ASP.NET Core + EF Core + SQL Server)
   - Developed patient management, appointment scheduling, billing modules
   - Implemented real-time notifications using SignalR

CERTIFICATIONS
- Microsoft Certified: Azure Developer Associate (AZ-204)
- Microsoft Certified: .NET Developer Fundamentals"
        };

        var resume2 = new Resume
        {
            Id = Guid.NewGuid(),
            UserId = candidate.Id,
            FileName = "Alex_Kumar_Full_Stack.pdf",
            FilePath = "C:\\ResumeAI_Uploads\\seeded\\alex_fullstack_resume.pdf",
            FileType = "PDF",
            Status = "Analyzed",
            UploadedAt = DateTime.UtcNow.AddDays(-7),
            ExtractedText = @"ALEX KUMAR
Full Stack Developer | alex@email.com | GitHub: github.com/alexkumar

SUMMARY
Versatile Full Stack Developer with expertise in React, ASP.NET Core, and cloud technologies.
Passionate about building scalable, user-friendly applications with clean code practices.

SKILLS
Frontend: React, JavaScript, TypeScript, HTML5, CSS3, Bootstrap, Redux
Backend: C#, ASP.NET Core, Node.js, REST APIs, GraphQL
Database: SQL Server, MongoDB, PostgreSQL
DevOps: Git, Docker, Azure, CI/CD Pipelines
Agile: Scrum, Kanban, JIRA

EXPERIENCE
Full Stack Developer | WebTech Solutions | 2020 - Present
- Built 10+ React applications with complex state management
- Developed RESTful APIs using ASP.NET Core with JWT authentication
- Implemented real-time features using SignalR and WebSockets
- Integrated payment gateways and third-party APIs

Junior Developer | StartupHub | 2019 - 2020
- Developed responsive web applications using React and Bootstrap
- Built REST APIs with Node.js and Express
- Managed SQL Server databases and wrote optimized queries

EDUCATION
B.E. Computer Engineering | 2015-2019

PROJECTS
- Task Management App (React + ASP.NET Core + SQL Server)
- Social Media Dashboard (React + Node.js + MongoDB)
- E-Learning Platform (ASP.NET Core + React + Azure)"
        };

        var resume3 = new Resume
        {
            Id = Guid.NewGuid(),
            UserId = admin.Id,
            FileName = "Sarah_Johnson_Recruiter.pdf",
            FilePath = "C:\\ResumeAI_Uploads\\seeded\\sarah_resume.pdf",
            FileType = "PDF",
            Status = "Parsed",
            UploadedAt = DateTime.UtcNow.AddDays(-5),
            ExtractedText = @"SARAH JOHNSON
HR Recruiter & Talent Acquisition Specialist
sarah@email.com | LinkedIn: linkedin.com/in/sarahjohnson

SUMMARY
Dynamic HR professional with 7+ years in talent acquisition, specializing in tech recruitment.
Expert in sourcing, screening, and onboarding top engineering talent.

SKILLS
Talent Acquisition, Technical Recruiting, LinkedIn Sourcing
ATS Systems (Workday, Greenhouse), HR Analytics
Employee Onboarding, Performance Management
Communication, Negotiation, Team Leadership

EXPERIENCE
Senior Talent Acquisition Lead | TechCorp | 2018 - Present
- Hired 200+ engineers across .NET, React, DevOps roles
- Reduced time-to-hire by 35% through process optimization
- Built relationships with top engineering universities

HR Manager | MidSoft Solutions | 2016 - 2018
- Managed full-cycle recruitment for 50+ open positions
- Implemented new onboarding program improving retention by 20%

EDUCATION
MBA in Human Resources | 2014-2016
B.Com | 2011-2014"
        };

        await db.Resumes.AddRangeAsync(resume1, resume2, resume3);
        await db.SaveChangesAsync();

        // ── Analysis Results ───────────────────────────────────
        var analysis1 = new AnalysisResult
        {
            Id = Guid.NewGuid(),
            ResumeId = resume1.Id,
            JobDescriptionId = jobDotNet.Id,
            OverallScore = 88,
            SkillScore = 92,
            ExperienceScore = 85,
            FormatScore = 90,
            Strengths = JsonSerializer.Serialize(new List<string>
            {
                "Strong C# and ASP.NET Core expertise with 5+ years of hands-on experience",
                "Azure certification (AZ-204) demonstrates cloud competency",
                "Leadership experience managing a team of 4 developers"
            }),
            Weaknesses = JsonSerializer.Serialize(new List<string>
            {
                "Limited exposure to Kubernetes in production environments",
                "No mention of unit testing frameworks like xUnit or NUnit",
                "Could elaborate more on microservices communication patterns"
            }),
            Suggestions = JsonSerializer.Serialize(new List<string>
            {
                "Add specific metrics for the microservices project (requests/sec, uptime)",
                "Include GitHub profile URL to showcase open-source contributions",
                "Mention experience with message brokers like RabbitMQ or Azure Service Bus"
            }),
            AISummary = "Alex Kumar presents as a strong Senior .NET Developer candidate with extensive experience in enterprise application development. The resume demonstrates solid technical depth in C#, ASP.NET Core, and Azure, complemented by leadership experience. The candidate's background aligns very well with modern .NET development practices and would be a strong fit for senior-level positions.",
            AnalyzedAt = DateTime.UtcNow.AddDays(-8)
        };

        var analysis2 = new AnalysisResult
        {
            Id = Guid.NewGuid(),
            ResumeId = resume2.Id,
            JobDescriptionId = jobFullStack.Id,
            OverallScore = 82,
            SkillScore = 85,
            ExperienceScore = 78,
            FormatScore = 88,
            Strengths = JsonSerializer.Serialize(new List<string>
            {
                "Excellent React and frontend development skills",
                "Good balance of frontend and backend technologies",
                "Real-world project experience with diverse tech stack"
            }),
            Weaknesses = JsonSerializer.Serialize(new List<string>
            {
                "Limited mention of Agile methodology experience",
                "No specific metrics or KPIs mentioned for projects",
                "TypeScript experience could be highlighted more"
            }),
            Suggestions = JsonSerializer.Serialize(new List<string>
            {
                "Quantify achievements with specific numbers and metrics",
                "Add more detail about Agile/Scrum experience",
                "Consider adding a portfolio link or GitHub profile"
            }),
            AISummary = "Alex's Full Stack profile shows a well-rounded developer comfortable across the entire web stack. Strong React fundamentals combined with ASP.NET Core backend skills make this candidate suitable for full-stack roles. Additional focus on quantifiable achievements would strengthen the profile significantly.",
            AnalyzedAt = DateTime.UtcNow.AddDays(-6)
        };

        var analysis3 = new AnalysisResult
        {
            Id = Guid.NewGuid(),
            ResumeId = resume1.Id,
            JobDescriptionId = null,
            OverallScore = 85,
            SkillScore = 88,
            ExperienceScore = 82,
            FormatScore = 87,
            Strengths = JsonSerializer.Serialize(new List<string>
            {
                "Well-structured resume with clear sections and formatting",
                "Strong technical skill set covering full .NET ecosystem",
                "Good mix of individual contributor and leadership experience"
            }),
            Weaknesses = JsonSerializer.Serialize(new List<string>
            {
                "Resume could benefit from a more prominent summary statement",
                "Certifications section could include dates of certification",
                "Consider adding soft skills section"
            }),
            Suggestions = JsonSerializer.Serialize(new List<string>
            {
                "Add a LinkedIn profile URL for better visibility",
                "Include open-source contributions or side projects",
                "Add specific technologies used in each project"
            }),
            AISummary = "This is a strong .NET developer resume with excellent technical coverage and relevant work experience. The candidate demonstrates progressive career growth from developer to team lead, which is impressive. With minor improvements to quantify achievements and add online presence, this resume would score even higher.",
            AnalyzedAt = DateTime.UtcNow.AddDays(-4)
        };

        await db.AnalysisResults.AddRangeAsync(analysis1, analysis2, analysis3);
        await db.SaveChangesAsync();

        // ── Skill Matches ──────────────────────────────────────
        var skillMatches1 = new List<SkillMatch>
        {
            new() { AnalysisId = analysis1.Id, SkillName = "C#", IsMatched = true, MatchScore = 100, Category = "Technical" },
            new() { AnalysisId = analysis1.Id, SkillName = "ASP.NET Core", IsMatched = true, MatchScore = 100, Category = "Technical" },
            new() { AnalysisId = analysis1.Id, SkillName = "SQL Server", IsMatched = true, MatchScore = 100, Category = "Technical" },
            new() { AnalysisId = analysis1.Id, SkillName = "Entity Framework", IsMatched = true, MatchScore = 80, Category = "Technical" },
            new() { AnalysisId = analysis1.Id, SkillName = "REST API", IsMatched = true, MatchScore = 100, Category = "Technical" },
            new() { AnalysisId = analysis1.Id, SkillName = "Azure", IsMatched = true, MatchScore = 100, Category = "Technical" },
            new() { AnalysisId = analysis1.Id, SkillName = "Docker", IsMatched = true, MatchScore = 80, Category = "Technical" },
            new() { AnalysisId = analysis1.Id, SkillName = "Microservices", IsMatched = true, MatchScore = 80, Category = "Technical" }
        };

        var skillMatches2 = new List<SkillMatch>
        {
            new() { AnalysisId = analysis2.Id, SkillName = "React", IsMatched = true, MatchScore = 100, Category = "Technical" },
            new() { AnalysisId = analysis2.Id, SkillName = "JavaScript", IsMatched = true, MatchScore = 100, Category = "Technical" },
            new() { AnalysisId = analysis2.Id, SkillName = "C#", IsMatched = true, MatchScore = 100, Category = "Technical" },
            new() { AnalysisId = analysis2.Id, SkillName = "ASP.NET Core", IsMatched = true, MatchScore = 100, Category = "Technical" },
            new() { AnalysisId = analysis2.Id, SkillName = "SQL Server", IsMatched = true, MatchScore = 80, Category = "Technical" },
            new() { AnalysisId = analysis2.Id, SkillName = "HTML/CSS", IsMatched = true, MatchScore = 100, Category = "Technical" },
            new() { AnalysisId = analysis2.Id, SkillName = "Git", IsMatched = true, MatchScore = 80, Category = "Technical" },
            new() { AnalysisId = analysis2.Id, SkillName = "Agile", IsMatched = false, MatchScore = 40, Category = "Soft" }
        };

        await db.SkillMatches.AddRangeAsync(skillMatches1);
        await db.SkillMatches.AddRangeAsync(skillMatches2);
        await db.SaveChangesAsync();

        // ── Interview Sessions ─────────────────────────────────
        var session1 = new InterviewSession
        {
            Id = Guid.NewGuid(),
            UserId = candidate.Id,
            ResumeId = resume1.Id,
            JobTitle = "Senior .NET Developer",
            Difficulty = "Hard",
            Status = "Completed",
            TotalScore = 78,
            StartedAt = DateTime.UtcNow.AddDays(-7),
            CompletedAt = DateTime.UtcNow.AddDays(-7).AddMinutes(45)
        };

        var session2 = new InterviewSession
        {
            Id = Guid.NewGuid(),
            UserId = candidate.Id,
            ResumeId = resume2.Id,
            JobTitle = "Full Stack Developer",
            Difficulty = "Medium",
            Status = "Completed",
            TotalScore = 85,
            StartedAt = DateTime.UtcNow.AddDays(-4),
            CompletedAt = DateTime.UtcNow.AddDays(-4).AddMinutes(30)
        };

        var session3 = new InterviewSession
        {
            Id = Guid.NewGuid(),
            UserId = candidate.Id,
            ResumeId = resume1.Id,
            JobTitle = ".NET Developer",
            Difficulty = "Easy",
            Status = "Completed",
            TotalScore = 92,
            StartedAt = DateTime.UtcNow.AddDays(-2),
            CompletedAt = DateTime.UtcNow.AddDays(-2).AddMinutes(25)
        };

        await db.InterviewSessions.AddRangeAsync(session1, session2, session3);
        await db.SaveChangesAsync();

        // ── Interview Questions ────────────────────────────────
        var questions1 = new List<InterviewQuestion>
        {
            new() {
                SessionId = session1.Id, OrderIndex = 1, QuestionType = "Technical",
                QuestionText = "Explain the difference between ASP.NET Core middleware and filters. When would you use each?",
                UserAnswer = "Middleware handles requests/responses at the pipeline level for cross-cutting concerns like logging, auth, and CORS. Filters are action-level and handle concerns specific to MVC like model validation and exception handling.",
                AIFeedback = "Good answer demonstrating solid understanding of ASP.NET Core pipeline. You correctly identified the scope difference. Consider mentioning specific middleware examples like UseAuthentication, UseAuthorization for extra depth.",
                Score = 82
            },
            new() {
                SessionId = session1.Id, OrderIndex = 2, QuestionType = "Technical",
                QuestionText = "How would you optimize a slow SQL Server query that's causing performance issues in production?",
                UserAnswer = "I would start by examining the execution plan, check for missing indexes, avoid SELECT *, use proper JOINs instead of subqueries, consider query caching, and potentially add pagination for large result sets.",
                AIFeedback = "Comprehensive answer covering the key optimization strategies. Execution plan analysis is the right starting point. You could also mention statistics updates, index fragmentation, and parameter sniffing issues for a complete answer.",
                Score = 88
            },
            new() {
                SessionId = session1.Id, OrderIndex = 3, QuestionType = "Behavioral",
                QuestionText = "Describe a time when you had to lead a team through a difficult technical challenge.",
                UserAnswer = "At TechSoft, we had a critical API performance issue affecting thousands of users. I coordinated with the team to identify bottlenecks, assigned tasks based on strengths, implemented caching strategies, and we resolved the issue within 48 hours.",
                AIFeedback = "Strong behavioral answer using the STAR format effectively. The specific mention of 48-hour resolution and thousands of users affected adds credibility. Good demonstration of leadership and technical problem-solving.",
                Score = 85
            },
            new() {
                SessionId = session1.Id, OrderIndex = 4, QuestionType = "Technical",
                QuestionText = "Explain microservices communication patterns. What are the tradeoffs between synchronous and asynchronous communication?",
                UserAnswer = "Synchronous communication via REST/gRPC is simpler but creates tight coupling and can cause cascading failures. Asynchronous messaging via RabbitMQ or Azure Service Bus provides better resilience and decoupling but adds complexity.",
                AIFeedback = "Excellent technical answer showing deep understanding of distributed systems. The tradeoff analysis is accurate. Mentioning saga pattern or event sourcing would have elevated this to an outstanding response.",
                Score = 80
            },
            new() {
                SessionId = session1.Id, OrderIndex = 5, QuestionType = "HR",
                QuestionText = "Where do you see yourself in 5 years? How does this role align with your career goals?",
                UserAnswer = "I see myself as a solutions architect or technical lead overseeing complex enterprise systems. This Senior .NET Developer role aligns perfectly as it offers opportunities to work with cutting-edge technologies and mentor junior developers.",
                AIFeedback = "Well-structured answer showing clear career vision and genuine interest in the role. The connection between the role and career goals is well-articulated. Shows ambition balanced with realistic expectations.",
                Score = 75
            }
        };

        var questions2 = new List<InterviewQuestion>
        {
            new() {
                SessionId = session2.Id, OrderIndex = 1, QuestionType = "Technical",
                QuestionText = "How do you manage state in a large React application? Compare Context API vs Redux.",
                UserAnswer = "For small-medium apps, Context API is sufficient for avoiding prop drilling. Redux is better for complex state with many components needing the same data, time-travel debugging, and predictable state management with actions and reducers.",
                AIFeedback = "Excellent answer showing practical React knowledge. The comparison is accurate and well-reasoned. Could also mention newer alternatives like Zustand or Jotai for bonus points.",
                Score = 90
            },
            new() {
                SessionId = session2.Id, OrderIndex = 2, QuestionType = "Technical",
                QuestionText = "Explain RESTful API design principles and best practices for versioning.",
                UserAnswer = "REST APIs should use proper HTTP verbs (GET, POST, PUT, DELETE), meaningful URIs, appropriate status codes, and be stateless. For versioning, I prefer URL versioning (/api/v1/) for simplicity, though header versioning is also common.",
                AIFeedback = "Solid answer covering core REST principles. Good mention of HTTP verbs and versioning strategies. Consider adding HATEOAS, pagination patterns, and error response standardization for a more complete answer.",
                Score = 85
            },
            new() {
                SessionId = session2.Id, OrderIndex = 3, QuestionType = "Behavioral",
                QuestionText = "Tell me about a time you had to learn a new technology quickly to meet a project deadline.",
                UserAnswer = "When our project required SignalR for real-time features, I had 2 weeks to learn it. I used Microsoft docs, built a small prototype, and successfully implemented real-time notifications that improved user engagement by 30%.",
                AIFeedback = "Excellent behavioral response with specific details and measurable outcome. The 30% improvement metric adds strong credibility. Shows initiative and ability to learn independently under pressure.",
                Score = 88
            },
            new() {
                SessionId = session2.Id, OrderIndex = 4, QuestionType = "Technical",
                QuestionText = "How do you ensure code quality in a team environment?",
                UserAnswer = "I enforce code quality through code reviews, enforcing coding standards with tools like StyleCop and SonarQube, writing unit tests with xUnit, maintaining >80% code coverage, and CI/CD pipelines that fail on quality gates.",
                AIFeedback = "Comprehensive answer covering multiple dimensions of code quality. The mention of specific tools and 80% coverage target shows practical experience. Integration with CI/CD is an excellent addition.",
                Score = 88
            },
            new() {
                SessionId = session2.Id, OrderIndex = 5, QuestionType = "HR",
                QuestionText = "Why are you looking to leave your current position?",
                UserAnswer = "I'm seeking new challenges and opportunities to grow. My current role has been great for learning fundamentals, but I'm ready for a more complex environment where I can work on larger-scale applications and contribute to architectural decisions.",
                AIFeedback = "Professional and positive response that focuses on growth rather than negativity about current employer. Shows ambition and maturity. Connecting this to the specific opportunities at this company would strengthen the answer.",
                Score = 75
            }
        };

        var questions3 = new List<InterviewQuestion>
        {
            new() {
                SessionId = session3.Id, OrderIndex = 1, QuestionType = "Technical",
                QuestionText = "What is dependency injection and why is it important in .NET?",
                UserAnswer = "Dependency injection is a design pattern where dependencies are provided from outside rather than created inside a class. It's crucial for testability, loose coupling, and following SOLID principles. .NET Core has built-in DI container.",
                AIFeedback = "Excellent, concise answer covering all key aspects. The mention of SOLID principles and .NET's built-in DI shows good understanding of the .NET ecosystem.",
                Score = 95
            },
            new() {
                SessionId = session3.Id, OrderIndex = 2, QuestionType = "Technical",
                QuestionText = "Explain the differences between async/await and Task in C#.",
                UserAnswer = "Task represents an asynchronous operation. async/await is syntactic sugar that makes async code readable. await suspends execution without blocking the thread, allowing better scalability especially in web applications handling many concurrent requests.",
                AIFeedback = "Clear and accurate explanation. The focus on thread non-blocking and scalability shows practical understanding beyond just syntax. Well done.",
                Score = 92
            },
            new() {
                SessionId = session3.Id, OrderIndex = 3, QuestionType = "Behavioral",
                QuestionText = "How do you handle disagreements with team members about technical decisions?",
                UserAnswer = "I believe in data-driven discussions. I present my reasoning with evidence, listen to alternatives with an open mind, and when needed, we prototype both approaches and measure results. The best technical solution should win, not the loudest voice.",
                AIFeedback = "Mature and professional answer showing strong teamwork and communication skills. The prototype-and-measure approach demonstrates pragmatism over ego.",
                Score = 90
            },
            new() {
                SessionId = session3.Id, OrderIndex = 4, QuestionType = "Technical",
                QuestionText = "What design patterns do you commonly use in .NET development?",
                UserAnswer = "Repository pattern for data access, Unit of Work for transactions, Factory for object creation, Strategy for interchangeable algorithms, Observer/events for loose coupling, and Decorator for extending functionality without inheritance.",
                AIFeedback = "Impressive breadth of design pattern knowledge. Each pattern is correctly described. Showing awareness of when NOT to use patterns would make this answer perfect.",
                Score = 95
            },
            new() {
                SessionId = session3.Id, OrderIndex = 5, QuestionType = "HR",
                QuestionText = "What motivates you as a developer?",
                UserAnswer = "I'm motivated by solving complex problems that have real impact. Seeing an application I built being used by thousands of people is incredibly rewarding. I also enjoy mentoring junior developers and contributing to a strong team culture.",
                AIFeedback = "Authentic and enthusiastic answer that connects personal motivation to tangible impact. The mention of mentoring shows leadership potential beyond just technical skills.",
                Score = 88
            }
        };

        await db.InterviewQuestions.AddRangeAsync(questions1);
        await db.InterviewQuestions.AddRangeAsync(questions2);
        await db.InterviewQuestions.AddRangeAsync(questions3);
        await db.SaveChangesAsync();
    }
}