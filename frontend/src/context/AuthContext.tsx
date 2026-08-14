'use client';

import React, { createContext, useContext, useState, useEffect, useCallback } from 'react';
import { User, UserRole, INITIAL_USERS, mockStore } from '../services/mockData';
import { apiClient } from '../services/apiClient';

interface LoginResponseDto {
  token: string;
  user: {
    id: string;
    email: string;
    firstName?: string;
    lastName?: string;
    name?: string;
    role: string;
  };
}

interface AuthUserApiResponse {
  id: string;
  email: string;
  firstName?: string;
  lastName?: string;
  name?: string;
  role: string;
}

interface AuthContextType {
  currentUser: User;
  switchRole: (role: UserRole) => Promise<void>;
  login: (email: string, password: string) => Promise<boolean>;
  usersList: User[];
  refreshData: () => Promise<void>;
  loading: boolean;
  error: string | null;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

const DEMO_CREDENTIALS: Record<UserRole, { email: string; password: string }> = {
  Admin: { email: 'admin@school.edu', password: 'Admin@123' },
  Teacher: { email: 'turing@school.edu', password: 'Teacher@123' },
  Student: { email: 'alex@student.edu', password: 'Student@123' },
};

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [usersList, setUsersList] = useState<User[]>(INITIAL_USERS);
  const [currentUser, setCurrentUser] = useState<User>(INITIAL_USERS[3]); // Default to Student
  const [loading, setLoading] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);

  const refreshData = useCallback(async () => {
    const res = await apiClient.get<AuthUserApiResponse[]>('/users');
    if (res.success && res.data) {
      const formatted = res.data.map((u) => ({
        id: u.id,
        name: u.name || `${u.firstName || ''} ${u.lastName || ''}`.trim() || u.email,
        email: u.email,
        role: u.role as UserRole,
      }));
      setUsersList(formatted);
    } else {
      setUsersList(mockStore.getUsers());
    }
  }, []);

  const loginWithCredentials = useCallback(async (email: string, password: string): Promise<boolean> => {
    setLoading(true);
    setError(null);
    const res = await apiClient.post<LoginResponseDto>('/auth/login', { email, password });
    setLoading(false);

    if (res.success && res.data && res.data.token) {
      localStorage.setItem('asm_jwt_token', res.data.token);
      const u = res.data.user;
      const loggedUser: User = {
        id: u.id,
        name: u.name || `${u.firstName || ''} ${u.lastName || ''}`.trim() || u.email,
        email: u.email,
        role: u.role as UserRole,
      };
      setCurrentUser(loggedUser);
      localStorage.setItem('asm_current_user_id', loggedUser.id);
      await refreshData();
      return true;
    } else {
      setError(res.error || 'Authentication failed');
      return false;
    }
  }, [refreshData]);

  const switchRole = useCallback(async (role: UserRole) => {
    const creds = DEMO_CREDENTIALS[role];
    if (creds) {
      const success = await loginWithCredentials(creds.email, creds.password);
      if (success) return;
    }
    // Fallback to local mock switch if API fails
    const foundUser = usersList.find((u) => u.role === role) || usersList[0];
    setCurrentUser(foundUser);
    localStorage.setItem('asm_current_user_id', foundUser.id);
  }, [loginWithCredentials, usersList]);

  useEffect(() => {
    const initAuth = async () => {
      const token = localStorage.getItem('asm_jwt_token');
      if (token) {
        const meRes = await apiClient.get<AuthUserApiResponse>('/users/me');
        if (meRes.success && meRes.data) {
          const u = meRes.data;
          const loggedUser: User = {
            id: u.id,
            name: u.name || `${u.firstName || ''} ${u.lastName || ''}`.trim() || u.email,
            email: u.email,
            role: u.role as UserRole,
          };
          setCurrentUser(loggedUser);
          await refreshData();
          return;
        }
      }
      await switchRole('Student');
    };

    void initAuth();
  }, [refreshData, switchRole]);

  return (
    <AuthContext.Provider
      value={{
        currentUser,
        switchRole,
        login: loginWithCredentials,
        usersList,
        refreshData,
        loading,
        error,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = (): AuthContextType => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};
