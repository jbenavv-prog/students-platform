import { Injectable, signal } from '@angular/core';

export type UserRole = 'administrador' | 'estudiante';

export interface StudentSessionState {
  userId: string;
  fullName: string;
  email: string;
  programName: string | null;
  role: UserRole;
  accessToken: string;
}

@Injectable({
  providedIn: 'root'
})
export class StudentSessionService {
  private readonly storageKey = 'students-platform.session';
  private readonly sessionSignal = signal<StudentSessionState | null>(this.readFromStorage());

  readonly session = this.sessionSignal.asReadonly();

  setSession(session: StudentSessionState): void {
    this.sessionSignal.set(session);
    this.persist(session);
  }

  clearSession(): void {
    this.sessionSignal.set(null);
    this.persist(null);
  }

  defaultRoute(): string {
    const session = this.sessionSignal();
    if (!session) {
      return '/login';
    }

    return session.role === 'administrador' ? '/students' : '/portal';
  }

  canAccessStudent(studentId: string | null): boolean {
    const session = this.sessionSignal();
    if (!session || !studentId) {
      return false;
    }

    return session.role === 'administrador' || session.userId === studentId;
  }

  private readFromStorage(): StudentSessionState | null {
    if (typeof window === 'undefined') {
      return null;
    }

    try {
      const raw = window.localStorage.getItem(this.storageKey);
      if (!raw) {
        return null;
      }

      return JSON.parse(raw) as StudentSessionState;
    } catch {
      return null;
    }
  }

  private persist(session: StudentSessionState | null): void {
    if (typeof window === 'undefined') {
      return;
    }

    if (!session) {
      window.localStorage.removeItem(this.storageKey);
      return;
    }

    window.localStorage.setItem(this.storageKey, JSON.stringify(session));
  }
}
