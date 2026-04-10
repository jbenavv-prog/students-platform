import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-shell',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive, RouterOutlet],
  templateUrl: './app-shell.component.html',
  styleUrl: './app-shell.component.scss'
})
export class AppShellComponent {
  readonly navigation = [
    {
      label: 'Acceso estudiante',
      description: 'Correo y clave',
      link: '/login',
      exact: true
    },
    {
      label: 'Portal estudiante',
      description: 'Materias y companeros',
      link: '/portal',
      exact: true
    },
    {
      label: 'Estudiantes',
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
