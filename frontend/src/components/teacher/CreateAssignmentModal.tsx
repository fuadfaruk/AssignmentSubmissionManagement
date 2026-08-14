'use client';

import React, { useState } from 'react';
import { Course } from '../../services/mockData';
import { apiClient } from '../../services/apiClient';
import { NeumorphicCard } from '../ui/NeumorphicCard';
import { NeumorphicButton } from '../ui/NeumorphicButton';
import { X, PlusCircle, AlertCircle } from 'lucide-react';

interface CreateAssignmentModalProps {
  courses: Course[];
  onClose: () => void;
  onSuccess: () => void;
}

export const CreateAssignmentModal: React.FC<CreateAssignmentModalProps> = ({
  courses,
  onClose,
  onSuccess,
}) => {
  const [courseId, setCourseId] = useState(courses[0]?.id || '');
  const [title, setTitle] = useState('');
  const [description, setDescription] = useState('');
  const [dueDate, setDueDate] = useState('');
  const [maxMarks, setMaxMarks] = useState(100);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [errorMessage, setErrorMessage] = useState('');

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!title || !courseId || !dueDate) {
      setErrorMessage('Please fill in title, course, and due date.');
      return;
    }

    setIsSubmitting(true);
    setErrorMessage('');
    const payload = {
      title,
      description,
      dueDate: new Date(dueDate).toISOString(),
      maxMarks: Number(maxMarks),
      classId: courseId,
    };

    const res = await apiClient.post('/assignments', payload);
    setIsSubmitting(false);

    if (!res.success) {
      setErrorMessage(res.error || 'Could not publish assignment. Please check the API connection.');
      return;
    }

    onSuccess();
    onClose();
  };

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/60 backdrop-blur-sm animate-fade-in">
      <NeumorphicCard variant="raised" className="w-full max-w-lg p-6 sm:p-8 space-y-6 relative">
        <button
          onClick={onClose}
          className="absolute top-6 right-6 p-2 rounded-full neu-button text-gray-500 hover:text-rose-500"
        >
          <X className="w-5 h-5" />
        </button>

        <div className="flex items-center gap-3">
          <div className="w-10 h-10 rounded-xl neu-button flex items-center justify-center text-indigo-500">
            <PlusCircle className="w-6 h-6" />
          </div>
          <div>
            <h2 className="text-xl font-extrabold text-gray-900 dark:text-gray-100">
              Create New Assignment
            </h2>
            <p className="text-xs text-gray-500">Assign coursework to your enrolled students</p>
          </div>
        </div>

        <form onSubmit={handleSubmit} className="space-y-4">
          {errorMessage && (
            <div className="p-3 rounded-xl neu-pressed text-xs text-rose-500 flex items-center gap-2">
              <AlertCircle className="w-4 h-4 flex-shrink-0" />
              <span>{errorMessage}</span>
            </div>
          )}

          <div className="space-y-1.5">
            <label className="text-xs font-bold text-gray-700 dark:text-gray-300">Select Class / Course</label>
            <select
              value={courseId}
              onChange={(e) => setCourseId(e.target.value)}
              className="w-full p-3.5 rounded-xl neu-input text-sm"
            >
              {courses.map((c) => (
                <option key={c.id} value={c.id}>
                  {c.code} - {c.title}
                </option>
              ))}
            </select>
          </div>

          <div className="space-y-1.5">
            <label className="text-xs font-bold text-gray-700 dark:text-gray-300">Assignment Title</label>
            <input
              type="text"
              required
              placeholder="e.g. Midterm Project: Database Schema Design"
              value={title}
              onChange={(e) => setTitle(e.target.value)}
              className="w-full p-3.5 rounded-xl neu-input text-sm"
            />
          </div>

          <div className="space-y-1.5">
            <label className="text-xs font-bold text-gray-700 dark:text-gray-300">Instructions & Description</label>
            <textarea
              rows={3}
              placeholder="Provide explicit assignment goals, requirements, and submission formats..."
              value={description}
              onChange={(e) => setDescription(e.target.value)}
              className="w-full p-3.5 rounded-xl neu-input text-sm resize-none"
            />
          </div>

          <div className="grid grid-cols-2 gap-4">
            <div className="space-y-1.5">
              <label className="text-xs font-bold text-gray-700 dark:text-gray-300">Due Date & Time</label>
              <input
                type="datetime-local"
                required
                value={dueDate}
                onChange={(e) => setDueDate(e.target.value)}
                className="w-full p-3.5 rounded-xl neu-input text-xs"
              />
            </div>
            <div className="space-y-1.5">
              <label className="text-xs font-bold text-gray-700 dark:text-gray-300">Max Marks</label>
              <input
                type="number"
                min={1}
                max={500}
                required
                value={maxMarks}
                onChange={(e) => setMaxMarks(Number(e.target.value))}
                className="w-full p-3.5 rounded-xl neu-input text-sm"
              />
            </div>
          </div>

          <div className="flex items-center justify-end gap-3 pt-4">
            <NeumorphicButton type="button" onClick={onClose}>
              Cancel
            </NeumorphicButton>
            <NeumorphicButton type="submit" variant="primary" disabled={isSubmitting}>
              {isSubmitting ? 'Publishing...' : 'Publish Assignment'}
            </NeumorphicButton>
          </div>
        </form>
      </NeumorphicCard>
    </div>
  );
};
