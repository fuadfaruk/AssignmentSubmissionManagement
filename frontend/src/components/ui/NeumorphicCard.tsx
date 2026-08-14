import React from 'react';

interface NeumorphicCardProps extends React.HTMLAttributes<HTMLDivElement> {
  children: React.ReactNode;
  variant?: 'flat' | 'raised' | 'pressed' | 'hover';
  className?: string;
}

export const NeumorphicCard: React.FC<NeumorphicCardProps> = ({
  children,
  variant = 'raised',
  className = '',
  ...props
}) => {
  const variantClasses = {
    flat: 'neu-flat',
    raised: 'neu-flat rounded-2xl p-6',
    hover: 'neu-flat-hover rounded-2xl p-6 cursor-pointer',
    pressed: 'neu-pressed rounded-2xl p-6',
  };

  return (
    <div
      className={`${variantClasses[variant]} ${className}`}
      {...props}
    >
      {children}
    </div>
  );
};
