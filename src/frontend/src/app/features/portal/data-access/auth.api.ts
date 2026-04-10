import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable, map } from 'rxjs';

import { API_BASE_URL } from '../../../core/api/api.tokens';
import { StudentSessionState } from '../../../core/session/student-session.service';

export interface StudentLoginRequest {
  email: string;
  password: string;
}

interface StudentSessionResponse {
  id: string;
  fullName: string;
  email: string;
  programName: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthApiService {
  private readonly http = inject(HttpClient);
  private readonly apiBaseUrl = inject(API_BASE_URL);

  login(request: StudentLoginRequest): Observable<StudentSessionState> {
    return this.http.post<StudentSessionResponse>(`${this.apiBaseUrl}/auth/login`, request).pipe(
      map((response) => ({
        studentId: response.id,
        fullName: response.fullName,
        email: response.email,
        programName: response.programName
      }))
    );
  }
}
