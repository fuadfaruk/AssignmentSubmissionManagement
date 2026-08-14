const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5000/api';

interface ErrorLike {
  message?: string;
}

export interface ApiResponse<T> {
  data?: T;
  error?: string;
  success: boolean;
}

const getErrorMessage = (error: unknown): string => {
  if (error instanceof Error) {
    return error.message;
  }

  if (typeof error === 'object' && error !== null && 'message' in error) {
    const message = (error as ErrorLike).message;
    if (typeof message === 'string' && message.length > 0) {
      return message;
    }
  }

  return 'Network request failed';
};

class ApiClient {
  private getHeaders(isFormData = false): HeadersInit {
    const token = typeof window !== 'undefined' ? localStorage.getItem('asm_jwt_token') : null;
    const headers: Record<string, string> = {};
    if (!isFormData) {
      headers['Content-Type'] = 'application/json';
    }
    if (token) {
      headers['Authorization'] = `Bearer ${token}`;
    }
    return headers;
  }

  async get<T>(endpoint: string): Promise<ApiResponse<T>> {
    try {
      const response = await fetch(`${API_BASE_URL}${endpoint}`, {
        method: 'GET',
        headers: this.getHeaders(),
      });
      if (!response.ok) {
        const errText = await response.text();
        throw new Error(errText || `HTTP Error: ${response.status}`);
      }
      const data = await response.json();
      return { data, success: true };
    } catch (err: unknown) {
      return { error: getErrorMessage(err), success: false };
    }
  }

  async post<T>(endpoint: string, payload?: unknown): Promise<ApiResponse<T>> {
    try {
      const response = await fetch(`${API_BASE_URL}${endpoint}`, {
        method: 'POST',
        headers: this.getHeaders(),
        body: payload !== undefined ? JSON.stringify(payload) : undefined,
      });
      if (!response.ok) {
        const errText = await response.text();
        throw new Error(errText || `HTTP Error: ${response.status}`);
      }
      const text = await response.text();
      const data = text ? JSON.parse(text) : undefined;
      return { data, success: true };
    } catch (err: unknown) {
      return { error: getErrorMessage(err), success: false };
    }
  }

  async upload<T>(endpoint: string, formData: FormData): Promise<ApiResponse<T>> {
    try {
      const response = await fetch(`${API_BASE_URL}${endpoint}`, {
        method: 'POST',
        headers: this.getHeaders(true),
        body: formData,
      });
      if (!response.ok) {
        const errText = await response.text();
        throw new Error(errText || `HTTP Error: ${response.status}`);
      }
      const data = await response.json();
      return { data, success: true };
    } catch (err: unknown) {
      return { error: getErrorMessage(err), success: false };
    }
  }

  async put<T>(endpoint: string, payload: unknown): Promise<ApiResponse<T>> {
    try {
      const response = await fetch(`${API_BASE_URL}${endpoint}`, {
        method: 'PUT',
        headers: this.getHeaders(),
        body: JSON.stringify(payload),
      });
      if (!response.ok) {
        const errText = await response.text();
        throw new Error(errText || `HTTP Error: ${response.status}`);
      }
      const text = await response.text();
      const data = text ? JSON.parse(text) : undefined;
      return { data, success: true };
    } catch (err: unknown) {
      return { error: getErrorMessage(err), success: false };
    }
  }

  async delete<T>(endpoint: string): Promise<ApiResponse<T>> {
    try {
      const response = await fetch(`${API_BASE_URL}${endpoint}`, {
        method: 'DELETE',
        headers: this.getHeaders(),
      });
      if (!response.ok) {
        const errText = await response.text();
        throw new Error(errText || `HTTP Error: ${response.status}`);
      }
      const text = await response.text();
      const data = text ? JSON.parse(text) : undefined;
      return { data, success: true };
    } catch (err: unknown) {
      return { error: getErrorMessage(err), success: false };
    }
  }
}

export const apiClient = new ApiClient();
