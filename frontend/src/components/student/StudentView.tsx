'use client';

import React, { useState, useEffect, useCallback } from 'react';
import { useAuth } from '../../context/AuthContext';
import { Assignment, Submission, mockStore } from '../../services/mockData';
import { apiClient } from '../../services/apiClient';
import { NeumorphicCard } from '../ui/NeumorphicCard';
import { NeumorphicButton } from '../ui/NeumorphicButton';
import { NeumorphicBadge } from '../ui/NeumorphicBadge';
import { DeadlineCountdown } from './DeadlineCountdown';
import { SubmissionModal } from './SubmissionModal';
import { BookOpen, ArrowRight, AlertCircle, Loader2 } from 'lucide-react';

interface PagedResult<T> {
  items: T[];
}

interface ApiAssignment {
  id: string;
  classId?: string;
  className?: string;
  courseTitle?: string;
  teacherId?: string;
  title: string;
  description: string;
  dueDate: string;
  maxMarks: number;
  createdAt: string;
}

interface ApiSubmission {
  id: string;
  assignmentId: string;
  assignmentTitle?: string;
  studentId?: string;
  studentName?: string;
  submittedAt?: string;
  textContent?: string;
  fileName?: string;
  filePath?: string;
  status?: string;
  marks?: number | null;
  feedback?: string | null;
}

const toSubmissionStatus = (status?: string): Submission['status'] => {
  const normalized = status?.toLowerCase();
  if (normalized === 'graded' || normalized === 'pending' || normalized === 'overdue') {
    return normalized;
  }
  return 'submitted';
};

export const StudentView: React.FC = () => {
  const { currentUser } = useAuth();
  const [assignments, setAssignments] = useState<Assignment[]>([]);
  const [submissions, setSubmissions] = useState<Submission[]>([]);
  const [selectedAssignment, setSelectedAssignment] = useState<Assignment | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [errorMessage, setErrorMessage] = useState('');

  const loadData = useCallback(async () => {
    setIsLoading(true);
    setErrorMessage('');

    const assignRes = await apiClient.get<PagedResult<ApiAssignment> | ApiAssignment[]>('/assignments');
    if (assignRes.success && assignRes.data) {
      const items = Array.isArray(assignRes.data) ? assignRes.data : (assignRes.data.items || []);
      const formatted = items.map((a) => ({
        id: a.id,
        courseId: a.classId || 'c-1',
        courseTitle: a.className || a.courseTitle || 'Computer Science',
        teacherId: a.teacherId || 'u-2',
        title: a.title,
        description: a.description,
        dueDate: a.dueDate,
        maxMarks: a.maxMarks,
        createdAt: a.createdAt,
      }));
      setAssignments(formatted.length > 0 ? formatted : mockStore.getAssignments());
    } else {
      setErrorMessage(assignRes.error || 'Could not load assignments from the API. Showing local demo data.');
      setAssignments(mockStore.getAssignments());
    }

    const subRes = await apiClient.get<PagedResult<ApiSubmission> | ApiSubmission[]>('/submissions/my-submissions');
    if (subRes.success && subRes.data) {
      const items = Array.isArray(subRes.data) ? subRes.data : (subRes.data.items || []);
      const mappedSubmissions: Submission[] = items.map((item) => ({
        id: item.id,
        assignmentId: item.assignmentId,
        assignmentTitle: item.assignmentTitle || 'Assignment',
        studentId: item.studentId || currentUser.id,
        studentName: item.studentName || currentUser.name,
        submittedAt: item.submittedAt || new Date().toISOString(),
        textAnswer: item.textContent || '',
        fileName: item.fileName || undefined,
        filePath: item.filePath || undefined,
        fileSize: item.fileName ? 'Attachment' : undefined,
        status: toSubmissionStatus(item.status),
        marksObtained: item.marks ?? undefined,
        feedback: item.feedback ?? undefined,
      }));
      setSubmissions(mappedSubmissions);
    } else {
      setErrorMessage((prev) =>
        prev || subRes.error || 'Could not load submissions from the API. Showing local demo data.'
      );
      setSubmissions(mockStore.getSubmissions().filter((s) => s.studentId === currentUser.id));
    }

    setIsLoading(false);
  }, [currentUser.id, currentUser.name]);

  useEffect(() => {
    // eslint-disable-next-line react-hooks/set-state-in-effect
    void loadData();
  }, [loadData]);

  const getSubmissionStatus = (assignmentId: string) => {
    const sub = submissions.find((s) => s.assignmentId === assignmentId);
    if (!sub) return { label: 'pending', sub: null };
    return { label: sub.status, sub };
  };

  return (
    <div className="space-y-8 animate-fade-in">
      {/* Welcome Banner */}
      <NeumorphicCard variant="raised" className="p-6 sm:p-8 bg-gradient-to-r from-indigo-500/10 via-purple-500/10 to-transparent border-indigo-500/20">
        <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
          <div>
            <h2 className="text-2xl font-extrabold text-gray-900 dark:text-gray-100">
              Welcome back, {currentUser.name}! 🚀
            </h2>
            <p className="text-sm text-gray-600 dark:text-gray-400 mt-1">
              You have <span className="font-bold text-indigo-600 dark:text-indigo-400">{assignments.length}</span> active course assignments. Track your due dates and grades below.
            </p>
          </div>
          <div className="flex items-center gap-3">
            <div className="px-4 py-3 rounded-2xl neu-pressed text-center">
              <p className="text-xs text-gray-400 font-medium">Submissions</p>
              <p className="text-xl font-bold text-indigo-600 dark:text-indigo-400">{submissions.length}</p>
            </div>
            <div className="px-4 py-3 rounded-2xl neu-pressed text-center">
              <p className="text-xs text-gray-400 font-medium">Graded</p>
              <p className="text-xl font-bold text-emerald-600 dark:text-emerald-400">
                {submissions.filter((s) => s.status === 'graded').length}
              </p>
            </div>
          </div>
        </div>
      </NeumorphicCard>

      {/* Error Alert if any */}
      {errorMessage && (
        <div className="p-4 rounded-xl neu-pressed text-xs text-rose-500 flex items-center gap-2">
          <AlertCircle className="w-4 h-4 flex-shrink-0" />
          <span>{errorMessage}</span>
        </div>
      )}

      {/* Active Assignments Section */}
      <div className="space-y-4">
        <div className="flex items-center justify-between">
          <h3 className="text-lg font-bold text-gray-900 dark:text-gray-100 flex items-center gap-2">
            <BookOpen className="w-5 h-5 text-indigo-500" />
            Your Course Assignments
          </h3>
          <span className="text-xs text-gray-400">{assignments.length} Total</span>
        </div>

        {isLoading ? (
          <div className="flex items-center justify-center py-12 text-indigo-500 gap-2">
            <Loader2 className="w-6 h-6 animate-spin" />
            <span className="text-sm font-medium">Loading assignments...</span>
          </div>
        ) : (
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {assignments.map((assignment) => {
              const { label, sub } = getSubmissionStatus(assignment.id);
              const isSubmitted = !!sub;

              return (
                <NeumorphicCard key={assignment.id} variant="raised" className="flex flex-col justify-between space-y-4">
                  <div className="space-y-3">
                    <div className="flex items-center justify-between gap-2">
                      <span className="text-xs font-bold text-indigo-600 dark:text-indigo-400 truncate">
                        {assignment.courseTitle}
                      </span>
                      <NeumorphicBadge status={label} />
                    </div>

                    <h4 className="text-base font-bold text-gray-900 dark:text-gray-100 leading-snug">
                      {assignment.title}
                    </h4>

                    <p className="text-xs text-gray-600 dark:text-gray-400 line-clamp-2">
                      {assignment.description}
                    </p>
                  </div>

                  <div className="pt-4 border-t border-gray-200/40 dark:border-gray-800/40 space-y-3">
                    <div className="flex items-center justify-between text-xs">
                      <span className="text-gray-400 font-medium">Time Remaining:</span>
                      <DeadlineCountdown dueDate={assignment.dueDate} />
                    </div>

                    {sub && sub.status === 'graded' ? (
                      <div className="p-3 rounded-xl neu-pressed bg-emerald-500/5 space-y-1">
                        <div className="flex items-center justify-between text-xs font-bold text-emerald-600 dark:text-emerald-400">
                          <span>Grade Awarded:</span>
                          <span>{sub.marksObtained} / {assignment.maxMarks}</span>
                        </div>
                        {sub.feedback && (
                          <p className="text-xs text-gray-600 dark:text-gray-300 italic">
                            &quot;{sub.feedback}&quot;
                          </p>
                        )}
                      </div>
                    ) : (
                      <NeumorphicButton
                        onClick={() => setSelectedAssignment(assignment)}
                        variant={isSubmitted ? 'default' : 'primary'}
                        className="w-full justify-between"
                      >
                        <span>{isSubmitted ? 'Update Submission' : 'Submit Work'}</span>
                        <ArrowRight className="w-4 h-4" />
                      </NeumorphicButton>
                    )}
                  </div>
                </NeumorphicCard>
              );
            })}
          </div>
        )}
      </div>

      {/* Submission Modal */}
      {selectedAssignment && (
        <SubmissionModal
          assignment={selectedAssignment}
          onClose={() => setSelectedAssignment(null)}
          onSuccess={loadData}
        />
      )}
    </div>
  );
};
