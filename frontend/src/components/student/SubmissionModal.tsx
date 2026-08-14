'use client';

import React, { useState } from 'react';
import { Assignment } from '../../services/mockData';
import { apiClient } from '../../services/apiClient';
import { NeumorphicCard } from '../ui/NeumorphicCard';
import { NeumorphicButton } from '../ui/NeumorphicButton';
import { UploadCloud, X, FileText, CheckCircle2, AlertCircle } from 'lucide-react';

interface SubmissionModalProps {
  assignment: Assignment;
  onClose: () => void;
  onSuccess: () => void;
}

export const SubmissionModal: React.FC<SubmissionModalProps> = ({
  assignment,
  onClose,
  onSuccess,
}) => {
  const [textAnswer, setTextAnswer] = useState('');
  const [selectedFile, setSelectedFile] = useState<File | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [submittedSuccess, setSubmittedSuccess] = useState(false);
  const [errorMessage, setErrorMessage] = useState('');

  const handleFileDrop = (e: React.DragEvent<HTMLDivElement>) => {
    e.preventDefault();
    if (e.dataTransfer.files && e.dataTransfer.files[0]) {
      setSelectedFile(e.dataTransfer.files[0]);
    }
  };

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (e.target.files && e.target.files[0]) {
      setSelectedFile(e.target.files[0]);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!textAnswer.trim() && !selectedFile) {
      setErrorMessage('Please provide either text notes or upload a file.');
      return;
    }

    setIsSubmitting(true);
    setErrorMessage('');
    const formData = new FormData();
    formData.append('AssignmentId', assignment.id);
    if (textAnswer) {
      formData.append('TextContent', textAnswer);
    }
    if (selectedFile) {
      formData.append('file', selectedFile);
    }

    const res = await apiClient.upload('/submissions', formData);
    setIsSubmitting(false);

    if (res.success) {
      setSubmittedSuccess(true);
      setTimeout(() => {
        onSuccess();
        onClose();
      }, 1200);
    } else {
      setErrorMessage(res.error || 'Submission failed. Please try again after checking the API connection.');
    }
  };

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/60 backdrop-blur-sm animate-fade-in">
      <NeumorphicCard variant="raised" className="w-full max-w-xl max-h-[90vh] overflow-y-auto p-6 sm:p-8 space-y-6 relative">
        <button
          onClick={onClose}
          className="absolute top-6 right-6 p-2 rounded-full neu-button text-gray-500 hover:text-rose-500"
        >
          <X className="w-5 h-5" />
        </button>

        <div>
          <span className="text-xs font-bold text-indigo-600 dark:text-indigo-400 uppercase tracking-wider">
            {assignment.courseTitle}
          </span>
          <h2 className="text-xl font-extrabold text-gray-900 dark:text-gray-100 mt-1">
            {assignment.title}
          </h2>
          <p className="text-xs text-gray-500 dark:text-gray-400 mt-1">
            Max Marks: <span className="font-semibold">{assignment.maxMarks}</span> | Due:{' '}
            <span className="font-semibold text-amber-600 dark:text-amber-400">
              {new Date(assignment.dueDate).toLocaleString()}
            </span>
          </p>
        </div>

        {submittedSuccess ? (
          <div className="py-8 text-center space-y-3 neu-pressed rounded-2xl p-6">
            <CheckCircle2 className="w-12 h-12 text-emerald-500 mx-auto animate-bounce" />
            <h3 className="text-lg font-bold text-gray-900 dark:text-gray-100">
              Assignment Submitted Successfully!
            </h3>
            <p className="text-xs text-gray-500">Your work has been safely received and stored.</p>
          </div>
        ) : (
          <form onSubmit={handleSubmit} className="space-y-5">
            {errorMessage && (
              <div className="p-3 rounded-xl neu-pressed text-xs text-rose-500 flex items-center gap-2">
                <AlertCircle className="w-4 h-4 flex-shrink-0" />
                <span>{errorMessage}</span>
              </div>
            )}

            {/* Drag & Drop File Uploader */}
            <div className="space-y-2">
              <label className="text-xs font-bold text-gray-700 dark:text-gray-300">
                Upload Work File (PDF, DOCX, ZIP, Code)
              </label>
              <div
                onDragOver={(e) => e.preventDefault()}
                onDrop={handleFileDrop}
                className="neu-pressed rounded-2xl p-6 text-center border-2 border-dashed border-gray-300 dark:border-gray-700 hover:border-indigo-500 transition-colors cursor-pointer"
              >
                <input
                  type="file"
                  id="file-upload"
                  className="hidden"
                  onChange={handleFileChange}
                />
                <label htmlFor="file-upload" className="cursor-pointer block space-y-2">
                  <UploadCloud className="w-10 h-10 text-indigo-500 mx-auto" />
                  {selectedFile ? (
                    <div className="flex items-center justify-center gap-2 text-sm font-semibold text-emerald-600 dark:text-emerald-400">
                      <FileText className="w-4 h-4" />
                      <span>{selectedFile.name}</span>
                      <span className="text-xs text-gray-400">({(selectedFile.size / 1024).toFixed(1)} KB)</span>
                    </div>
                  ) : (
                    <div>
                      <p className="text-sm font-semibold text-gray-800 dark:text-gray-200">
                        Drag and drop your file here, or <span className="text-indigo-600 underline">browse</span>
                      </p>
                      <p className="text-xs text-gray-400 mt-1">Supports files up to 25MB</p>
                    </div>
                  )}
                </label>
              </div>
            </div>

            {/* Text Answer Input */}
            <div className="space-y-2">
              <label className="text-xs font-bold text-gray-700 dark:text-gray-300">
                Submission Notes & Summary
              </label>
              <textarea
                rows={4}
                value={textAnswer}
                onChange={(e) => setTextAnswer(e.target.value)}
                placeholder="Type your notes, solution details, or references here..."
                className="w-full p-4 rounded-xl neu-input text-sm resize-none"
              />
            </div>

            {/* Modal Actions */}
            <div className="flex items-center justify-end gap-3 pt-2">
              <NeumorphicButton type="button" onClick={onClose}>
                Cancel
              </NeumorphicButton>
              <NeumorphicButton
                type="submit"
                variant="primary"
                disabled={isSubmitting}
                className="min-w-[120px]"
              >
                {isSubmitting ? 'Submitting...' : 'Submit Work'}
              </NeumorphicButton>
            </div>
          </form>
        )}
      </NeumorphicCard>
    </div>
  );
};
