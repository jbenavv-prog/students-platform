import { CommonModule, DatePipe } from '@angular/common';
import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';

import { StudentsFacade } from '../data-access/students.facade';
import { StudentListItem } from '../models/student.models';
import { EmptyStateComponent } from '../../../shared/ui/empty-state/empty-state.component';
import { SectionCardComponent } from '../../../shared/ui/section-card/section-card.component';

@Component({
  selector: 'app-students-list-page',
  standalone: true,
  imports: [CommonModule, RouterLink, DatePipe, SectionCardComponent, EmptyStateComponent],
  templateUrl: './students-list.page.html',
  styleUrl: './students-list.page.scss'
})
export class StudentsListPage implements OnInit {
  readonly facade = inject(StudentsFacade);
  readonly searchTerm = signal('');

  readonly filteredStudents = computed(() => {
    const term = this.searchTerm().trim().toLowerCase();
    const students = this.facade.students();

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
    const students = this.facade.students();
    const enrolled = students.filter((student) => student.subjectsCount > 0).length;
    const totalCredits = students.reduce((sum, student) => sum + student.credits, 0);
    const fullLoad = students.filter((student) => student.subjectsCount === 3).length;

    return {
      totalStudents: students.length,
      enrolledStudents: enrolled,
      averageCredits: students.length ? (totalCredits / students.length).toFixed(1) : '0.0',
      fullLoadStudents: fullLoad
    };
  });

  ngOnInit(): void {
    void this.facade.loadStudents();
  }

  setSearchTerm(value: string): void {
    this.searchTerm.set(value);
  }

  trackByStudentId(_: number, student: StudentListItem): string {
    return student.id;
  }

  async deleteStudent(student: StudentListItem): Promise<void> {
    const confirmed = window.confirm(`Eliminar a ${student.fullName}? Esta accion no se puede deshacer.`);
    if (!confirmed) {
      return;
    }

    await this.facade.deleteStudent(student.id);
  }
}
