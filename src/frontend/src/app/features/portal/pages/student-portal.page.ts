import { CommonModule, DatePipe } from '@angular/common';
import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';

import { StudentSessionService } from '../../../core/session/student-session.service';
import { EmptyStateComponent } from '../../../shared/ui/empty-state/empty-state.component';
import { SectionCardComponent } from '../../../shared/ui/section-card/section-card.component';
import { StudentsFacade } from '../../students/data-access/students.facade';
import { StudentListItem } from '../../students/models/student.models';

@Component({
  selector: 'app-student-portal-page',
  standalone: true,
  imports: [CommonModule, RouterLink, DatePipe, SectionCardComponent, EmptyStateComponent],
  templateUrl: './student-portal.page.html',
  styleUrl: './student-portal.page.scss'
})
export class StudentPortalPage implements OnInit {
  private readonly sessionService = inject(StudentSessionService);
  readonly facade = inject(StudentsFacade);
  readonly router = inject(Router);

  readonly session = this.sessionService.session;
  readonly searchTerm = signal('');

  readonly student = computed(() => this.facade.studentDetail());
  readonly classmatesCount = computed(() => {
    const subjects = this.student()?.enrollment?.subjects ?? [];
    return new Set(subjects.flatMap((subject) => subject.classmates)).size;
  });

  readonly otherStudents = computed(() => {
    const currentId = this.session()?.userId;
    if (!currentId) {
      return [];
    }

    return this.facade.students().filter((student) => student.id !== currentId);
  });

  readonly filteredOtherStudents = computed(() => {
    const term = this.searchTerm().trim().toLowerCase();
    const students = this.otherStudents();

    if (!term) {
      return students;
    }

    return students.filter((student) =>
      [student.fullName, student.email, student.programName].some((value) =>
        value.toLowerCase().includes(term)
      )
    );
  });

  readonly stats = computed(() => {
    const current = this.student();
    const subjectsCount = current?.enrollment?.subjects?.length ?? 0;
    const totalCredits = current?.enrollment?.totalCredits ?? 0;

    return {
      subjectsCount,
      totalCredits,
      classmates: this.classmatesCount(),
      otherStudents: this.otherStudents().length
    };
  });

  async ngOnInit(): Promise<void> {
    const session = this.session();
    if (!session) {
      return;
    }

    if (this.facade.students().length === 0) {
      await this.facade.loadStudents();
    }

    const current = this.facade.studentDetail();
    if (!current || current.id !== session.userId) {
      await this.facade.loadStudent(session.userId);
    }
  }

  setSearchTerm(value: string): void {
    this.searchTerm.set(value);
  }

  trackByStudentId(_: number, student: StudentListItem): string {
    return student.id;
  }

  async logout(): Promise<void> {
    this.sessionService.clearSession();
    await this.router.navigate(['/login']);
  }
}
