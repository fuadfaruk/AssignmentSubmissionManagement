'use client';

import React from 'react';
import { useAuth } from '../../context/AuthContext';
import { UserRole } from '../../services/mockData';
import { Shield, GraduationCap, UserCheck } from 'lucide-react';

export const DemoRoleSwitcher: React.FC = () => {
  const { currentUser, switchRole } = useAuth();

  const roles: { role: UserRole; label: string; icon: React.ReactNode }[] = [
    { role: 'Admin', label: 'Admin', icon: <Shield className="w-4 h-4 text-purple-500" /> },
    { role: 'Teacher', label: 'Teacher', icon: <GraduationCap className="w-4 h-4 text-blue-500" /> },
    { role: 'Student', label: 'Student', icon: <UserCheck className="w-4 h-4 text-teal-500" /> },
  ];

  return (
    <div className="flex items-center gap-2 p-1.5 rounded-2xl neu-pressed">
      <span className="text-xs font-semibold px-2 text-gray-500 dark:text-gray-400 hidden sm:inline">
        Demo Role:
      </span>
      {roles.map(({ role, label, icon }) => {
        const isActive = currentUser.role === role;
        return (
          <button
            key={role}
            onClick={() => switchRole(role)}
            className={`flex items-center gap-1.5 px-3 py-1.5 rounded-xl text-xs font-semibold transition-all ${
              isActive
                ? 'neu-button text-indigo-600 dark:text-indigo-400 border-indigo-500/30'
                : 'text-gray-600 dark:text-gray-400 hover:text-gray-900 dark:hover:text-gray-100'
            }`}
            title={`Switch to ${label} view`}
          >
            {icon}
            <span>{label}</span>
          </button>
        );
      })}
    </div>
  );
};
