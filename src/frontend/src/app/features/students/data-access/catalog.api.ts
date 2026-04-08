import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { API_BASE_URL } from '../../../core/api/api.tokens';
import { ProfessorSummary, SubjectSummary } from '../models/student.models';

@Injectable({
  providedIn: 'root'
})
export class CatalogApiService {
  private readonly http = inject(HttpClient);
  private readonly apiBaseUrl = inject(API_BASE_URL);

  getSubjects(): Observable<SubjectSummary[]> {
    return this.http.get<SubjectSummary[]>(`${this.apiBaseUrl}/subjects`);
  }

  getProfessors(): Observable<ProfessorSummary[]> {
    return this.http.get<ProfessorSummary[]>(`${this.apiBaseUrl}/professors`);
  }
}

