import React from 'react';

interface NeumorphicButtonProps extends React.ButtonHTMLAttributes<HTMLButtonElement> {
  children: React.ReactNode;
  variant?: 'default' | 'primary' | 'accent' | 'danger';
  active?: boolean;
  className?: string;
}

export const NeumorphicButton: React.FC<NeumorphicButtonProps> = ({
  children,
  variant = 'default',
  active = false,
  className = '',
  ...props
}) => {
  const baseClasses = 'neu-button rounded-xl px-4 py-2.5 font-medium text-sm inline-flex items-center justify-center gap-2 transition-all';
  
  const variantStyles = {
    default: active ? 'neu-pressed text-indigo-600 dark:text-indigo-400 font-semibold' : 'text-gray-700 dark:text-gray-200',
    primary: 'bg-indigo-600 text-white shadow-lg shadow-indigo-500/20 hover:bg-indigo-700 active:translate-y-0.5',
    accent: 'bg-teal-600 text-white shadow-lg shadow-teal-500/20 hover:bg-teal-700 active:translate-y-0.5',
    danger: 'bg-rose-600 text-white shadow-lg shadow-rose-500/20 hover:bg-rose-700 active:translate-y-0.5',
  };

  return (
    <button
      className={`${baseClasses} ${variantStyles[variant]} ${active ? 'active' : ''} ${className}`}
      {...props}
    >
      {children}
    </button>
  );
};
