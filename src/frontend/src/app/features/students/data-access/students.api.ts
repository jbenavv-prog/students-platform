import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { API_BASE_URL } from '../../../core/api/api.tokens';
import {
  StudentDetail,
  StudentListItem,
  StudentUpsertRequest
} from '../models/student.models';

@Injectable({
  providedIn: 'root'
})
export class StudentsApiService {
  private readonly http = inject(HttpClient);
  private readonly apiBaseUrl = inject(API_BASE_URL);

  getStudents(): Observable<StudentListItem[]> {
    return this.http.get<StudentListItem[]>(`${this.apiBaseUrl}/students`);
  }

  getStudent(id: string): Observable<StudentDetail> {
    return this.http.get<StudentDetail>(`${this.apiBaseUrl}/students/${id}`);
  }

  createStudent(request: StudentUpsertRequest): Observable<StudentDetail> {
    return this.http.post<StudentDetail>(`${this.apiBaseUrl}/students`, request);
  }

  updateStudent(id: string, request: StudentUpsertRequest): Observable<StudentDetail> {
    return this.http.put<StudentDetail>(`${this.apiBaseUrl}/students/${id}`, request);
  }

  deleteStudent(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiBaseUrl}/students/${id}`);
  }
}

