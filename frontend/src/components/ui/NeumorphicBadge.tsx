import React from 'react';

type BadgeStatus = 'submitted' | 'graded' | 'pending' | 'overdue' | 'admin' | 'teacher' | 'student';

interface NeumorphicBadgeProps {
  status: BadgeStatus | string;
  label?: string;
  className?: string;
}

export const NeumorphicBadge: React.FC<NeumorphicBadgeProps> = ({
  status,
  label,
  className = '',
}) => {
  const displayLabel = label || status.charAt(0).toUpperCase() + status.slice(1);

  const statusStyles: Record<string, string> = {
    submitted: 'bg-emerald-500/10 text-emerald-600 dark:text-emerald-400 border-emerald-500/20',
    graded: 'bg-indigo-500/10 text-indigo-600 dark:text-indigo-400 border-indigo-500/20',
    pending: 'bg-amber-500/10 text-amber-600 dark:text-amber-400 border-amber-500/20',
    overdue: 'bg-rose-500/10 text-rose-600 dark:text-rose-400 border-rose-500/20',
    admin: 'bg-purple-500/10 text-purple-600 dark:text-purple-400 border-purple-500/20',
    teacher: 'bg-blue-500/10 text-blue-600 dark:text-blue-400 border-blue-500/20',
    student: 'bg-teal-500/10 text-teal-600 dark:text-teal-400 border-teal-500/20',
  };

  const selectedStyle = statusStyles[status.toLowerCase()] || 'bg-gray-500/10 text-gray-600 dark:text-gray-400 border-gray-500/20';

  return (
    <span
      className={`inline-flex items-center px-3 py-1 rounded-full text-xs font-semibold border neu-pressed ${selectedStyle} ${className}`}
    >
      <span className="w-1.5 h-1.5 rounded-full bg-current mr-1.5 animate-pulse" />
      {displayLabel}
    </span>
  );
};
