import { HttpErrorResponse } from '@angular/common/http';

import { ApiErrorResponse, NormalizedApiError } from './api-error.model';

function toMessages(value: string[] | string | undefined): string[] {
  if (!value) {
    return [];
  }

  return Array.isArray(value) ? value : [value];
}

export function normalizeApiError(error: unknown): NormalizedApiError {
  if (error instanceof HttpErrorResponse) {
    const body = error.error as ApiErrorResponse | string | null;

    if (typeof body === 'string') {
      return {
        message: body,
        status: error.status,
        fieldErrors: {}
      };
    }

    if (body && typeof body === 'object') {
      const fieldErrors: Record<string, string[]> = {};

      for (const [key, value] of Object.entries(body.errors ?? {})) {
        fieldErrors[key] = toMessages(value);
      }

      return {
        message: body.detail ?? body.title ?? error.message ?? 'No pudimos completar la solicitud.',
        status: body.status ?? error.status,
        fieldErrors
      };
    }

    return {
      message: error.message || 'No pudimos completar la solicitud.',
      status: error.status,
      fieldErrors: {}
    };
  }

  if (error instanceof Error) {
    return {
      message: error.message,
      fieldErrors: {}
    };
  }

  return {
    message: 'Ocurrio un error inesperado.',
    fieldErrors: {}
  };
}
