import { CommonModule, DatePipe } from '@angular/common';
import { Component, OnInit, computed, inject } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';

import { EmptyStateComponent } from '../../../shared/ui/empty-state/empty-state.component';
import { SectionCardComponent } from '../../../shared/ui/section-card/section-card.component';
import { StudentsFacade } from '../data-access/students.facade';

@Component({
  selector: 'app-student-detail-page',
  standalone: true,
  imports: [CommonModule, RouterLink, DatePipe, SectionCardComponent, EmptyStateComponent],
  templateUrl: './student-detail.page.html',
  styleUrl: './student-detail.page.scss'
})
export class StudentDetailPage implements OnInit {
  private readonly route = inject(ActivatedRoute);
  readonly router = inject(Router);
  readonly facade = inject(StudentsFacade);

  readonly student = computed(() => this.facade.studentDetail());
  readonly classmatesCount = computed(() => {
    const subjects = this.student()?.enrollment?.subjects ?? [];
    return new Set(subjects.flatMap((subject) => subject.classmates)).size;
  });

  async ngOnInit(): Promise<void> {
    const studentId = this.route.snapshot.paramMap.get('id');
    if (studentId) {
      await this.facade.loadStudent(studentId);
    }
  }

  async deleteStudent(): Promise<void> {
    const student = this.student();
    if (!student) {
      return;
    }

    const confirmed = window.confirm(`Eliminar a ${student.fullName}? Esta accion no se puede deshacer.`);
    if (!confirmed) {
      return;
    }

    await this.facade.deleteStudent(student.id);
    await this.router.navigate(['/students']);
  }
}
