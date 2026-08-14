'use client';

import React, { useCallback, useEffect, useState } from 'react';
import { useAuth } from '../../context/AuthContext';
import { User, Course, mockStore, UserRole } from '../../services/mockData';
import { apiClient } from '../../services/apiClient';
import { NeumorphicCard } from '../ui/NeumorphicCard';
import { NeumorphicButton } from '../ui/NeumorphicButton';
import { NeumorphicBadge } from '../ui/NeumorphicBadge';
import { Users, BookOpen, Shield, Plus, Trash2, X } from 'lucide-react';

interface UserApiRecord {
  id: string;
  name?: string;
  firstName?: string;
  lastName?: string;
  email: string;
  role?: string;
}

interface CourseApiRecord {
  id: string;
  name?: string;
  teacherId?: string;
  teacherName?: string;
  enrolledStudentsCount?: number;
  studentIds?: string[];
}

export const AdminView: React.FC = () => {
  const { currentUser, refreshData } = useAuth();
  const [users, setUsers] = useState<User[]>([]);
  const [courses, setCourses] = useState<Course[]>([]);
  const [activeTab, setActiveTab] = useState<'users' | 'courses'>('users');

  // User Form State
  const [showUserModal, setShowUserModal] = useState(false);
  const [userName, setUserName] = useState('');
  const [userEmail, setUserEmail] = useState('');
  const [userRole, setUserRole] = useState<UserRole>('Student');

  // Course Form State
  const [showCourseModal, setShowCourseModal] = useState(false);
  const [courseCode, setCourseCode] = useState('');
  const [courseTitle, setCourseTitle] = useState('');
  const [teacherId, setTeacherId] = useState('');

  const loadData = useCallback(async () => {
    const usersRes = await apiClient.get<UserApiRecord[]>('/users');
    if (usersRes.success && usersRes.data) {
      const formattedUsers: User[] = usersRes.data.map((u) => ({
        id: u.id,
        name: u.name || `${u.firstName || ''} ${u.lastName || ''}`.trim() || u.email,
        email: u.email,
        role: (u.role as UserRole) || 'Student',
      }));
      setUsers(formattedUsers);
    } else {
      setUsers(mockStore.getUsers());
    }

    const classRes = await apiClient.get<CourseApiRecord[]>('/classes');
    if (classRes.success && classRes.data) {
      const formattedCourses: Course[] = classRes.data.map((c) => ({
        id: c.id,
        code: c.name ? c.name.split(':')[0].trim() : 'CS101',
        title: c.name && c.name.includes(':') ? c.name.split(':').slice(1).join(':').trim() : c.name || 'Course',
        teacherId: c.teacherId || 'u-2',
        teacherName: c.teacherName || 'Prof. Faculty',
        enrolledStudentsCount: c.enrolledStudentsCount || c.studentIds?.length || 20,
      }));
      setCourses(formattedCourses);
    } else {
      setCourses(mockStore.getCourses());
    }
  }, []);

  useEffect(() => {
    const timeoutId = window.setTimeout(() => {
      void loadData();
    }, 0);

    return () => window.clearTimeout(timeoutId);
  }, [loadData]);

  const handleAddUser = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!userName || !userEmail) return;

    const names = userName.split(' ');
    const firstName = names[0] || userName;
    const lastName = names.slice(1).join(' ') || 'User';

    const res = await apiClient.post('/users', {
      firstName,
      lastName,
      email: userEmail,
      password: 'User@123',
      role: userRole,
    });

    if (!res.success) {
      const currentUsers = mockStore.getUsers();
      const newUser: User = {
        id: `u-${Date.now()}`,
        name: userName,
        email: userEmail,
        role: userRole,
      };
      mockStore.saveUsers([...currentUsers, newUser]);
    }
    await refreshData();
    await loadData();
    setShowUserModal(false);
    setUserName('');
    setUserEmail('');
  };

  const handleDeleteUser = async (id: string) => {
    if (id === currentUser.id) {
      alert('You cannot delete your active admin account.');
      return;
    }
    const res = await apiClient.delete(`/users/${id}`);
    if (!res.success) {
      const updated = users.filter((u) => u.id !== id);
      mockStore.saveUsers(updated);
    }
    await refreshData();
    await loadData();
  };

  const handleAddCourse = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!courseCode || !courseTitle) return;

    const res = await apiClient.post<{ id?: string }>('/classes', {
      name: `${courseCode}: ${courseTitle}`,
      description: `Class course ${courseCode}`,
    });

    if (res.success && res.data?.id && teacherId) {
      await apiClient.post(`/classes/${res.data.id}/teachers`, { teacherId });
    } else if (!res.success) {
      const teachers = users.filter((u) => u.role === 'Teacher');
      const assignedTeacher = teachers.find((t) => t.id === teacherId) || teachers[0];
      const newCourse: Course = {
        id: `c-${Date.now()}`,
        code: courseCode,
        title: courseTitle,
        teacherId: assignedTeacher ? assignedTeacher.id : 'u-2',
        teacherName: assignedTeacher ? assignedTeacher.name : 'Prof. Alan Turing',
        enrolledStudentsCount: 20,
      };
      mockStore.saveCourses([...courses, newCourse]);
    }
    await loadData();
    setShowCourseModal(false);
    setCourseCode('');
    setCourseTitle('');
  };

  const teachersList = users.filter((u) => u.role === 'Teacher');

  return (
    <div className="space-y-8 animate-fade-in">
      {/* Admin Summary Header */}
      <NeumorphicCard variant="raised" className="p-6 sm:p-8 bg-gradient-to-r from-purple-500/10 via-indigo-500/10 to-transparent border-purple-500/20">
        <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
          <div>
            <h2 className="text-2xl font-extrabold text-gray-900 dark:text-gray-100 flex items-center gap-2">
              <Shield className="w-6 h-6 text-purple-500" />
              System Administration Panel
            </h2>
            <p className="text-sm text-gray-600 dark:text-gray-400 mt-1">
              Oversee user accounts, assign faculty roles, and configure school classes.
            </p>
          </div>
          <div className="flex items-center gap-3">
            <NeumorphicButton
              active={activeTab === 'users'}
              onClick={() => setActiveTab('users')}
            >
              <Users className="w-4 h-4" />
              Users ({users.length})
            </NeumorphicButton>
            <NeumorphicButton
              active={activeTab === 'courses'}
              onClick={() => setActiveTab('courses')}
            >
              <BookOpen className="w-4 h-4" />
              Courses ({courses.length})
            </NeumorphicButton>
          </div>
        </div>
      </NeumorphicCard>

      {/* Users Management Tab */}
      {activeTab === 'users' && (
        <div className="space-y-4">
          <div className="flex items-center justify-between">
            <h3 className="text-lg font-bold text-gray-900 dark:text-gray-100">User Accounts Directory</h3>
            <NeumorphicButton onClick={() => setShowUserModal(true)} variant="primary">
              <Plus className="w-4 h-4" />
              Add User
            </NeumorphicButton>
          </div>

          <NeumorphicCard variant="raised" className="overflow-x-auto p-2">
            <table className="w-full text-left border-collapse">
              <thead>
                <tr className="border-b border-gray-200/40 dark:border-gray-800/40 text-xs text-gray-400 uppercase">
                  <th className="p-3">User Name</th>
                  <th className="p-3">Email Address</th>
                  <th className="p-3">Role</th>
                  <th className="p-3 text-right">Actions</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-gray-200/30 dark:divide-gray-800/30 text-sm">
                {users.map((u) => (
                  <tr key={u.id} className="hover:bg-gray-500/5 transition-colors">
                    <td className="p-3 font-semibold text-gray-900 dark:text-gray-100">{u.name}</td>
                    <td className="p-3 text-gray-500">{u.email}</td>
                    <td className="p-3">
                      <NeumorphicBadge status={u.role} />
                    </td>
                    <td className="p-3 text-right">
                      <button
                        onClick={() => handleDeleteUser(u.id)}
                        className="p-2 rounded-lg neu-button text-rose-500 hover:bg-rose-500/10"
                        title="Delete User"
                      >
                        <Trash2 className="w-4 h-4" />
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </NeumorphicCard>
        </div>
      )}

      {/* Course Management Tab */}
      {activeTab === 'courses' && (
        <div className="space-y-4">
          <div className="flex items-center justify-between">
            <h3 className="text-lg font-bold text-gray-900 dark:text-gray-100">School Classes & Courses</h3>
            <NeumorphicButton onClick={() => setShowCourseModal(true)} variant="primary">
              <Plus className="w-4 h-4" />
              Add Class / Course
            </NeumorphicButton>
          </div>

          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            {courses.map((c) => (
              <NeumorphicCard key={c.id} variant="raised" className="space-y-3">
                <div className="flex items-center justify-between">
                  <span className="text-xs font-bold px-2.5 py-1 rounded-lg neu-pressed text-indigo-600 dark:text-indigo-400">
                    {c.code}
                  </span>
                  <span className="text-xs text-gray-400 font-medium">
                    {c.enrolledStudentsCount} Enrolled
                  </span>
                </div>
                <h4 className="text-base font-bold text-gray-900 dark:text-gray-100">{c.title}</h4>
                <p className="text-xs text-gray-500">Instructor: <span className="font-semibold">{c.teacherName}</span></p>
              </NeumorphicCard>
            ))}
          </div>
        </div>
      )}

      {/* Add User Modal */}
      {showUserModal && (
        <div className="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/60 backdrop-blur-sm animate-fade-in">
          <NeumorphicCard variant="raised" className="w-full max-w-md p-6 space-y-4 relative">
            <button onClick={() => setShowUserModal(false)} className="absolute top-4 right-4 p-2 rounded-full neu-button text-gray-500">
              <X className="w-4 h-4" />
            </button>
            <h3 className="text-lg font-bold text-gray-900 dark:text-gray-100">Create User Account</h3>
            <form onSubmit={handleAddUser} className="space-y-3">
              <input
                type="text"
                placeholder="Full Name"
                required
                value={userName}
                onChange={(e) => setUserName(e.target.value)}
                className="w-full p-3 rounded-xl neu-input text-sm"
              />
              <input
                type="email"
                placeholder="Email Address"
                required
                value={userEmail}
                onChange={(e) => setUserEmail(e.target.value)}
                className="w-full p-3 rounded-xl neu-input text-sm"
              />
              <select
                value={userRole}
                onChange={(e) => setUserRole(e.target.value as UserRole)}
                className="w-full p-3 rounded-xl neu-input text-sm"
              >
                <option value="Student">Student</option>
                <option value="Teacher">Teacher</option>
                <option value="Admin">Admin</option>
              </select>
              <div className="flex justify-end gap-2 pt-2">
                <NeumorphicButton type="button" onClick={() => setShowUserModal(false)}>Cancel</NeumorphicButton>
                <NeumorphicButton type="submit" variant="primary">Create User</NeumorphicButton>
              </div>
            </form>
          </NeumorphicCard>
        </div>
      )}

      {/* Add Course Modal */}
      {showCourseModal && (
        <div className="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/60 backdrop-blur-sm animate-fade-in">
          <NeumorphicCard variant="raised" className="w-full max-w-md p-6 space-y-4 relative">
            <button onClick={() => setShowCourseModal(false)} className="absolute top-4 right-4 p-2 rounded-full neu-button text-gray-500">
              <X className="w-4 h-4" />
            </button>
            <h3 className="text-lg font-bold text-gray-900 dark:text-gray-100">Create Course / Class</h3>
            <form onSubmit={handleAddCourse} className="space-y-3">
              <input
                type="text"
                placeholder="Course Code (e.g. CS101)"
                required
                value={courseCode}
                onChange={(e) => setCourseCode(e.target.value)}
                className="w-full p-3 rounded-xl neu-input text-sm"
              />
              <input
                type="text"
                placeholder="Course Title"
                required
                value={courseTitle}
                onChange={(e) => setCourseTitle(e.target.value)}
                className="w-full p-3 rounded-xl neu-input text-sm"
              />
              <select
                value={teacherId}
                onChange={(e) => setTeacherId(e.target.value)}
                className="w-full p-3 rounded-xl neu-input text-sm"
              >
                {teachersList.map((t) => (
                  <option key={t.id} value={t.id}>{t.name}</option>
                ))}
              </select>
              <div className="flex justify-end gap-2 pt-2">
                <NeumorphicButton type="button" onClick={() => setShowCourseModal(false)}>Cancel</NeumorphicButton>
                <NeumorphicButton type="submit" variant="primary">Create Course</NeumorphicButton>
              </div>
            </form>
          </NeumorphicCard>
        </div>
      )}
    </div>
  );
};
