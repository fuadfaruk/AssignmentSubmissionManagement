'use client';

import React, { useState } from 'react';
import { useAuth } from '../context/AuthContext';
import { Header } from '../components/layout/Header';
import { Sidebar } from '../components/layout/Sidebar';
import { StudentView } from '../components/student/StudentView';
import { TeacherView } from '../components/teacher/TeacherView';
import { AdminView } from '../components/admin/AdminView';

export default function Home() {
  const { currentUser } = useAuth();
  const [activeTab, setActiveTab] = useState('dashboard');

  const renderRoleView = () => {
    switch (currentUser.role) {
      case 'Admin':
        return <AdminView />;
      case 'Teacher':
        return <TeacherView />;
      case 'Student':
      default:
        return <StudentView />;
    }
  };

  return (
    <div className="min-h-screen flex flex-col bg-[var(--bg-color)] text-[var(--text-main)] transition-colors duration-300">
      <Header />
      <div className="flex flex-1">
        <Sidebar activeTab={activeTab} setActiveTab={setActiveTab} />
        <main className="flex-1 p-4 lg:p-8 max-w-7xl mx-auto w-full">
          {renderRoleView()}
        </main>
      </div>
    </div>
  );
}
