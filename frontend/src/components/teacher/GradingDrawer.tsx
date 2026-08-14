'use client';

import React, { useState } from 'react';
import { Submission, Assignment } from '../../services/mockData';
import { apiClient } from '../../services/apiClient';
import { NeumorphicCard } from '../ui/NeumorphicCard';
import { NeumorphicButton } from '../ui/NeumorphicButton';
import { NeumorphicBadge } from '../ui/NeumorphicBadge';
import { X, FileText, Download, Sparkles, AlertCircle } from 'lucide-react';

interface GradingDrawerProps {
  submission: Submission;
  assignment: Assignment;
  onClose: () => void;
  onSuccess: () => void;
}

export const GradingDrawer: React.FC<GradingDrawerProps> = ({
  submission,
  assignment,
  onClose,
  onSuccess,
}) => {
  const [marks, setMarks] = useState<number>(submission.marksObtained ?? Math.round(assignment.maxMarks * 0.85));
  const [feedback, setFeedback] = useState<string>(submission.feedback ?? '');
  const [isSaving, setIsSaving] = useState(false);
  const [errorMessage, setErrorMessage] = useState('');

  const quickTemplates = [
    'Great effort! Excellent structure and attention to requirements.',
    'Good work overall. Review edge cases and optimize code formatting.',
    'Incomplete submission. Please address missing requirements in resubmission.',
  ];

  const handleSaveGrade = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsSaving(true);
    setErrorMessage('');
    const res = await apiClient.put(`/submissions/${submission.id}/grade`, {
      marks: Number(marks),
      feedback,
    });
    setIsSaving(false);

    if (!res.success) {
      setErrorMessage(res.error || 'Could not save grade. Please check the API connection.');
      return;
    }
    onSuccess();
    onClose();
  };

  const handleDownloadFile = () => {
    if (submission.filePath) {
      const backendBase = process.env.NEXT_PUBLIC_API_URL?.replace('/api', '') || 'http://localhost:5000';
      window.open(`${backendBase}/${submission.filePath}`, '_blank');
    } else {
      alert(`Simulating download of ${submission.fileName || 'submission_file.pdf'}`);
    }
  };

  return (
    <div className="fixed inset-0 z-50 flex justify-end bg-black/60 backdrop-blur-sm animate-fade-in">
      <div className="w-full max-w-xl h-full bg-[var(--bg-color)] shadow-2xl p-6 sm:p-8 overflow-y-auto space-y-6 flex flex-col justify-between border-l border-gray-200/50 dark:border-gray-800/50">
        <div className="space-y-6">
          {/* Header */}
          <div className="flex items-center justify-between border-b border-gray-200/40 dark:border-gray-800/40 pb-4">
            <div>
              <NeumorphicBadge status={submission.status} />
              <h2 className="text-xl font-extrabold text-gray-900 dark:text-gray-100 mt-2">
                Grading: {submission.studentName}
              </h2>
              <p className="text-xs text-gray-500">{submission.assignmentTitle}</p>
            </div>
            <button
              onClick={onClose}
              className="p-2 rounded-full neu-button text-gray-500 hover:text-rose-500"
            >
              <X className="w-5 h-5" />
            </button>
          </div>

          {/* Submission Preview Card */}
          <NeumorphicCard variant="pressed" className="p-5 space-y-4">
            <h3 className="text-xs font-bold uppercase tracking-wider text-gray-400">
              Student Work Preview
            </h3>

            {submission.fileName && (
              <div className="flex items-center justify-between p-3 rounded-xl neu-flat bg-indigo-500/5">
                <div className="flex items-center gap-2 text-sm font-semibold text-indigo-600 dark:text-indigo-400">
                  <FileText className="w-4 h-4" />
                  <span>{submission.fileName}</span>
                  <span className="text-xs text-gray-400">({submission.fileSize})</span>
                </div>
                <NeumorphicButton
                  onClick={handleDownloadFile}
                  className="p-2 text-xs"
                >
                  <Download className="w-4 h-4 text-indigo-500" />
                </NeumorphicButton>
              </div>
            )}

            {submission.textAnswer && (
              <div className="space-y-1">
                <p className="text-xs font-semibold text-gray-500">Submission Notes:</p>
                <p className="text-xs text-gray-800 dark:text-gray-200 bg-gray-500/5 p-3 rounded-xl font-mono">
                  {submission.textAnswer}
                </p>
              </div>
            )}

            <p className="text-xs text-gray-400">
              Submitted on: {new Date(submission.submittedAt).toLocaleString()}
            </p>
          </NeumorphicCard>

          {/* Grading Input Form */}
          <form onSubmit={handleSaveGrade} className="space-y-6">
            {errorMessage && (
              <div className="p-3 rounded-xl neu-pressed text-xs text-rose-500 flex items-center gap-2">
                <AlertCircle className="w-4 h-4 flex-shrink-0" />
                <span>{errorMessage}</span>
              </div>
            )}

            {/* Score Slider & Numeric Input */}
            <div className="space-y-3">
              <div className="flex items-center justify-between">
                <label className="text-xs font-bold text-gray-700 dark:text-gray-300">
                  Assign Score (Max: {assignment.maxMarks})
                </label>
                <span className="text-lg font-extrabold text-indigo-600 dark:text-indigo-400">
                  {marks} / {assignment.maxMarks}
                </span>
              </div>
              <input
                type="range"
                min={0}
                max={assignment.maxMarks}
                value={marks}
                onChange={(e) => setMarks(Number(e.target.value))}
                className="w-full accent-indigo-600 cursor-pointer h-2 bg-gray-200 dark:bg-gray-700 rounded-lg"
              />
            </div>

            {/* Quick Feedback Templates */}
            <div className="space-y-2">
              <span className="text-xs font-bold text-gray-500 flex items-center gap-1">
                <Sparkles className="w-3.5 h-3.5 text-amber-500" /> Quick Feedback Snippets:
              </span>
              <div className="flex flex-wrap gap-2">
                {quickTemplates.map((template, idx) => (
                  <button
                    key={idx}
                    type="button"
                    onClick={() => setFeedback(template)}
                    className="text-xs p-2 rounded-xl neu-button text-left text-gray-600 dark:text-gray-300 hover:text-indigo-500"
                  >
                    {template}
                  </button>
                ))}
              </div>
            </div>

            {/* Detailed Feedback Textarea */}
            <div className="space-y-1.5">
              <label className="text-xs font-bold text-gray-700 dark:text-gray-300">
                Detailed Feedback & Comments
              </label>
              <textarea
                rows={4}
                value={feedback}
                onChange={(e) => setFeedback(e.target.value)}
                placeholder="Type constructive advice or recommendations for the student..."
                className="w-full p-4 rounded-xl neu-input text-sm resize-none"
              />
            </div>

            <div className="flex items-center justify-end gap-3 pt-4 border-t border-gray-200/40 dark:border-gray-800/40">
              <NeumorphicButton type="button" onClick={onClose}>
                Cancel
              </NeumorphicButton>
              <NeumorphicButton type="submit" variant="primary" disabled={isSaving}>
                {isSaving ? 'Saving...' : 'Save & Publish Grade'}
              </NeumorphicButton>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
};
