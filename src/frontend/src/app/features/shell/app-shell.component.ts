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
