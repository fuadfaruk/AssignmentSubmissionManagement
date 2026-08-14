'use client';

import React from 'react';
import { useAuth } from '../../context/AuthContext';
import {
  LayoutDashboard,
  BookOpen,
  FileCheck2,
  Users,
  Award,
  Settings,
  HelpCircle,
} from 'lucide-react';

interface SidebarProps {
  activeTab: string;
  setActiveTab: (tab: string) => void;
}

export const Sidebar: React.FC<SidebarProps> = ({ activeTab, setActiveTab }) => {
  const { currentUser } = useAuth();

  const getNavItems = () => {
    switch (currentUser.role) {
      case 'Admin':
        return [
          { id: 'dashboard', label: 'Overview', icon: <LayoutDashboard className="w-5 h-5" /> },
          { id: 'users', label: 'User Management', icon: <Users className="w-5 h-5" /> },
          { id: 'classes', label: 'Classes & Courses', icon: <BookOpen className="w-5 h-5" /> },
          { id: 'settings', label: 'System Settings', icon: <Settings className="w-5 h-5" /> },
        ];
      case 'Teacher':
        return [
          { id: 'dashboard', label: 'Teacher Hub', icon: <LayoutDashboard className="w-5 h-5" /> },
          { id: 'assignments', label: 'My Assignments', icon: <BookOpen className="w-5 h-5" /> },
          { id: 'submissions', label: 'Student Submissions', icon: <FileCheck2 className="w-5 h-5" /> },
          { id: 'analytics', label: 'Grade Analytics', icon: <Award className="w-5 h-5" /> },
        ];
      case 'Student':
      default:
        return [
          { id: 'dashboard', label: 'Student Dashboard', icon: <LayoutDashboard className="w-5 h-5" /> },
          { id: 'my-assignments', label: 'Active Assignments', icon: <BookOpen className="w-5 h-5" /> },
          { id: 'my-submissions', label: 'My Submissions', icon: <FileCheck2 className="w-5 h-5" /> },
          { id: 'my-grades', label: 'Marks & Feedback', icon: <Award className="w-5 h-5" /> },
        ];
    }
  };

  const navItems = getNavItems();

  return (
    <aside className="w-64 flex-shrink-0 p-4 neu-flat border-r border-gray-200/50 dark:border-gray-800/50 min-h-[calc(100vh-65px)] hidden md:block">
      <div className="flex flex-col h-full justify-between">
        <div className="space-y-6">
          <div className="px-3 py-2 text-xs font-bold uppercase tracking-wider text-gray-400 dark:text-gray-500">
            {currentUser.role} Menu
          </div>

          <nav className="space-y-2">
            {navItems.map((item) => {
              const isActive = activeTab === item.id;
              return (
                <button
                  key={item.id}
                  onClick={() => setActiveTab(item.id)}
                  className={`w-full flex items-center gap-3 px-4 py-3 rounded-xl font-medium text-sm transition-all ${
                    isActive
                      ? 'neu-pressed text-indigo-600 dark:text-indigo-400 font-semibold'
                      : 'neu-button text-gray-700 dark:text-gray-300 hover:text-indigo-600 dark:hover:text-indigo-400'
                  }`}
                >
                  <span className={isActive ? 'text-indigo-600 dark:text-indigo-400' : 'text-gray-500 dark:text-gray-400'}>
                    {item.icon}
                  </span>
                  <span>{item.label}</span>
                </button>
              );
            })}
          </nav>
        </div>

        {/* Footer Support Card */}
        <div className="p-4 rounded-2xl neu-pressed text-xs text-gray-500 dark:text-gray-400 text-center space-y-2">
          <div className="w-8 h-8 rounded-full neu-button mx-auto flex items-center justify-center text-indigo-500">
            <HelpCircle className="w-4 h-4" />
          </div>
          <p className="font-semibold text-gray-800 dark:text-gray-200">Need Guidance?</p>
          <p>Read Project Details or use role buttons to preview functionality.</p>
        </div>
      </div>
    </aside>
  );
};
