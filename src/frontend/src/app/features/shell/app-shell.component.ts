import { CommonModule } from '@angular/common';
import { Component, computed, inject } from '@angular/core';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';

import { StudentSessionService } from '../../core/session/student-session.service';

@Component({
  selector: 'app-shell',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive, RouterOutlet],
  templateUrl: './app-shell.component.html',
  styleUrl: './app-shell.component.scss'
})
export class AppShellComponent {
  private readonly sessionService = inject(StudentSessionService);
  private readonly router = inject(Router);

  readonly session = this.sessionService.session;
  readonly roleLabel = computed(() => this.session()?.role === 'administrador' ? 'Administrador' : 'Estudiante');
  readonly navigation = computed(() => {
    const session = this.session();
    if (!session) {
      return [
        {
          label: 'Acceso',
          description: 'Administrador o estudiante',
          link: '/login',
          exact: true
        }
      ];
    }

    if (session.role === 'administrador') {
      return [
        {
          label: 'Panel administrativo',
          description: 'Listado y seguimiento',
          link: '/students',
          exact: true
        },
        {
          label: 'Nuevo registro',
          description: 'Crear estudiante',
          link: '/students/new',
          exact: true
        }
      ];
    }

    return [
      {
        label: 'Portal estudiante',
        description: 'Materias y companeros',
        link: '/portal',
        exact: true
      },
      {
        label: 'Mi registro',
        description: 'Editar perfil y materias',
        link: `/students/${session.userId}/edit`,
        exact: true
      }
    ];
  });

  async logout(): Promise<void> {
    this.sessionService.clearSession();
    await this.router.navigate(['/login']);
  }
}
