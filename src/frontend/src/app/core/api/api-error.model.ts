export interface ApiErrorResponse {
  type?: string;
  title?: string;
  status?: number;
  detail?: string;
  instance?: string;
  traceId?: string;
  errors?: Record<string, string[] | string>;
}

export interface NormalizedApiError {
  message: string;
  status?: number;
  fieldErrors: Record<string, string[]>;
}

