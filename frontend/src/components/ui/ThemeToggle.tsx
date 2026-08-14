'use client';

import React, { useEffect, useState } from 'react';
import { Sun, Moon } from 'lucide-react';
import { NeumorphicButton } from './NeumorphicButton';

const getStoredTheme = (): 'light' | 'dark' => {
  if (typeof window === 'undefined') {
    return 'light';
  }

  return (window.localStorage.getItem('theme') as 'light' | 'dark') || 'light';
};

const applyTheme = (theme: 'light' | 'dark') => {
  if (typeof document === 'undefined') {
    return;
  }

  document.documentElement.classList.toggle('dark', theme === 'dark');
  document.documentElement.setAttribute('data-theme', theme);
};

export const ThemeToggle: React.FC = () => {
  const [theme, setTheme] = useState<'light' | 'dark'>(getStoredTheme);

  useEffect(() => {
    applyTheme(theme);
  }, [theme]);

  const toggleTheme = () => {
    const nextTheme = theme === 'light' ? 'dark' : 'light';
    setTheme(nextTheme);
    localStorage.setItem('theme', nextTheme);
    applyTheme(nextTheme);
  };

  return (
    <NeumorphicButton
      onClick={toggleTheme}
      aria-label="Toggle theme"
      title={`Switch to ${theme === 'light' ? 'Dark Mode (Matte Black)' : 'Light Mode (Off White)'}`}
      className="p-2.5 rounded-xl"
    >
      {theme === 'light' ? (
        <Moon className="w-5 h-5 text-gray-700 hover:text-indigo-600 transition-colors" />
      ) : (
        <Sun className="w-5 h-5 text-amber-400 hover:text-amber-300 transition-colors" />
      )}
    </NeumorphicButton>
  );
};
