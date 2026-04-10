import { Routes } from '@angular/router';

import { portalGuard } from './core/session/portal.guard';

export const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    redirectTo: 'login'
  },
  {
    path: 'login',
    loadComponent: () =>
      import('./features/portal/pages/student-login.page').then((module) => module.StudentLoginPage)
  },
  {
    path: 'portal',
    canActivate: [portalGuard],
    loadComponent: () =>
      import('./features/portal/pages/student-portal.page').then((module) => module.StudentPortalPage)
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
    redirectTo: 'login'
  }
];
