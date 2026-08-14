'use client';

import React, { useEffect, useState } from 'react';
import { Clock } from 'lucide-react';

interface DeadlineCountdownProps {
  dueDate: string;
}

export const DeadlineCountdown: React.FC<DeadlineCountdownProps> = ({ dueDate }) => {
  const [timeLeft, setTimeLeft] = useState<{ days: number; hours: number; mins: number; secs: number; isOverdue: boolean }>({
    days: 0,
    hours: 0,
    mins: 0,
    secs: 0,
    isOverdue: false,
  });

  useEffect(() => {
    const calculateTime = () => {
      const target = new Date(dueDate).getTime();
      const now = new Date().getTime();
      const diff = target - now;

      if (diff <= 0) {
        setTimeLeft({ days: 0, hours: 0, mins: 0, secs: 0, isOverdue: true });
        return;
      }

      const days = Math.floor(diff / (1000 * 60 * 60 * 24));
      const hours = Math.floor((diff % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
      const mins = Math.floor((diff % (1000 * 60 * 60)) / (1000 * 60));
      const secs = Math.floor((diff % (1000 * 60)) / 1000);

      setTimeLeft({ days, hours, mins, secs, isOverdue: false });
    };

    calculateTime();
    const interval = setInterval(calculateTime, 1000);
    return () => clearInterval(interval);
  }, [dueDate]);

  if (timeLeft.isOverdue) {
    return (
      <span className="inline-flex items-center gap-1.5 px-3 py-1 rounded-lg text-xs font-bold bg-rose-500/10 text-rose-600 dark:text-rose-400 neu-pressed">
        <Clock className="w-3.5 h-3.5" />
        Deadline Passed
      </span>
    );
  }

  return (
    <div className="inline-flex items-center gap-1.5 text-xs font-bold text-amber-600 dark:text-amber-400 neu-pressed px-3 py-1 rounded-lg">
      <Clock className="w-3.5 h-3.5 animate-pulse text-amber-500" />
      <span>
        {timeLeft.days > 0 && `${timeLeft.days}d `}
        {String(timeLeft.hours).padStart(2, '0')}h {String(timeLeft.mins).padStart(2, '0')}m {String(timeLeft.secs).padStart(2, '0')}s
      </span>
    </div>
  );
};
