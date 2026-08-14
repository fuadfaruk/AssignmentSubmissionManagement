# Assignment & Submission Management System

A full-stack, role-based web application for managing academic assignments, courses, and student submissions. Featuring an ASP.NET Core Web API backend and a state-of-the-art **Neumorphic (Soft UI) Next.js** frontend supporting both **Off White Light Mode** and **Matte Black Dark Mode**.

---

## 🚀 Technology Stack

| Layer | Technology |
|---|---|
| **Frontend Framework** | Next.js (React 19, App Router) + TypeScript |
| **Frontend Styling** | Tailwind CSS v4 + Custom Neumorphism Design System |
| **Icons & Utilities** | Lucide React, `clsx`, `tailwind-merge` |
| **Backend API** | ASP.NET Core Web API (C#) |
| **Database** | PostgreSQL (via EF Core + Npgsql) |
| **ORM** | Entity Framework Core |
| **Authentication** | JWT Bearer tokens & Dual-Mode Client Auth Engine |
| **API Documentation** | Swagger / OpenAPI |
| **Testing** | xUnit + Moq + FluentAssertions |

---

## 📂 Project Structure

```
AssignmentSubmissionManagement/
├── frontend/                                         # Next.js Frontend Application
│   ├── src/
│   │   ├── app/                                     # App Router (layout, page, globals.css with Neumorphic tokens)
│   │   ├── components/
│   │   │   ├── ui/                                  # NeumorphicCard, NeumorphicButton, NeumorphicBadge, ThemeToggle, DemoRoleSwitcher
│   │   │   ├── layout/                              # Header, Sidebar navigation
│   │   │   ├── student/                             # StudentView, DeadlineCountdown, SubmissionModal (Drag & Drop uploader)
│   │   │   ├── teacher/                             # TeacherView, CreateAssignmentModal, GradingDrawer
│   │   │   └── admin/                               # AdminView (User CRUD, Course Management)
│   │   ├── context/                                 # AuthContext (Role switching & state)
│   │   └── services/                                # mockData.ts (localStorage engine) & apiClient.ts (REST wrapper)
│   ├── package.json
│   └── tsconfig.json
│
├── backend/                                          # ASP.NET Core Solution & Backend
│   ├── src/
│   │   ├── AssignmentSubmissionManagement.Api/      # Controllers & Startup pipeline
│   │   ├── AssignmentSubmissionManagement.Core/     # Domain entities, DTOs, interfaces, validators
│   │   └── AssignmentSubmissionManagement.Infrastructure/ # EF Core DataContext, Repositories, Seeder
│   ├── tests/
│   │   └── AssignmentSubmissionManagement.Tests/    # Unit tests
│   └── AssignmentSubmissionManagement.slnx        # Solution file
│
├── .env.example
└── README.md
```

---

## ✨ Frontend Features & Neumorphic Design System

### 🎨 Visual Aesthetics & Neumorphism
- **Dual Neumorphic Themes:**
  - **Light Mode:** Off White base (`#F4F5F8`) with dual drop shadow (`#D1D5DB`) and soft top light highlight (`#FFFFFF`).
  - **Dark Mode:** Matte Black base (`#141414`) with dark drop shadow (`#0A0A0A`) and soft top highlight (`#242424`).
- **Interactive Depressions:** Soft push button animations (`neu-button`, `neu-pressed`, `neu-flat`, `neu-input`).

### 🔄 Dual-Mode Auth & Service Layer
- **Demo Role Switcher:** Top header bar features instant role switching to preview views as **Admin**, **Teacher**, or **Student**.
- **Persistent Mock Store:** Built-in `mockData.ts` engine pre-seeded with demo accounts, active courses, assignments, and student submissions sync'd to `localStorage`.
- **API Client:** [`apiClient.ts`](file:///C:/Users/FuadFaruk/Desktop/Codex%20Projects/AssignmentSubmissionManagement/frontend/src/services/apiClient.ts) prepared for ASP.NET Core endpoints when backend integration is activated.

### 👥 Role-Based Interactive Dashboards
1. **Student Dashboard:**
   - Enrolled courses overview cards.
   - Assignment timeline with dynamic **Deadline Countdown Timers** (days, hours, minutes, seconds).
   - **Neumorphic Drag & Drop File Uploader** with text submission input.
   - Grade and teacher feedback inspector.
2. **Teacher Dashboard:**
   - **Assignment Creator Modal** for publishing coursework to enrolled classes with max marks and due dates.
   - **Slide-Over Grading Drawer** with submission file viewer, interactive score slider (0–Max Marks), and quick feedback snippets.
3. **Admin Dashboard:**
   - **User Accounts Directory** with real-time user addition and role deletion.
   - **School Classes & Courses** directory with instructor assignment controls.

---

## 🛠️ Quick Setup & Local Execution

### 1. Run the Frontend (Next.js)

```bash
cd frontend

# Install dependencies (if not already installed)
npm install

# Start the local development server
npm run dev
```

Open [http://localhost:3000](http://localhost:3000) in your web browser.

### 2. Run the Backend (ASP.NET Core Web API)

```bash
cd backend/src/AssignmentSubmissionManagement.Api

# Run database migrations / seeder & launch API
dotnet run
```

The API will be available at `http://localhost:5000` (or `https://localhost:5001`). Swagger UI is served at `/`.

### 3. Run Backend Unit Tests

```bash
cd backend/tests/AssignmentSubmissionManagement.Tests
dotnet test
```

---

## 🔑 Demo Credentials

| Role | Email | Password | Pre-seeded Mock Account |
|---|---|---|---|
| **Admin** | `admin@school.edu` | `Admin@123` | Dr. Sarah Connor (Admin) |
| **Teacher** | `turing@school.edu` | `Teacher@123` | Prof. Alan Turing |
| **Teacher** | `margaret@school.edu` | `Teacher@123` | Prof. Margaret Hamilton |
| **Student** | `alex@student.edu` | `Student@123` | Alex Johnson (Student) |

> **Tip:** You can switch roles instantly without password entry using the **Demo Role** toggle in the top navigation header.

---

## 📌 Business Rules Enforced

1. **Role-Based Access Control:** Role checks enforced across both UI components and API endpoints.
2. **Deadline Enforcement:** Real-time countdown timers warn students of upcoming due dates.
3. **Grading Ownership:** Teachers grade submissions with max marks validation and feedback documentation.
4. **User & Course Management:** Admin interface allows full creation and role management of school users and classes.
