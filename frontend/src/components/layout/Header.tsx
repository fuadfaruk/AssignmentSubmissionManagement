'use client';

import React from 'react';
import { useAuth } from '../../context/AuthContext';
import { ThemeToggle } from '../ui/ThemeToggle';
import { DemoRoleSwitcher } from '../ui/DemoRoleSwitcher';
import { NeumorphicBadge } from '../ui/NeumorphicBadge';
import { BookOpen, Search } from 'lucide-react';

export const Header: React.FC = () => {
  const { currentUser } = useAuth();

  return (
    <header className="sticky top-0 z-30 w-full neu-flat border-b border-gray-200/50 dark:border-gray-800/50 backdrop-blur-md bg-opacity-90 px-4 lg:px-8 py-3.5 flex items-center justify-between gap-4">
      {/* Brand & Search */}
      <div className="flex items-center gap-4 flex-1">
        <div className="flex items-center gap-3">
          <div className="w-10 h-10 rounded-xl neu-button flex items-center justify-center text-indigo-600 dark:text-indigo-400 font-bold">
            <BookOpen className="w-6 h-6" />
          </div>
          <div>
            <h1 className="text-base font-bold text-gray-900 dark:text-gray-100 tracking-tight leading-none">
              EduPort
            </h1>
            <p className="text-xs text-gray-500 dark:text-gray-400 mt-0.5">
              Assignment Portal
            </p>
          </div>
        </div>

        {/* Search Bar */}
        <div className="hidden md:flex items-center gap-2 px-3 py-2 rounded-xl neu-input w-64 max-w-sm ml-4">
          <Search className="w-4 h-4 text-gray-400" />
          <input
            type="text"
            placeholder="Search assignments, courses..."
            className="bg-transparent text-xs outline-none w-full text-gray-800 dark:text-gray-200 placeholder-gray-400"
          />
        </div>
      </div>

      {/* Actions & Role Switcher */}
      <div className="flex items-center gap-3">
        <DemoRoleSwitcher />
        <ThemeToggle />

        {/* User Info Badge */}
        <div className="hidden sm:flex items-center gap-3 pl-2 border-l border-gray-300/40 dark:border-gray-700/40">
          <div className="text-right">
            <p className="text-xs font-semibold text-gray-900 dark:text-gray-100">
              {currentUser.name}
            </p>
            <NeumorphicBadge status={currentUser.role} className="mt-0.5" />
          </div>
          <div className="w-9 h-9 rounded-full neu-button flex items-center justify-center font-bold text-xs text-indigo-600 dark:text-indigo-400">
            {currentUser.name.charAt(0)}
          </div>
        </div>
      </div>
    </header>
  );
};
