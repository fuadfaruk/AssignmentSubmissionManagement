'use client';

import React, { useState, useEffect, useCallback } from 'react';
import { useAuth } from '../../context/AuthContext';
import { Assignment, Submission, Course, mockStore } from '../../services/mockData';
import { apiClient } from '../../services/apiClient';
import { NeumorphicCard } from '../ui/NeumorphicCard';
import { NeumorphicButton } from '../ui/NeumorphicButton';
import { NeumorphicBadge } from '../ui/NeumorphicBadge';
import { CreateAssignmentModal } from './CreateAssignmentModal';
import { GradingDrawer } from './GradingDrawer';
import { Plus, BookOpen, AlertCircle, Loader2 } from 'lucide-react';

interface PagedResult<T> {
  items: T[];
}

interface ApiAssignment {
  id: string;
  classId?: string;
  className?: string;
  teacherId?: string;
  title: string;
  description: string;
  dueDate: string;
  maxMarks: number;
  createdAt: string;
}

interface ApiClass {
  id: string;
  name?: string;
  teachers?: Array<{ id: string; name?: string; firstName?: string; lastName?: string }>;
  students?: unknown[];
}

interface ApiSubmission {
  id: string;
  assignmentId?: string;
  assignmentTitle?: string;
  studentId: string;
  studentName?: string;
  submittedAt?: string;
  status?: string;
  fileName?: string;
  filePath?: string;
  textContent?: string;
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

export const TeacherView: React.FC = () => {
  const { currentUser } = useAuth();
  const [assignments, setAssignments] = useState<Assignment[]>([]);
  const [submissions, setSubmissions] = useState<Submission[]>([]);
  const [courses, setCourses] = useState<Course[]>([]);
  const [isCreateOpen, setIsCreateOpen] = useState(false);
  const [selectedSubmission, setSelectedSubmission] = useState<Submission | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [errorMessage, setErrorMessage] = useState('');

  const loadData = useCallback(async () => {
    setIsLoading(true);
    setErrorMessage('');
    let myAssignments: Assignment[] = [];
    const assignRes = await apiClient.get<PagedResult<ApiAssignment> | ApiAssignment[]>('/assignments');
    if (assignRes.success && assignRes.data) {
      const items = Array.isArray(assignRes.data) ? assignRes.data : (assignRes.data.items || []);
      myAssignments = items.map((a) => ({
        id: a.id,
        courseId: a.classId || 'c-1',
        courseTitle: a.className || 'Course Assignment',
        teacherId: a.teacherId || currentUser.id,
        title: a.title,
        description: a.description,
        dueDate: a.dueDate,
        maxMarks: a.maxMarks,
        createdAt: a.createdAt,
      }));
    } else {
      setErrorMessage(assignRes.error || 'Could not load assignments from the API. Showing local demo data.');
      const allAssignments = mockStore.getAssignments();
      myAssignments = allAssignments.filter((a) => a.teacherId === currentUser.id);
    }
    setAssignments(myAssignments);

    const classRes = await apiClient.get<ApiClass[]>('/classes');
    if (classRes.success && classRes.data) {
      const formattedCourses: Course[] = classRes.data.map((c) => {
        const firstTeacher = c.teachers?.[0];
        const teacherName = firstTeacher?.name
          || [firstTeacher?.firstName, firstTeacher?.lastName].filter(Boolean).join(' ')
          || currentUser.name;

        return {
        id: c.id,
        code: c.name ? c.name.split(':')[0].trim() : 'CS101',
        title: c.name && c.name.includes(':') ? c.name.split(':').slice(1).join(':').trim() : c.name || 'Course',
        teacherId: firstTeacher?.id || currentUser.id,
        teacherName,
        enrolledStudentsCount: c.students?.length || 0,
        };
      });
      setCourses(formattedCourses);
    } else {
      const myCourses = mockStore.getCourses().filter((c) => c.teacherId === currentUser.id);
      setCourses(myCourses.length > 0 ? myCourses : mockStore.getCourses());
    }

    const allSubs: Submission[] = [];
    for (const a of myAssignments) {
      const subsRes = await apiClient.get<PagedResult<ApiSubmission>>(`/submissions/assignment/${a.id}`);
      if (subsRes.success && subsRes.data?.items) {
        subsRes.data.items.forEach((s) => {
          allSubs.push({
            id: s.id,
            assignmentId: s.assignmentId || a.id,
            assignmentTitle: s.assignmentTitle || a.title,
            studentId: s.studentId,
            studentName: s.studentName || 'Student',
            submittedAt: s.submittedAt || new Date().toISOString(),
            status: toSubmissionStatus(s.status),
            fileName: s.fileName || undefined,
            filePath: s.filePath || undefined,
            fileSize: s.fileName ? 'Attachment' : undefined,
            textAnswer: s.textContent || '',
            marksObtained: s.marks ?? undefined,
            feedback: s.feedback ?? undefined,
          });
        });
      } else if (!subsRes.success) {
        setErrorMessage((prev) =>
          prev || subsRes.error || 'Could not load submissions from the API. Showing local demo data.'
        );
      }
    }
    if (allSubs.length > 0) {
      setSubmissions(allSubs);
    } else {
      setSubmissions(mockStore.getSubmissions());
    }
    setIsLoading(false);
  }, [currentUser.id, currentUser.name]);

  useEffect(() => {
    // eslint-disable-next-line react-hooks/set-state-in-effect
    void loadData();
  }, [loadData]);

  const getAssignmentSubmissions = (assignmentId: string) => {
    return submissions.filter((s) => s.assignmentId === assignmentId);
  };

  return (
    <div className="space-y-8 animate-fade-in">
      {/* Teacher Action Header */}
      <NeumorphicCard variant="raised" className="p-6 sm:p-8 bg-gradient-to-r from-teal-500/10 via-indigo-500/10 to-transparent border-teal-500/20">
        <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
          <div>
            <h2 className="text-2xl font-extrabold text-gray-900 dark:text-gray-100">
              Teacher Dashboard 🎓
            </h2>
            <p className="text-sm text-gray-600 dark:text-gray-400 mt-1">
              Welcome {currentUser.name}. Manage course assignments and grade student submissions.
            </p>
          </div>
          <NeumorphicButton
            onClick={() => setIsCreateOpen(true)}
            variant="primary"
            className="self-start md:self-auto"
          >
            <Plus className="w-5 h-5" />
            Create Assignment
          </NeumorphicButton>
        </div>
      </NeumorphicCard>

      {errorMessage && (
        <div className="p-4 rounded-xl neu-pressed text-xs text-rose-500 flex items-center gap-2">
          <AlertCircle className="w-4 h-4 flex-shrink-0" />
          <span>{errorMessage}</span>
        </div>
      )}

      {/* Course Assignments Grid */}
      <div className="space-y-4">
        <h3 className="text-lg font-bold text-gray-900 dark:text-gray-100 flex items-center gap-2">
          <BookOpen className="w-5 h-5 text-indigo-500" />
          Active Assignments ({assignments.length})
        </h3>

        {isLoading ? (
          <div className="flex items-center justify-center py-12 text-indigo-500 gap-2">
            <Loader2 className="w-6 h-6 animate-spin" />
            <span className="text-sm font-medium">Loading assignments...</span>
          </div>
        ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
          {assignments.map((assignment) => {
            const assignmentSubs = getAssignmentSubmissions(assignment.id);
            const gradedCount = assignmentSubs.filter((s) => s.status === 'graded').length;

            return (
              <NeumorphicCard key={assignment.id} variant="raised" className="space-y-4">
                <div className="flex items-center justify-between">
                  <span className="text-xs font-bold text-teal-600 dark:text-teal-400">
                    {assignment.courseTitle}
                  </span>
                  <span className="text-xs font-semibold text-gray-400">
                    Due: {new Date(assignment.dueDate).toLocaleDateString()}
                  </span>
                </div>

                <div>
                  <h4 className="text-base font-bold text-gray-900 dark:text-gray-100">
                    {assignment.title}
                  </h4>
                  <p className="text-xs text-gray-500 mt-1">{assignment.description}</p>
                </div>

                <div className="p-3 rounded-xl neu-pressed flex items-center justify-between text-xs">
                  <div className="flex items-center gap-4">
                    <div>
                      <p className="text-gray-400">Submissions</p>
                      <p className="font-bold text-indigo-600 dark:text-indigo-400">{assignmentSubs.length}</p>
                    </div>
                    <div>
                      <p className="text-gray-400">Graded</p>
                      <p className="font-bold text-emerald-600 dark:text-emerald-400">{gradedCount}</p>
                    </div>
                  </div>
                  <span className="text-xs font-bold text-gray-500">Max: {assignment.maxMarks} Marks</span>
                </div>

                {/* Submissions List for this assignment */}
                <div className="space-y-2 pt-2 border-t border-gray-200/40 dark:border-gray-800/40">
                  <p className="text-xs font-bold uppercase tracking-wider text-gray-400">
                    Student Submissions ({assignmentSubs.length})
                  </p>
                  {assignmentSubs.length === 0 ? (
                    <p className="text-xs text-gray-400 italic">No submissions received yet for this assignment.</p>
                  ) : (
                    <div className="space-y-2">
                      {assignmentSubs.map((sub) => (
                        <div
                          key={sub.id}
                          className="flex items-center justify-between p-3 rounded-xl neu-button text-xs"
                        >
                          <div className="space-y-0.5">
                            <p className="font-bold text-gray-900 dark:text-gray-100">{sub.studentName}</p>
                            <p className="text-[10px] text-gray-400">
                              {new Date(sub.submittedAt).toLocaleString()}
                            </p>
                          </div>
                          <div className="flex items-center gap-3">
                            <NeumorphicBadge status={sub.status} />
                            <NeumorphicButton
                              onClick={() => setSelectedSubmission(sub)}
                              variant={sub.status === 'graded' ? 'default' : 'accent'}
                              className="px-3 py-1 text-xs"
                            >
                              {sub.status === 'graded' ? 'Review Grade' : 'Grade Work'}
                            </NeumorphicButton>
                          </div>
                        </div>
                      ))}
                    </div>
                  )}
                </div>
              </NeumorphicCard>
            );
          })}
        </div>
        )}
      </div>

      {/* Modals */}
      {isCreateOpen && (
        <CreateAssignmentModal
          courses={courses}
          onClose={() => setIsCreateOpen(false)}
          onSuccess={loadData}
        />
      )}

      {selectedSubmission && (
        <GradingDrawer
          submission={selectedSubmission}
          assignment={assignments.find((a) => a.id === selectedSubmission.assignmentId) || assignments[0]}
          onClose={() => setSelectedSubmission(null)}
          onSuccess={loadData}
        />
      )}
    </div>
  );
};
