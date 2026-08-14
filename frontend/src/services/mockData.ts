export type UserRole = 'Admin' | 'Teacher' | 'Student';

export interface User {
  id: string;
  name: string;
  email: string;
  role: UserRole;
  avatarUrl?: string;
}

export interface Course {
  id: string;
  code: string;
  title: string;
  teacherId: string;
  teacherName: string;
  enrolledStudentsCount: number;
}

export interface Assignment {
  id: string;
  courseId: string;
  courseTitle: string;
  teacherId: string;
  title: string;
  description: string;
  dueDate: string; // ISO String
  maxMarks: number;
  createdAt: string;
}

export interface Submission {
  id: string;
  assignmentId: string;
  assignmentTitle: string;
  studentId: string;
  studentName: string;
  submittedAt: string;
  textAnswer?: string;
  fileName?: string;
  filePath?: string;
  fileSize?: string;
  marksObtained?: number;
  feedback?: string;
  status: 'submitted' | 'graded' | 'pending' | 'overdue';
}

export const INITIAL_USERS: User[] = [
  { id: 'u-1', name: 'Dr. Sarah Connor (Admin)', email: 'admin@school.edu', role: 'Admin' },
  { id: 'u-2', name: 'Prof. Alan Turing', email: 'turing@school.edu', role: 'Teacher' },
  { id: 'u-3', name: 'Prof. Margaret Hamilton', email: 'margaret@school.edu', role: 'Teacher' },
  { id: 'u-4', name: 'Alex Johnson (Student)', email: 'alex@student.edu', role: 'Student' },
  { id: 'u-5', name: 'Emily Davis (Student)', email: 'emily@student.edu', role: 'Student' },
];

export const INITIAL_COURSES: Course[] = [
  { id: 'c-101', code: 'CS101', title: 'Introduction to Computer Science', teacherId: 'u-2', teacherName: 'Prof. Alan Turing', enrolledStudentsCount: 28 },
  { id: 'c-201', code: 'CS201', title: 'Data Structures & Algorithms', teacherId: 'u-2', teacherName: 'Prof. Alan Turing', enrolledStudentsCount: 22 },
  { id: 'c-305', code: 'SE305', title: 'Software Architecture & Design', teacherId: 'u-3', teacherName: 'Prof. Margaret Hamilton', enrolledStudentsCount: 30 },
];

export const INITIAL_ASSIGNMENTS: Assignment[] = [
  {
    id: 'a-1',
    courseId: 'c-101',
    courseTitle: 'CS101 - Introduction to Computer Science',
    teacherId: 'u-2',
    title: 'Python Fundamentals & Logic Controls',
    description: 'Implement a CLI app solving standard control structure problems and submit your python file along with design notes.',
    dueDate: new Date(Date.now() + 86400000 * 3).toISOString(), // 3 days from now
    maxMarks: 100,
    createdAt: new Date(Date.now() - 86400000 * 2).toISOString(),
  },
  {
    id: 'a-2',
    courseId: 'c-201',
    courseTitle: 'CS201 - Data Structures & Algorithms',
    teacherId: 'u-2',
    title: 'Binary Search Tree & Graph Traversals',
    description: 'Provide asymptotic time complexity analyses and implement BFS / DFS algorithms in C++ or C#.',
    dueDate: new Date(Date.now() + 86400000 * 5).toISOString(), // 5 days from now
    maxMarks: 50,
    createdAt: new Date(Date.now() - 86400000 * 1).toISOString(),
  },
  {
    id: 'a-3',
    courseId: 'c-305',
    courseTitle: 'SE305 - Software Architecture & Design',
    teacherId: 'u-3',
    title: 'Microservices & REST API Specification',
    description: 'Design an OpenAPI 3.0 specification for an e-commerce catalog API and outline the database schema.',
    dueDate: new Date(Date.now() - 86400000 * 1).toISOString(), // Overdue / Past due
    maxMarks: 100,
    createdAt: new Date(Date.now() - 86400000 * 7).toISOString(),
  },
];

export const INITIAL_SUBMISSIONS: Submission[] = [
  {
    id: 'sub-1',
    assignmentId: 'a-3',
    assignmentTitle: 'Microservices & REST API Specification',
    studentId: 'u-4',
    studentName: 'Alex Johnson (Student)',
    submittedAt: new Date(Date.now() - 86400000 * 2).toISOString(),
    textAnswer: 'I have designed the OpenAPI specification with 12 endpoints including swagger documentation links.',
    fileName: 'openapi_schema_alex.json',
    fileSize: '45 KB',
    marksObtained: 92,
    feedback: 'Excellent schema design! Clean separation of concerns in the data transfer objects.',
    status: 'graded',
  },
  {
    id: 'sub-2',
    assignmentId: 'a-1',
    assignmentTitle: 'Python Fundamentals & Logic Controls',
    studentId: 'u-4',
    studentName: 'Alex Johnson (Student)',
    submittedAt: new Date(Date.now() - 3600000 * 4).toISOString(),
    textAnswer: 'Here is my implementation of the CLI calculator and logic analyzer.',
    fileName: 'assignment1_solution.py',
    fileSize: '12 KB',
    status: 'submitted',
  },
];

// Helper class for Mock Store
class MockStore {
  private getItem<T>(key: string, defaultValue: T): T {
    if (typeof window === 'undefined') return defaultValue;
    const data = localStorage.getItem(`asm_${key}`);
    return data ? JSON.parse(data) : defaultValue;
  }

  private setItem<T>(key: string, value: T): void {
    if (typeof window === 'undefined') return;
    localStorage.setItem(`asm_${key}`, JSON.stringify(value));
  }

  getUsers(): User[] {
    return this.getItem('users', INITIAL_USERS);
  }

  saveUsers(users: User[]): void {
    this.setItem('users', users);
  }

  getCourses(): Course[] {
    return this.getItem('courses', INITIAL_COURSES);
  }

  saveCourses(courses: Course[]): void {
    this.setItem('courses', courses);
  }

  getAssignments(): Assignment[] {
    return this.getItem('assignments', INITIAL_ASSIGNMENTS);
  }

  saveAssignments(assignments: Assignment[]): void {
    this.setItem('assignments', assignments);
  }

  getSubmissions(): Submission[] {
    return this.getItem('submissions', INITIAL_SUBMISSIONS);
  }

  saveSubmissions(submissions: Submission[]): void {
    this.setItem('submissions', submissions);
  }
}

export const mockStore = new MockStore();
