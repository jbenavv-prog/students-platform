import { inject, Injectable, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';

import { normalizeApiError } from '../../../core/api/api-error.mapper';
import { CatalogApiService } from './catalog.api';
import { StudentsApiService } from './students.api';
import {
  ProfessorSummary,
  StudentDetail,
  StudentListItem,
  StudentUpsertRequest,
  SubjectSummary
} from '../models/student.models';

@Injectable({
  providedIn: 'root'
})
export class StudentsFacade {
  private readonly studentsApi = inject(StudentsApiService);
  private readonly catalogApi = inject(CatalogApiService);

  readonly students = signal<StudentListItem[]>([]);
  readonly studentDetail = signal<StudentDetail | null>(null);
  readonly subjects = signal<SubjectSummary[]>([]);
  readonly professors = signal<ProfessorSummary[]>([]);
  readonly loading = signal(false);
  readonly submitting = signal(false);
  readonly error = signal<string | null>(null);

  async loadStudents(): Promise<void> {
    this.loading.set(true);
    this.error.set(null);

    try {
      const students = await firstValueFrom(this.studentsApi.getStudents());
      this.students.set(students);
    } catch (error) {
      this.error.set(normalizeApiError(error).message);
    } finally {
      this.loading.set(false);
    }
  }

  async loadStudent(id: string): Promise<StudentDetail | null> {
    this.loading.set(true);
    this.error.set(null);

    try {
      const student = await firstValueFrom(this.studentsApi.getStudent(id));
      this.studentDetail.set(student);
      return student;
    } catch (error) {
      this.error.set(normalizeApiError(error).message);
      return null;
    } finally {
      this.loading.set(false);
    }
  }

  async loadCatalog(): Promise<void> {
    this.loading.set(true);
    this.error.set(null);

    try {
      const [subjects, professors] = await Promise.all([
        firstValueFrom(this.catalogApi.getSubjects()),
        firstValueFrom(this.catalogApi.getProfessors())
      ]);

      this.subjects.set(subjects);
      this.professors.set(professors);
    } catch (error) {
      this.error.set(normalizeApiError(error).message);
    } finally {
      this.loading.set(false);
    }
  }

  async createStudent(request: StudentUpsertRequest): Promise<StudentDetail> {
    this.submitting.set(true);
    this.error.set(null);

    try {
      const student = await firstValueFrom(this.studentsApi.createStudent(request));
      this.studentDetail.set(student);
      return student;
    } catch (error) {
      const normalized = normalizeApiError(error);
      this.error.set(normalized.message);
      throw error;
    } finally {
      this.submitting.set(false);
    }
  }

  async updateStudent(id: string, request: StudentUpsertRequest): Promise<StudentDetail> {
    this.submitting.set(true);
    this.error.set(null);

    try {
      const student = await firstValueFrom(this.studentsApi.updateStudent(id, request));
      this.studentDetail.set(student);
      return student;
    } catch (error) {
      const normalized = normalizeApiError(error);
      this.error.set(normalized.message);
      throw error;
    } finally {
      this.submitting.set(false);
    }
  }

  async deleteStudent(id: string): Promise<void> {
    this.submitting.set(true);
    this.error.set(null);

    try {
      await firstValueFrom(this.studentsApi.deleteStudent(id));
      this.students.update((students) => students.filter((student) => student.id !== id));
      if (this.studentDetail()?.id === id) {
        this.studentDetail.set(null);
      }
    } catch (error) {
      this.error.set(normalizeApiError(error).message);
      throw error;
    } finally {
      this.submitting.set(false);
    }
  }

  clearStudentDetail(): void {
    this.studentDetail.set(null);
  }
}

