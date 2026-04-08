import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    redirectTo: 'students'
  },
  {
    path: 'students',
    loadComponent: () =>
      import('./features/students/pages/students-list.page').then((module) => module.StudentsListPage)
  },
  {
    path: 'students/new',
    loadComponent: () =>
      import('./features/students/pages/student-form.page').then((module) => module.StudentFormPage)
  },
  {
    path: 'students/:id/edit',
    loadComponent: () =>
      import('./features/students/pages/student-form.page').then((module) => module.StudentFormPage)
  },
  {
    path: 'students/:id',
    loadComponent: () =>
      import('./features/students/pages/student-detail.page').then((module) => module.StudentDetailPage)
  },
  {
    path: '**',
    redirectTo: 'students'
  }
];
