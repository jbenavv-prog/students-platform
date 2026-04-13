import { inject } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivateFn, Router } from '@angular/router';

import { StudentSessionService } from './student-session.service';

function redirectToLogin(router: Router) {
  return router.createUrlTree(['/login']);
}

function redirectToDefaultRoute(sessionService: StudentSessionService, router: Router) {
  return router.parseUrl(sessionService.defaultRoute());
}

export const loginGuard: CanActivateFn = () => {
  const sessionService = inject(StudentSessionService);
  const router = inject(Router);

  return sessionService.session()
    ? redirectToDefaultRoute(sessionService, router)
    : true;
};

export const studentPortalGuard: CanActivateFn = () => {
  const sessionService = inject(StudentSessionService);
  const router = inject(Router);
  const session = sessionService.session();

  if (!session) {
    return redirectToLogin(router);
  }

  return session.role === 'estudiante'
    ? true
    : redirectToDefaultRoute(sessionService, router);
};

export const adminGuard: CanActivateFn = () => {
  const sessionService = inject(StudentSessionService);
  const router = inject(Router);
  const session = sessionService.session();

  if (!session) {
    return redirectToLogin(router);
  }

  return session.role === 'administrador'
    ? true
    : redirectToDefaultRoute(sessionService, router);
};

export const adminOrSelfStudentGuard: CanActivateFn = (route: ActivatedRouteSnapshot) => {
  const sessionService = inject(StudentSessionService);
  const router = inject(Router);
  const session = sessionService.session();

  if (!session) {
    return redirectToLogin(router);
  }

  return sessionService.canAccessStudent(route.paramMap.get('id'))
    ? true
    : redirectToDefaultRoute(sessionService, router);
};
