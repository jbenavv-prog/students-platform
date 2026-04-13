import { Routes } from '@angular/router';

import {
  adminGuard,
  adminOrSelfStudentGuard,
  loginGuard,
  studentPortalGuard
} from './core/session/session.guard';

export const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    redirectTo: 'login'
  },
  {
    path: 'login',
    canActivate: [loginGuard],
    loadComponent: () =>
      import('./features/portal/pages/student-login.page').then((module) => module.StudentLoginPage)
  },
  {
    path: 'portal',
    canActivate: [studentPortalGuard],
    loadComponent: () =>
      import('./features/portal/pages/student-portal.page').then((module) => module.StudentPortalPage)
  },
  {
    path: 'students',
    canActivate: [adminGuard],
    loadComponent: () =>
      import('./features/students/pages/students-list.page').then((module) => module.StudentsListPage)
  },
  {
    path: 'students/new',
    canActivate: [adminGuard],
    loadComponent: () =>
      import('./features/students/pages/student-form.page').then((module) => module.StudentFormPage)
  },
  {
    path: 'students/:id/edit',
    canActivate: [adminOrSelfStudentGuard],
    loadComponent: () =>
      import('./features/students/pages/student-form.page').then((module) => module.StudentFormPage)
  },
  {
    path: 'students/:id',
    canActivate: [adminOrSelfStudentGuard],
    loadComponent: () =>
      import('./features/students/pages/student-detail.page').then((module) => module.StudentDetailPage)
  },
  {
    path: '**',
    redirectTo: 'login'
  }
];
