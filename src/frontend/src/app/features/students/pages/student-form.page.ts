import { CommonModule } from '@angular/common';
import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';

import { normalizeApiError } from '../../../core/api/api-error.mapper';
import { StudentSessionService } from '../../../core/session/student-session.service';
import { SectionCardComponent } from '../../../shared/ui/section-card/section-card.component';
import { analyzeSubjectSelection, canAddSubject } from '../../../shared/utils/student-selection.util';
import { StudentsFacade } from '../data-access/students.facade';
import { StudentUpsertRequest, SubjectSummary } from '../models/student.models';

@Component({
  selector: 'app-student-form-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink, SectionCardComponent],
  templateUrl: './student-form.page.html',
  styleUrl: './student-form.page.scss'
})
export class StudentFormPage implements OnInit {
  readonly facade = inject(StudentsFacade);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly formBuilder = inject(NonNullableFormBuilder);
  private readonly sessionService = inject(StudentSessionService);

  readonly ready = signal(false);
  readonly studentId = signal<string | null>(null);
  readonly submitAttempted = signal(false);
  readonly submissionError = signal<string | null>(null);
  readonly serverFieldErrors = signal<Record<string, string[]>>({});
  readonly selectedSubjectIds = signal<string[]>([]);

  readonly form = this.formBuilder.group({
    fullName: this.formBuilder.control('', [Validators.required, Validators.maxLength(150)]),
    email: this.formBuilder.control('', [Validators.required, Validators.email]),
    programName: this.formBuilder.control('', [Validators.required, Validators.maxLength(150)]),
    subjectIds: this.formBuilder.control<string[]>([]),
    password: this.formBuilder.control('')
  });

  readonly isEditing = computed(() => Boolean(this.studentId()));
  readonly isOwnRecord = computed(() => {
    const session = this.sessionService.session();
    return Boolean(session && this.studentId() && session.role === 'estudiante' && session.userId === this.studentId());
  });
  readonly backLink = computed(() => this.isOwnRecord() ? '/portal' : '/students');
  readonly subjects = computed(() => this.facade.subjects());
  readonly selection = computed(() =>
    analyzeSubjectSelection(this.selectedSubjectIds(), this.subjects())
  );

  constructor() {
    this.form.controls.subjectIds.valueChanges
      .pipe(takeUntilDestroyed())
      .subscribe((value) => {
        this.selectedSubjectIds.set(value);
        this.serverFieldErrors.update((current) => ({ ...current, subjectIds: [] }));
      });
  }

  async ngOnInit(): Promise<void> {
    this.facade.clearStudentDetail();
    await this.facade.loadCatalog();

    const studentId = this.route.snapshot.paramMap.get('id');
    if (studentId) {
      this.studentId.set(studentId);
      this.configurePasswordValidation(true);
      const student = await this.facade.loadStudent(studentId);

      if (student) {
        this.form.setValue({
          fullName: student.fullName,
          email: student.email,
          programName: student.programName,
          subjectIds: student.enrollment?.subjects.map((subject) => subject.subjectId) ?? [],
          password: ''
        });
      }
    } else {
      this.configurePasswordValidation(false);
    }

    this.selectedSubjectIds.set(this.form.controls.subjectIds.value);
    this.ready.set(true);
  }

  isSelected(subjectId: string): boolean {
    return this.selectedSubjectIds().includes(subjectId);
  }

  isDisabled(subject: SubjectSummary): boolean {
    return !this.isSelected(subject.id) && !canAddSubject(subject.id, this.selectedSubjectIds(), this.subjects());
  }

  toggleSubject(subjectId: string): void {
    const selected = this.selectedSubjectIds();
    const nextValue = selected.includes(subjectId)
      ? selected.filter((id) => id !== subjectId)
      : [...selected, subjectId];

    this.form.controls.subjectIds.setValue(nextValue);
    this.form.controls.subjectIds.markAsTouched();
    this.submissionError.set(null);
  }

  fieldError(fieldName: 'fullName' | 'email' | 'programName' | 'subjectIds' | 'password'): string | null {
    const serverMessage = this.serverFieldErrors()[fieldName]?.[0];
    if (serverMessage) {
      return serverMessage;
    }

    if (fieldName === 'subjectIds') {
      return this.submitAttempted() || this.form.controls.subjectIds.touched
        ? this.selection().messages[0] ?? null
        : null;
    }

    const control = this.form.controls[fieldName];
    if (!this.submitAttempted() && !control.touched) {
      return null;
    }

    if (control.hasError('required')) {
      if (fieldName === 'fullName') {
        return 'El nombre completo es obligatorio.';
      }

      if (fieldName === 'email') {
        return 'El correo es obligatorio.';
      }

      if (fieldName === 'password') {
        return 'La clave es obligatoria.';
      }

      return 'El programa academico es obligatorio.';
    }

    if (fieldName === 'email' && control.hasError('email')) {
      return 'Debes ingresar un correo valido.';
    }

    return null;
  }

  async submit(): Promise<void> {
    this.submitAttempted.set(true);
    this.submissionError.set(null);
    this.serverFieldErrors.set({});

    if (this.form.invalid || !this.selection().valid) {
      this.form.markAllAsTouched();
      this.submissionError.set(this.selection().messages[0] ?? 'Revisa los campos obligatorios antes de guardar.');
      return;
    }

    const request: StudentUpsertRequest = {
      fullName: this.form.controls.fullName.value.trim(),
      email: this.form.controls.email.value.trim(),
      programName: this.form.controls.programName.value.trim(),
      subjectIds: this.form.controls.subjectIds.value
    };

    const password = this.form.controls.password.value.trim();
    if (password) {
      request.password = password;
    }

    try {
      const student = this.studentId()
        ? await this.facade.updateStudent(this.studentId()!, request)
        : await this.facade.createStudent(request);

      await this.facade.loadStudents();
      await this.router.navigate(this.isOwnRecord() ? ['/portal'] : ['/students', student.id]);
    } catch (error) {
      const normalized = normalizeApiError(error);
      this.serverFieldErrors.set(normalized.fieldErrors);
      this.submissionError.set(normalized.message);
    }
  }

  private configurePasswordValidation(isEditing: boolean): void {
    const validators = isEditing ? [] : [Validators.required];
    this.form.controls.password.setValidators(validators);
    this.form.controls.password.updateValueAndValidity();
  }
}
