import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { firstValueFrom } from 'rxjs';

import { normalizeApiError } from '../../../core/api/api-error.mapper';
import { StudentSessionService } from '../../../core/session/student-session.service';
import { SectionCardComponent } from '../../../shared/ui/section-card/section-card.component';
import { AuthApiService } from '../data-access/auth.api';

@Component({
  selector: 'app-student-login-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink, SectionCardComponent],
  templateUrl: './student-login.page.html',
  styleUrl: './student-login.page.scss'
})
export class StudentLoginPage {
  private readonly authApi = inject(AuthApiService);
  private readonly sessionService = inject(StudentSessionService);
  private readonly router = inject(Router);
  private readonly formBuilder = inject(NonNullableFormBuilder);

  readonly submitAttempted = signal(false);
  readonly loginError = signal<string | null>(null);
  readonly submitting = signal(false);

  readonly form = this.formBuilder.group({
    email: this.formBuilder.control('', [Validators.required, Validators.email]),
    password: this.formBuilder.control('', [Validators.required])
  });

  fieldError(fieldName: 'email' | 'password'): string | null {
    const control = this.form.controls[fieldName];
    if (!this.submitAttempted() && !control.touched) {
      return null;
    }

    if (control.hasError('required')) {
      return fieldName === 'email' ? 'El correo es obligatorio.' : 'La clave es obligatoria.';
    }

    if (fieldName === 'email' && control.hasError('email')) {
      return 'Debes ingresar un correo valido.';
    }

    return null;
  }

  async submit(): Promise<void> {
    this.submitAttempted.set(true);
    this.loginError.set(null);

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.submitting.set(true);

    try {
      const email = this.form.controls.email.value.trim().toLowerCase();
      const password = this.form.controls.password.value;
      const session = await firstValueFrom(this.authApi.login({ email, password }));
      this.sessionService.setSession(session);
      await this.router.navigate(['/portal']);
    } catch (error) {
      this.loginError.set(normalizeApiError(error).message);
    } finally {
      this.submitting.set(false);
    }
  }
}
